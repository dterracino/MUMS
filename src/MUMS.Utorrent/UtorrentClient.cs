using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text.RegularExpressions;
using MUMS.Utorrent.Model;
using MUMS.Utorrent.Service;

namespace MUMS.Utorrent
{
    /// <summary>
    /// A client that wraps, and communicates through, an IUtorrentChannel.
    /// </summary>
    public class UtorrentClient
    {
        /// <summary>
        /// Holds the "channel" that is used to communicate with the uTorrent Web API.
        /// </summary>
        private IUtorrentChannel Channel { get; set; }
        
        private string m_token;
        
        /// <summary>
        /// Holds the token that is required for calling uTorrent Web API methods.
        /// The get-method will call RetrieveToken() if m_token is not yet set.
        /// </summary>
        private string Token 
        {
            get
            {
                if (string.IsNullOrEmpty(m_token))
                    m_token = RetrieveToken();

                return m_token;
            }
            set
            {
                m_token = value;
            }
        }

        /// <summary>
        /// Constructor that inits the connection to the uTorrent Web API.
        /// </summary>
        public UtorrentClient(string uTorrentUserName, string uTorrentPassword, string uTorrentAddress)
        {
            var encodingBindingElement = new WebMessageEncodingBindingElement() 
            { 
                ContentTypeMapper = new JsonContentTypeMapper() 
            };

            var transportBindingElement = new HttpTransportBindingElement() 
            { 
                ManualAddressing = true, 
                AuthenticationScheme = AuthenticationSchemes.Basic, 
                Realm = "uTorrent", 
                AllowCookies = true,
                MaxReceivedMessageSize = 524288 // 512k ought to be enough for everybody.. ;)
            };

            var binding = new CustomBinding(encodingBindingElement, transportBindingElement);
            
            var factory = new WebChannelFactory<IUtorrentChannel>(binding);
            factory.Endpoint.Address = new EndpointAddress(uTorrentAddress);;
            factory.Credentials.UserName.UserName = uTorrentUserName;
            factory.Credentials.UserName.Password = uTorrentPassword;
            
            Channel = factory.CreateChannel();
        }

        /// <summary>
        /// Returns a list of all the torrents currently available in uTorrent.
        /// </summary>
        /// <returns></returns>
        public List<Torrent> GetTorrents()
        {
            // func takes a dummy argument (object) which will not be used in
            // actually executing the function.
            
            Func<List<Torrent>> func = () => Channel.GetList(Token)
                .Torrents
                .Select(t => new Torrent(t))
                .ToList();

            return ExecuteTokenSafe(func);
        }

        public DefaultResponse AddTorrentFromUrl(Uri url, Dictionary<string, string> cookies)
        {
            string cookieString = "";
            
            if (cookies != null)
            {
                cookieString = ":COOKIE:";
                foreach (var kp in cookies)
                    cookieString += string.Format("{0}={1}", kp.Key, kp.Value);
            }

            return ExecuteTokenSafe(() => Channel.AddTorrentFromUrl(Token, url.ToString(), cookieString));
        }

        public DefaultResponse AddTorrentFromUrl(Uri url)
        {
            return AddTorrentFromUrl(url, null);
        }

        public DefaultResponse SetLabel(Torrent torrent, string label)
        {
            return SetLabel(torrent.Hash, label);
        }

        public DefaultResponse SetLabel(string torrentHash, string label)
        {
            return ExecuteTokenSafe(() => Channel.SetProperty(Token, torrentHash, "label", label));
        }

        public DefaultResponse Start(string torrentHash)
        {
            return ExecuteTokenSafe(() => Channel.Start(Token, torrentHash));
        }

        public DefaultResponse Stop(string torrentHash)
        {
            return ExecuteTokenSafe(() => Channel.Stop(Token, torrentHash));
        }

        public DefaultResponse RemoveTorrentAndData(string torrentHash)
        {
            return ExecuteTokenSafe(() => Channel.RemoveTorrentAndData(Token, torrentHash));
        }
        
        public DefaultResponse Remove(string torrentHash)
        {
            return ExecuteTokenSafe(() => Channel.Remove(Token, torrentHash));
        }

        /// <summary>
        /// Tries to perform the given function. It wraps the function in a try/catch once,
        /// and if that fails, it tries to update the Token throught SetToken(), and then
        /// runs the function again.
        /// </summary>
        /// <typeparam name="TResult">Type of the result</typeparam>
        /// <param name="func">The function that should be executed</param>
        /// <returns>Returns the result of running the function.</returns>
        private TResult ExecuteTokenSafe<TResult>(Func<TResult> func)
        {
            try
            {
                return func();
            }
            catch(ProtocolException)
            {
                Token = RetrieveToken();
                return func();
            }
        }

        /// <summary>
        /// Queries the ./token.html page and parses the given token out of the page
        /// and returns it.
        /// </summary>
        private string RetrieveToken()
        {
            using (var reader = new StreamReader(Channel.GetToken()))
            {
                string htmlContent = reader.ReadToEnd();
                Regex r = new Regex(".*<div[^>]*id=[\"\']token[\"\'][^>]*>([^<]*)</div>.*");
                
                return r.Match(htmlContent).Result("$1");
            }
        }
    }
}
