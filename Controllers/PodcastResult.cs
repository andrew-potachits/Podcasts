using System;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml;
using Podcasts.Data;

namespace Podcasts.Controllers
{
    public class PodcastResult : FileResult
    {
        private readonly string _name;
        private readonly Func<string, string> _itemUrlFunc;

        public PodcastResult(string name, Func<string, string> itemUrlFunc)
            : base("text/xml")
        {
            _name = name;
            _itemUrlFunc = itemUrlFunc;
        }

        protected override void WriteFile(HttpResponseBase response)
        {
            var manager = new PodcastsManager(HostingEnvironment.MapPath(@"/App_Data"));

            var podcast = manager.GetPodcast(_name);

            OverridePaths(podcast);

            var podcastGenerator = new PodcastGenerator();

            using (XmlWriter writer = new XmlTextWriter(response.Output))
            {
                podcastGenerator.Generate(podcast, writer);
                writer.Flush();
            }
        }

        private void OverridePaths(Podcast podcast)
        {
            foreach (var channel in podcast.Channels)
            {
                foreach (var podcastItem in channel.Items)
                {
                    if (!File.Exists(podcastItem.Path))
                        continue;

                    podcastItem.Path = _itemUrlFunc(podcastItem.Path);

                }
            }
        }
    }
}