using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MUMS.Data;
using MUMS.Web.Config;
using MUMS.Web.Models;
using UTorrentAPI;
using System.Web.Script.Serialization;

namespace MUMS.Web.Controllers
{
    [UserAuth]
    public partial class RootController : MumsController
    {
        public virtual ActionResult Index()
        {
            var model = new IndexModel()
            {
                DetailsModel = new JavaScriptSerializer().Serialize(new DetailsModel())
            };

            return View(model);
        }

        public virtual ActionResult GetEpisodes()
        {
            try
            {
                var model = new PollEpisodesModel();

                using (var ctx = new MumsDataContext())
                {
                    DateTime now = DateTime.Now;

                    var items = FeedController.GetItems(6);

                    model.LatestEpisodes = items.Select(e => new RssEpisodeModel
                    {
                        Name = e.ReleaseName.Trim(),
                        SecondsSinceAdded = (int)(now - e.Added).TotalSeconds,
                        Id = e.RssEpisodeItemId.ToString(),
                        ImageUrl = Url.Action(MVC.Image.Episode(e.RssEpisodeItemId)),
                        ShowName = e.ShowName,
                        Season = e.Season,
                        Episode = e.Episode
                    }).ToList();
                }

                return JsonContract(model);
            }
            catch (Exception ex)
            {
                return JsonContract(new TorrentResult { Ok = false, ErrorMessage = ex.Message });
            }
        }

        public virtual ActionResult GetTorrents()
        {
            try
            {
                var pollModel = new PollTorrentsModel { Sections = new List<Section>() };
                var client = CurrentSession.Client;

                var torrents = client.Torrents
                    .Select(t => new TorrentModel(t))
                    .ToList();

                torrents.ForEach(t =>
                {
                    pollModel.DownloadSpeedInBytes += t.DownloadSpeedInBytes;
                    pollModel.UploadSpeedInBytes += t.UploadSpeedInBytes;
                });

                var grouped = torrents
                    .GroupBy(t => new { Status = (int)t.Status, Finished = t.Percentage == 100 });

                foreach (var grouping in grouped)
                {
                    var sect = new Section
                    {
                        Status = grouping.Key.Status,
                        Finished = grouping.Key.Finished,
                        Id = "section-" + grouping.Key.Status + "-" + grouping.Key.Finished,
                        Torrents = grouping.ToList().OrderBy(t => t.QueueOrder).ToList()
                    };

                    pollModel.Sections.Add(sect);
                }

                pollModel.Sections.Sort(new Comparison<Section>((s1, s2) =>
                {
                    bool d1 = DownloadingOrQueued((TorrentStatus)s1.Status, s1.Finished);
                    bool d2 = DownloadingOrQueued((TorrentStatus)s2.Status, s2.Finished);

                    if (d1 == d2)
                        return s1.Id.CompareTo(s2.Id);
                    else
                        return d2.CompareTo(d1);
                }));

                return JsonContract(pollModel);
            }
            catch (Exception ex)
            {
                return JsonContract(new TorrentResult { Ok = false, ErrorMessage = ex.Message });
            }
        }

        public bool DownloadingOrQueued(TorrentStatus status, bool finished)
        {
            if (finished)
                return false;

            if ((status & TorrentStatus.Started) != 0)
            {
                if ((status & TorrentStatus.Paused) == 0)
                    return true;
            }
            else if ((status & TorrentStatus.Queued) != 0)
            {
                return true;
            }

            return false;
        }

        public virtual ActionResult AddRemoteUrl(string url, string label, string hash)
        {
            return AddTorrent(url, label, hash);
        }

