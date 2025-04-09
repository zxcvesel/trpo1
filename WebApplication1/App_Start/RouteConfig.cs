using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace WebApplication1
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Добавляем ограничение для точного соответствия URL
            routes.MapRoute(
                name: "Profile",
                url: "Account/UserProfile",
                defaults: new { controller = "Account", action = "UserProfile" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") } // Ограничиваем GET-запросами
            );

            // Стандартный маршрут
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
