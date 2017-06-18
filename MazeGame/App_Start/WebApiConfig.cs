using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MazeGame
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "GetMoves",
                routeTemplate: "api/{controller}/{action}/{playerId}",
                defaults: new {controller = "Multiplayer" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
