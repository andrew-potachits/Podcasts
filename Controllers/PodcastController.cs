using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Podcasts.Controllers
{
    public class PodcastController : Controller
    {
        // GET ListEpisodes/channelName
        public ActionResult ListEpisodes(string name)
        {
            return new PodcastResult(name, 
                itemPath =>
                    {
                        var helper = new UrlHelper(Request.RequestContext);
                        return helper.Action("GetFile", "Podcast", new { name = name, file = Path.GetFileName(itemPath) }, Request.Url.Scheme);
                    });
        }

        public ActionResult GetFile(string name, string file)
        {
            var manager = new PodcastsManager(HostingEnvironment.MapPath(@"/App_Data"));

            var fs = new FileStream(manager.GetItemPath(name, file), FileMode.Open, FileAccess.Read);
            var data = new byte[fs.Length];
            fs.Read(data, 0, data.Length);
            return new FileContentResult(data, "audio/mpeg");
        }
    }
}
