using System;
using Sindicait;
using WebActivator;

[assembly: PostApplicationStartMethod(typeof(MUMS.Web.App_Start.SindicaitStartCode), "Start")]

namespace MUMS.Web.App_Start
{
    public class SindicaitStartCode
    {
        public static void Start()
        {
            // TODO: Replace the title, description (optional),
            // entryData and entryMapper properties below.
			
            DataFeeds.Register(
                "", // Root URL name
                "TODO: Place your feed's title here", // Title
                () => new [] { // Entry data
                    new { Title = "Item #1", Summary = "TODO: Put real data here", Published = DateTime.Now.AddDays(-2) },
                    new { Title = "Item #2", Summary = "TODO: Put real data here", Published = DateTime.Now.AddDays(-10) }
                },
                entryMapper: (d) => new FeedEntry {
                    Title = d.Title,
                    Content = d.Summary,
                    Published = d.Published,
                    Updated = d.Published
                },
                description: "TODO: Place your feed's description here");
        }
    }
}