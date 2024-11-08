using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;

namespace WebApplication1
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Enable CORS policy 
            EnableCorsAttribute cors = new EnableCorsAttribute("*", "*", "*");
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
            config.EnableCors(cors);
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings
            .Add(new RequestHeaderMapping("Accept",
                              "text/html",
                              StringComparison.InvariantCultureIgnoreCase,
                              true,
                              "application/json"));
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional, action = "Get" }
            );
        }
    }
}
