using System.Collections.Generic;

namespace Podcasts.Data
{
    public class Channel
    {
        public Channel()
        {
            Items = new List<PodcastItem>();
        }
        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Description { get; set; }

        public string Author { get; set; }

        public List<PodcastItem>Items { get; protected set; }
    }
}