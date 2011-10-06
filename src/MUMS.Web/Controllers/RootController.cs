using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using MUMS.Utorrent.Model;
using MUMS.Utorrent.Service;
using MUMS.Web.Models;
using MUMS.Web.Config;

namespace MUMS.Web.Controllers
{
    [UserAuth]
    public partial class RootController : MumsController
    {
        public virtual ActionResult Index()
        {
            var cookies = CookieTriggers.GetConfig();

            return View();
        }

        public virtual ActionResult GetTorrents()
        {
            try
            {
                var pollModel = new PollTorrentsModel { Sections = new List<Section>() };
                var torrents = CurrentSession.Client.GetTorrents();

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

                return JsonContract(pollModel);
            }
            catch (Exception ex)
            {
                return JsonContract(new TorrentResult { Ok = false, ErrorMessage = ex.Message });
            }
        }

        public virtual ActionResult AddRemoteUrl(string url, string label)
        {
            return AddUrl(url, label);
        }

        [HttpPost]
        public virtual ActionResult AddUrl(string torrentUrl, string selLabel)
        {
            string hash = "";
            
            try
            {
                var req = HttpWebRequest.Create(torrentUrl) as HttpWebRequest;
                req.AllowAutoRedirect = true;
                AssertAuthenticatedRequest(req);

                using (var readStream = req.GetResponse().GetResponseStream())
                    hash = SaveTorrentStream(readStream, selLabel);
            }
            catch(Exception ex)
            {
                if (Request.IsAjaxRequest())
                    return JsonContract(new TorrentResult { Ok = false, ErrorMessage = ex.ToString() });

                return RedirectToAction(MVC.Root.Index());
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
        public virtual ActionResult AddFile(string selLabel)
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
                    hash = SaveTorrentStream(torrentFile.InputStream, selLabel);
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
            return TorrentAction(hash, () => CurrentSession.Client.Start(hash));
        }

        [HttpPost]
        public virtual ActionResult StopTorrent(string hash)
        {
            return TorrentAction(hash, () => CurrentSession.Client.Stop(hash));
        }

        [HttpPost]
        public virtual ActionResult RemoveTorrent(string hash)
        {
            return TorrentAction(hash, () => CurrentSession.Client.Remove(hash));
        }

        [HttpPost]
        public virtual ActionResult RemoveTorrentAndData(string hash)
        {
            return TorrentAction(hash, () => CurrentSession.Client.RemoveTorrentAndData(hash));
        }

        public virtual ActionResult TorrentAction(string hash, Func<DefaultResponse> action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                return JsonContract(new TorrentResult { Ok = false, Hash = hash, ErrorMessage = ex.ToString() });
            }

            return JsonContract(new TorrentResult { Ok = true, Hash = hash });
        }

        private string SaveTorrentStream(Stream readStream, string label)
        {
            var guid = Guid.NewGuid();
            string fileName = guid.ToString() + ".torrent";
            
            var fileInfo = new FileInfo("C:\\Temp\\" + fileName);
            string fullPath = fileInfo.FullName;
            string targetPath = Path.Combine(ConfigurationManager.AppSettings["TorrentStore"], fileName);

            WriteTorrentContent(readStream, fileInfo);

            var dict = DotNetTorrent.BEncoding.Torrent.ParseTorrentFile(fileInfo.FullName);
            string hash = DotNetTorrent.BEncoding.Torrent.ComputeInfoHash(dict).ToString().ToUpper();
            
            // For some reason, moving the file won't make uTorrent react to the directory change.. So copy it is.
            System.IO.File.Copy(fullPath, targetPath);
            System.IO.File.Delete(fullPath);

            if (string.IsNullOrWhiteSpace(hash))
                return string.Empty;

            if (!string.IsNullOrWhiteSpace(label))
                SetLabel(label, hash);

            return hash;
        }

        private static void WriteTorrentContent(Stream readStream, FileInfo fileInfo)
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

        public virtual ActionResult SetLabel(string newLabel, string hash)
        {
            newLabel = newLabel ?? string.Empty;

            var client = CurrentSession.Client;
            int counter = 0;
            Torrent torrent = null;

            while (torrent == null)
            {
                torrent = client.GetTorrents()
                    .SingleOrDefault(t => t.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase));

                if (torrent != null || counter == 10)
                    break;
                else
                    Thread.Sleep(300);
                
                counter++;
            }

            if (torrent != null)
                client.SetLabel(torrent.Hash, newLabel);

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

        public virtual ActionResult GetTorrent(string hash)
        {
            var torrent = CurrentSession.Client.GetTorrents()
                .SingleOrDefault(t => t.Hash.Equals(hash, StringComparison.OrdinalIgnoreCase));

            if (torrent == null)
                return JsonContract(new TorrentResult { Ok = false, ErrorMessage = "Hittade ingen torrent med hash " + hash });

            return JsonContract(torrent);
        }

        [HttpPost]
        public virtual ActionResult ClearFinished()
        {
            var client = CurrentSession.Client;
            
            client.GetTorrents()
                .Where(t => t.Percentage >= 100)
                .ToList()
                .ForEach(t => client.Remove(t.Hash));

            return Json(new TorrentResult { Ok = true, Hash = string.Empty });
        }
    }
}