        [HttpPost]
        public virtual ActionResult AddTorrent(string url, string label, string hash)
        {
            var torrentFiles = Request.Files;

            if (HasFileUploads(torrentFiles))
                return UploadFiles(label);

            if (string.IsNullOrWhiteSpace(url))
            {
                if (Request.IsAjaxRequest())
                    return JsonContract(new TorrentResult { Ok = false, ErrorMessage = "Empty url" });

                return RedirectToAction(MVC.Root.Index());
            }

            if (url.StartsWith("magnet:"))
            {
                StartMagnetUri(url, label, hash);

                if (Request.IsAjaxRequest())
                    return JsonContract(new TorrentResult { Ok = true, Hash=hash });

                return RedirectToAction(MVC.Root.Index());
            }

            var req = HttpWebRequest.Create(url) as HttpWebRequest;
            var cookies = GetCookies(req.RequestUri).ToList();

            if (cookies.Count > 0 || string.IsNullOrWhiteSpace(hash))
            {
                try
                {
                    req.AllowAutoRedirect = true;
                    AssertAuthenticatedRequest(req);

                    using (var readStream = req.GetResponse().GetResponseStream())
                        hash = SaveTorrentStream(readStream, label);
                }
                catch (Exception ex)
                {
                    if (Request.IsAjaxRequest())
                        return JsonContract(new TorrentResult { Ok = false, ErrorMessage = ex.ToString() });

                    return RedirectToAction(MVC.Root.Index());
                }
            }
            else
            {
                CurrentSession.Client.Torrents.AddUrl(url);
                SetLabel(label, hash);
            }

            if (Request.IsAjaxRequest())
            {
                return JsonContract(new TorrentResult
                {
                    Ok = true,
                    Hash = hash
                });
            }

            return RedirectToAction(MVC.Root.Index());
        }

        protected void StartMagnetUri(string url, string label, string hash)
        {
            CurrentSession.Client.Torrents.AddUrl(url);
            
            Uri result;
            if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out result))
            {
                var queryParts = HttpUtility.ParseQueryString(result.Query);
                string xt = queryParts.Get("xt");
                var urnParts = xt.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

                if (urnParts.Length == 3 && urnParts.FirstOrDefault() == "urn" && urnParts.Skip(1).FirstOrDefault() == "btih")
                    hash = urnParts.Last();
            }
            
