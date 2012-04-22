using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using TvdbLib;
using TvdbLib.Data;
using MUMS.Data;
using System.Configuration;
using TvdbLib.Data.Banner;
using System.Threading;

namespace MUMS.Web.Controllers
{
    public partial class ImageController : Controller
    {
        public virtual ActionResult Index()
        {
            return View();
        }

        public virtual ActionResult Episode(int id)
        {
            using (var ctx = new MumsDataContext())
            {
                var data = ctx.RssEpisodeItems.Where(e => e.RssEpisodeItemId == id).Select(e => new
                {
                    e.ShowName,
                    e.Season,
                    e.Episode
                }).SingleOrDefault();

                if (data == null)
                    return new EmptyResult();

                return TvShow(data.ShowName, data.Season, data.Episode);
            }
        }

        public virtual ActionResult TvShow(string title, int season, int episode)
        {
            if (title != null)
            {
                Path.GetInvalidFileNameChars()
                    .ToList()
                    .ForEach(c => title.Replace(c, '_'));
            }

            string fileName = string.Format("{0}.S{1:00}E{2:00}.jpg", title, season, episode);

            string contentType = "image/jpeg";
            string serverPath = Server.MapPath("~/Content/tvshow/" + fileName);

            if (string.IsNullOrWhiteSpace(title))
            {
                serverPath = Server.MapPath("~/Content/images/mums.png");
                contentType = "image/png";
            }

            if (!System.IO.File.Exists(serverPath))
            {
                if (!DownloadImage(serverPath, title, season, episode))
                {
                    var client = new WebClient();
                    string text = HttpUtility.UrlEncode(string.Format("{0} S{1:00}", title, season));
                    string url = "http://placehold.it/400x200/ffffff/000000&text=" + text;
                    client.DownloadFile(url, serverPath);
                }
            }

            return File(serverPath, contentType);
        }

        private bool DownloadImage(string serverPath, string title, int season, int episode)
        {
            string apiKey = ConfigurationManager.AppSettings["tvdb.API.key"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return false;

            var handler = new TvdbHandler(apiKey);
            var downloader = new TvdbDownloader(apiKey);

            var searchResult = handler.SearchSeries(title);
            if (searchResult != null && searchResult.Count > 0)
            {
                var result = searchResult.First();
                int sId = result.Id;

                var banner = GetLoadedBanner(downloader, sId, season, episode, result.Banner);
                
                if (banner == null)
                    return false;

                banner.BannerImage.Save(serverPath);
                return true;
            }

            return false;
        }

        private TvdbBanner GetLoadedBanner(TvdbDownloader downloader, int sId, int season, int episode, TvdbBanner fallback)
        {
            TvdbBanner result = GetEpisodeBanner(downloader, sId, season, episode);
            if (result != null && TryLoadBanner(result))
                return result;

            var bannerHits = downloader.DownloadBanners(sId);

            result = GetSeasonBanner(bannerHits, season);
            if (result != null && TryLoadBanner(result))
                return result;

            result = GetSeriesBanner(bannerHits);
            if (result != null && TryLoadBanner(result))
                return result;

            if (fallback != null && TryLoadBanner(fallback))
                return fallback;

            return null;
        }

        private bool TryLoadBanner(TvdbBanner banner)
        {
            if (banner.IsLoaded)
                return true;

            try
            {
                banner.LoadBanner();
                while (banner.BannerLoading)
                    Thread.Sleep(10);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private TvdbBanner GetSeriesBanner(List<TvdbBanner> bannerHits)
        {
            try
            {
                return bannerHits
                    .OfType<TvdbSeriesBanner>()
                    .Where(b => b.BannerType == TvdbSeriesBanner.Type.graphical)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private TvdbBanner GetSeasonBanner(List<TvdbBanner> bannerHits, int season)
        {
            try
            {
                return bannerHits
                    .OfType<TvdbSeasonBanner>()
                    .Where(b => b.BannerType == TvdbSeasonBanner.Type.season)
                    .Where(b => b.Season == season)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private TvdbBanner GetEpisodeBanner(TvdbDownloader downloader, int seriesId, int season, int episode)
        {
            try
            {
                var tvEpisode = downloader.DownloadEpisode(seriesId, season, episode, TvdbEpisode.EpisodeOrdering.DefaultOrder, TvdbLanguage.DefaultLanguage);
                return tvEpisode.Banner;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
