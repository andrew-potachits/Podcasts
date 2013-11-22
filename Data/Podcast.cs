using System.Collections.Generic;

namespace Podcasts.Data
{
    public class Podcast
    {
        public Podcast()
        {
            Channels = new List<Channel>();
        }

        public IList<Channel> Channels { get; protected set; }


    }
}