            if (!string.IsNullOrWhiteSpace(hash))
                SetLabel(label, hash);
        }

        private bool HasFileUploads(HttpFileCollectionBase torrentFiles)
        {
            if (torrentFiles == null || torrentFiles.Count == 0)
                return false;

            for (int i = 0; i < torrentFiles.Count; i++)
                if (torrentFiles[i].ContentLength > 0)
                    return true;

            return false;
        }

        private void AssertAuthenticatedRequest(HttpWebRequest req)
        {
            var cookies = GetCookies(req.RequestUri).ToList();
            if (cookies == null || cookies.Count == 0)
                return;

            var container = new CookieContainer();
            cookies.ForEach(c => container.Add(c));

            req.CookieContainer = container;
        }

        private IEnumerable<Cookie> GetCookies(Uri requestUrl)
        {
            var cookieTriggers = CookieTriggers.GetConfig();

            if (cookieTriggers == null)
                yield break;

            string domain = requestUrl.Host.ToLowerInvariant();

            var matches = cookieTriggers
                .Where(t => t.TriggerDomains.Any(d => d == domain))
                .ToList();

            foreach (var trigger in cookieTriggers)
            {
                foreach (var cookie in trigger.Cookies)
                {
                    if (string.IsNullOrWhiteSpace(cookie.Domain))
                        cookie.Domain = domain;

                    yield return cookie;
                }
            }
        }

        [HttpPost]
        public virtual ActionResult UploadFiles(string label)
        {
            var torrentFiles = Request.Files;

            if (torrentFiles == null || torrentFiles.Count == 0)
            {
                if (Request.IsAjaxRequest())
                    return JsonContract(new TorrentResult { Ok = false });

                return RedirectToAction(MVC.Root.Index());
            }

            var results = new List<TorrentResult>();

            for (int i = 0; i < torrentFiles.Count; i++)
            {
                var torrentFile = torrentFiles[i] as HttpPostedFileWrapper;

                if (torrentFile == null || torrentFile.ContentLength == 0)
                    continue;

                string hash = "";

                try
                {
                    hash = SaveTorrentStream(torrentFile.InputStream, label);
                    results.Add(new TorrentResult { Ok = true, Hash = hash });
                }
                catch (Exception ex)
                {
                    results.Add(new TorrentResult { Ok = false, ErrorMessage = ex.ToString() });
                }
            }

            if (Request.IsAjaxRequest())
                return JsonContract(results);

            return RedirectToAction(MVC.Root.Index());
        }

        [HttpPost]
        public virtual ActionResult StartTorrent(string hash)
        {
            return TorrentAction(hash, h => CurrentSession.Client.Torrents[h].Start());
        }

        [HttpPost]
        public virtual ActionResult StopTorrent(string hash)
        {
            return TorrentAction(hash, h => CurrentSession.Client.Torrents[h].Stop());
        }

        [HttpPost]
        public virtual ActionResult RemoveTorrent(string hash)
        {
            return TorrentAction(hash, h => CurrentSession.Client.Torrents.Remove(h));
        }

        [HttpPost]
        public virtual ActionResult RemoveTorrentAndData(string hash)
        {
            return TorrentAction(hash, h => CurrentSession.Client.Torrents.Remove(h, TorrentRemovalOptions.TorrentFileAndData));
        }

        [HttpPost]
        public virtual ActionResult TorrentAction(string hash, Action<string> action)
        {
            try
            {
                action(hash);
            }
            catch (Exception ex)
            {
                if(Request.IsAjaxRequest())
                return Json(new TorrentResult { Ok = false, Hash = hash, ErrorMessage = ex.ToString() });
            }

            return Json(new TorrentResult { Ok = true, Hash = hash });
        }

        private string SaveTorrentStream(Stream readStream, string label)
        {
            var guid = Guid.NewGuid();
            string fileName = guid.ToString() + ".torrent";

            var fileInfo = new FileInfo("C:\\Temp\\" + fileName);
            string fullPath = fileInfo.FullName;
            string targetPath = Path.Combine(ConfigurationManager.AppSettings["TorrentStore"], fileName);

            WriteTorrentContent(readStream, fileInfo);

            // For some reason, moving the file won't make uTorrent react to the directory change.. So copy it is.
            System.IO.File.Copy(fullPath, targetPath);
            System.IO.File.Delete(fullPath);

            string hash = CalculateHash(fileInfo);
            
            if (!string.IsNullOrWhiteSpace(label))
                SetLabel(label, hash);

            return hash;
        }

        private string CalculateHash(FileInfo fileInfo)
        {
            var dict = DotNetTorrent.BEncoding.Torrent.ParseTorrentFile(fileInfo.FullName);
            return DotNetTorrent.BEncoding.Torrent.ComputeInfoHash(dict).ToString().ToUpper();
        }

        private void WriteTorrentContent(Stream readStream, FileInfo fileInfo)
        {
            using (var writeStream = fileInfo.OpenWrite())
            {
                int length = 256;
                var buffer = new Byte[length];
                int bytesRead = readStream.Read(buffer, 0, length);

                while (bytesRead > 0)
                {
                    writeStream.Write(buffer, 0, bytesRead);
                    bytesRead = readStream.Read(buffer, 0, length);
                }
            }
        }

        [HttpPost]
        public virtual ActionResult SetLabel(string newLabel, string hash)
        {
            newLabel = newLabel ?? string.Empty;

            var client = CurrentSession.Client;
            int counter = 0;
            Torrent torrent = null;

            while (torrent == null)
            {
                torrent = client.Torrents
                    .SingleOrDefault(t => t.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase));

                if (torrent != null || counter == 10)
                    break;
                else
                    Thread.Sleep(300);

                counter++;
            }

            if (torrent != null)
                client.Torrents[torrent.Hash].Label = newLabel;

            if (Request.IsAjaxRequest())
            {
                var result = new TorrentResult
                {
                    Ok = torrent != null,
                    Hash = torrent.Hash,
                };

                if (!result.Ok)
                    result.ErrorMessage = "Hittade ingen torrent med hash " + hash;

                return JsonContract(result);
            }

            return RedirectToAction(MVC.Root.Index());
        }

        [HttpPost]
        public virtual ActionResult GetTorrent(string hash)
        {
            var torrent = CurrentSession.Client.Torrents[hash];

            if (torrent == null)
                return JsonContract(new TorrentResult { Ok = false, ErrorMessage = "Hittade ingen torrent med hash " + hash });

            var detailsModel = new DetailsModel(torrent);

            if (detailsModel.Files != null)
            {
                detailsModel.Files.Sort(new Comparison<UTorrentAPI.File>((f1, f2) =>
                {
                    double r1 = f1.DownloadedBytes / f1.SizeInBytes;
                    double r2 = f2.DownloadedBytes / f2.SizeInBytes;
                    
                    if (r1 == r2 && f1.Path != null && f2.Path != null)
                        return f1.Path.CompareTo(f2.Path);

                    return r1.CompareTo(r2);
                }));
            }

            return Json(detailsModel);
        }

        [HttpPost]
        public virtual ActionResult ClearFinished()
        {
            var client = CurrentSession.Client;

            client.Torrents
                .Where(t => t.ProgressInMils >= 1000)
                .ToList()
                .ForEach(t => client.Torrents.Remove(t.Hash, TorrentRemovalOptions.Job));

            return Json(new TorrentResult { Ok = true, Hash = string.Empty });
        }
    }
}
