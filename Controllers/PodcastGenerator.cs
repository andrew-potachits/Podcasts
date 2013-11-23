using System;
using System.Xml;
using Podcasts.Data;

namespace Podcasts.Controllers
{
    public class PodcastGenerator
    {
        private const string iTunes = "itunes";

        public void Generate(Podcast podcast, XmlWriter writer)
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("rss");
            writer.WriteAttributeString("xmlns", iTunes, null, "http://www.itunes.com/dtds/podcast-1.0.dtd");
            writer.WriteAttributeString("version", "2.0");

            foreach (var channel in podcast.Channels)
            {
                GenerateChannel(channel, writer);
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        private void GenerateChannel(Channel channel, XmlWriter writer)
        {
            writer.WriteStartElement("channel");
            writer.WriteElementString("title", channel.Title);
            writer.WriteElementString(iTunes, "subtitle", null, channel.SubTitle);
            writer.WriteElementString(iTunes, "author", null, channel.Author);
            foreach (var item in channel.Items)
            {
                writer.WriteStartElement("item");

                GenerateChannelItem(item, writer);

                writer.WriteEndElement();   //  item
            }
            writer.WriteEndElement();   //  channel
        }

        private void GenerateChannelItem(PodcastItem item, XmlWriter writer)
        {
            writer.WriteElementString("title", item.Title);
            writer.WriteElementString(iTunes, "author", null, item.Author);
            writer.WriteElementString(iTunes, "subtitle", null, item.Title);
            writer.WriteStartElement("enclosure");
            {
                writer.WriteAttributeString("url", item.Path);
                writer.WriteAttributeString("type", item.MimeType);
                writer.WriteAttributeString("length", item.Size.ToString("D"));
            }
            writer.WriteEndElement();
            writer.WriteElementString("guid", Guid.NewGuid().ToString());
            writer.WriteElementString(iTunes, "duration", null, string.Format("{0}:{1:D2}", (int)item.Duration.TotalMinutes, item.Duration.Seconds));
            writer.WriteElementString("pubDate", item.PubDate.ToString("MM/dd/yyyy HH:mm:ss"));
        }
    }
}