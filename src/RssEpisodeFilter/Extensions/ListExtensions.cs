using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MUMS.RssEpisodeFilter.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// Shuffles an IList using a <see cref="RNGCryptoServiceProvider"/>.
        /// http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp/1262619#1262619
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
