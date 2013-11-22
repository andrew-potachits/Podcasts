using System;

namespace Podcasts.Data
{
    public class PodcastItem
    {
        public string Title { get; set; }

        public string Author { get; set; }

        public string Album { get; set; }

        public TimeSpan Duration { get; set; }

        public DateTime PubDate { get; set; }

        public string Path { get; set; }

        public string MimeType { get; set; }
    }
}