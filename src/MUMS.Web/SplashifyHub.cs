using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SignalR.Hubs;
using MUMS.Web.Models.Splashify;

namespace MUMS.Web
{
    public class SplashifyHub: Hub
    {
        protected static PlayingModel Playing = new PlayingModel
        {
            Artist = "",
            Track = "Awkward silence?"
        };

        public void SetPlaying(PlayingModel playing)
        {
            Playing = playing;
            Clients.updatePlaying(JsonSerializer.ToJson<PlayingModel>(Playing));
        }

        public void GetPlaying()
        {
            Caller.updatePlaying(JsonSerializer.ToJson<PlayingModel>(Playing));
        }

        public void TestArtist(string artist)
        {
            Playing.Artist = artist;
            Clients.updatePlaying(JsonSerializer.ToJson<PlayingModel>(Playing));
        }
    }
}