using System.Web.Http;
using System.Web.Mvc;
using Podcasts.Data;

namespace Podcasts
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{name}",
                defaults: new { name = UrlParameter.Optional }
            );

            // Remove the JSON formatter
            config.Formatters.Remove(config.Formatters.JsonFormatter);
            config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
