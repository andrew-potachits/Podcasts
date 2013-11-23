using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Xml.XPath;
using Id3.Id3v2.v23;
using Podcasts.Data;
using Id3;

namespace Podcasts.Controllers
{
    public class PodcastsManager
    {
        enum SortMethod
        {
            None,
            Title,
            Date
        }
        public enum SortOrder
        {
            Asc,
            Desc
        }

        private readonly string _dataPath;

        public PodcastsManager(string dataPath)
        {
            _dataPath = dataPath;
        }

        public Podcast GetPodcast(string channelName)
        {
            var podcast = new Podcast();
            var searchPattern = string.IsNullOrEmpty(channelName)
                                    ? "*.xml"
                                    : string.Format("{0}.xml", channelName);
            foreach (var fileName in Directory.GetFiles(_dataPath, searchPattern, SearchOption.TopDirectoryOnly))
            {
                podcast.Channels.Add(GetPodcastChannel(fileName));
            }
            return podcast;
        }

        public Channel GetPodcastChannel(string channelName)
        {
            var channelFile = XDocument.Load(channelName);
            var podcastFolder = channelFile.XPathSelectElement("//podcastFolder");
            if (podcastFolder == null)
                throw new InvalidOperationException("XML file ust contain podcastFolder element with path to MP3 files");

            var fileMask = channelFile.XPathSelectElement("//fileMask") != null ? channelFile.XPathSelectElement("//fileMask").Value : "*.mp3";
            var recursively = channelFile.XPathSelectElement("//recursively") != null ? channelFile.XPathSelectElement("//recursively").Value : "false";
            SearchOption searchOption = recursively.ToLower() == "true"
                                            ? SearchOption.AllDirectories
                                            : SearchOption.TopDirectoryOnly;
            var channel = new Channel();
            foreach (var mp3File in Directory.GetFiles(podcastFolder.Value, fileMask, searchOption))
            {
                channel.Items.Add(GetChannelItem(mp3File));
            }

            var sortMethod = channelFile.XPathSelectElement("//SortMethod");
            var method = SortMethod.None;
            if (sortMethod != null)
                method  = (SortMethod)Enum.Parse(typeof(SortMethod), sortMethod.Value);

            var sortOrder = channelFile.XPathSelectElement("//SortOrder");
            var order = SortOrder.Asc;
            if (sortOrder != null)
                order = (SortOrder) Enum.Parse(typeof (SortOrder), sortOrder.Value);

            SortItems(channel.Items, method, order);

            channel.Title = channel.Items[0].Album;
            channel.SubTitle = channel.Items[0].Album;
            channel.Author = channel.Items[0].Author;
            return channel;
        }

        private void SortItems(IList<PodcastItem> items, SortMethod sortMethod, SortOrder order)
        {
            
        }

        private PodcastItem GetChannelItem(string mp3File)
        {
            using (Stream stm = new FileStream(mp3File, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var mp3 = new Mp3Stream(stm))
            {
                var item = new PodcastItem();
                Id3Tag id3Tag;
                if (mp3.HasTagOfFamily(Id3TagFamily.FileStartTag))
                    id3Tag = mp3.GetTag(Id3TagFamily.FileStartTag);
                else if (mp3.HasTagOfFamily(Id3TagFamily.FileEndTag))
                    id3Tag = mp3.GetTag(Id3TagFamily.FileEndTag);
                else
                    id3Tag = new Id3v23Tag();
                item.Title = id3Tag.Title.Value;
                item.Author = id3Tag.Artists.Value;
                item.Album = id3Tag.Album.Value;
                item.Duration = mp3.Audio.Duration;
                item.PubDate = File.GetCreationTime(mp3File);
                item.Path = mp3File;
                item.MimeType = GetMimeType(mp3File);
                item.Size = (new FileInfo(mp3File)).Length;
                return item;
            }
        }

        private string GetMimeType(string mp3File)
        {
            string ext = Path.GetExtension(mp3File);
            switch (ext)
            {
                case ".mp3":
                    return "audio/mpeg";
                case ".m4a":
                    return "audio/x-m4a";
                case ".mp4":
                    return "video/mp4";
                case ".m4v":
                    return "video/x-m4v";
                case ".mov":
                    return "video/quicktime";
                case ".pdf":
                    return "application/pdf";
                case ".epub":
                    return "document/x-epub";
                default:
                    return "octet/stream";
            }
        }

        public string GetItemPath(string channelName, string fileName)
        {
            var channelFilePath = Path.Combine(_dataPath, string.Format("{0}.xml", channelName));
            var channelFile = XDocument.Load(channelFilePath);
            var podcastFolder = channelFile.XPathSelectElement("//podcastFolder");
            if (podcastFolder == null)
                throw new InvalidOperationException("XML file ust contain podcastFolder element with path to MP3 files");

            return Path.Combine(podcastFolder.Value, fileName);
        }
    }

}