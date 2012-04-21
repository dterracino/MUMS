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
        public string ImageUrl { get; set; }
        
        public string ShowName { get; set; }
        public int Season { get; set; }
        public int Episode { get; set; }
    }
}
