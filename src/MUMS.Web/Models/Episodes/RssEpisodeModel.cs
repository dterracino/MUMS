using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MUMS.Web.Models
{
    public class RssEpisodeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int SecondsSinceAdded { get; set; }
    }
}
