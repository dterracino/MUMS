using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using MUMS.Utorrent.Model;

namespace MUMS.Utorrent
{
    class Program
    {
        public static void Main(string[] args)
        {
            string username = "christoffer";
            string password = "ethernet";
            string address = "http://home.chsk.se:8080/gui";

            Uri torrentUrl = new Uri("http://torrents.thepiratebay.org/5614343/Eminem-Recovery-(Retail)-2010-[NoFS].5614343.TPB.torrent");
            string torrentPath = @"C:\Users\Christoffer\Downloads\Eminem-Recovery-(Retail)-2010-[NoFS].5614343.TPB.torrent";

            // 06FBA0E9D165CA14432D431E43C29E6CD04EFD99
            
            var dict = DotNetTorrent.BEncoding.Torrent.ParseTorrentFile(torrentPath);
            string hash = DotNetTorrent.BEncoding.Torrent.ComputeInfoHash(dict).ToString().ToUpper();

            var client = new UtorrentClient(username, password, address);
            client.AddTorrentFromUrl(torrentUrl);
            
            int counter = 0;
            while (!client.GetTorrents().Any(t => t.Hash == hash) && counter < 10)
            {
                Thread.Sleep(200);
                counter++;
            }

            client.SetLabel(hash, "megatest");

            Console.ReadLine();
        }

        public static void Benchmark(string username, string password, string address)
        {
            var sw = new Stopwatch();

            sw.Start();
            var client = new UtorrentClient(username, password, address);
            sw.Stop();

            Console.WriteLine(sw.Elapsed);
            sw.Reset();

            sw.Start();
            List<Torrent> torrents = client.GetTorrents();
            sw.Stop();

            Console.WriteLine(sw.Elapsed);
            sw.Reset();

            sw.Start();
            torrents = client.GetTorrents();
            sw.Stop();

            Console.WriteLine(sw.Elapsed);

            foreach (var t in torrents)
                Console.WriteLine(t.Name);
        }
    }
}
