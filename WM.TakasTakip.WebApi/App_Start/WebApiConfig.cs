using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WM.EczaneNobet.WebApi.MessageHandlers;

namespace WM.TakasTakip.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.MessageHandlers.Add(new AuthenticationHandler());
            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            // config.Formatters.Remove(config.Formatters.XmlFormatter);
        }
    }
}
