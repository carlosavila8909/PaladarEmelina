using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Paladar_Emelina
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Admin",
                url: "Administracion",
                defaults: new { controller = "Administracion", action = "Configuracion" }
            );

            routes.MapRoute(
                name: "AdminHostal",
                url: "AdministracionHostal",
                defaults: new { controller = "AdministracionHostal", action = "Configuracion" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Paladar", action = "Mostrar", id = UrlParameter.Optional }
            );
        }
    }
}
