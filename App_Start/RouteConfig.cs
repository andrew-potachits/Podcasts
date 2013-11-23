using System.Web.Mvc;
using System.Web.Routing;

namespace Podcasts
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "GetFile",
                url: "{controller}/{action}/{name}/{file}",
                defaults: new { controller = "Podcast", action = "GetFile" }
            );
            
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{name}",
                defaults: new { controller = "Podcast", action = "ListEpisodes" }
            );




            PreApplicationStartCode.Start();
        }
    }
}