using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OhayooWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces:new string[] { "OhayooWeb.Controllers" }
            );
            routes.MapRoute(
                name: "rakuten",
                url: "rakuten/{controller}/{action}/{id}/{name}/{cateid}/{catename}",
                defaults: new {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    name = UrlParameter.Optional,
                    cateid = UrlParameter.Optional,
                    catename = UrlParameter.Optional
                }, namespaces: new string[] { "OhayooWeb.Areas.rakuten.Controllers" }
            );
            routes.MapRoute(
                name: "yahooauction",
                url: "yahooauction/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }, namespaces: new string[] { "OhayooWeb.Areas.yahooauction.Controllers" }
            );
            routes.MapRoute(
                name: "yahooshopping",
                url: "yahooshopping/{controller}/{action}/{id}/{name}/{cateid}/{catename}",
                defaults: new {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    name = UrlParameter.Optional,
                    cateid = UrlParameter.Optional,
                    catename = UrlParameter.Optional
                }, namespaces: new string[] { "OhayooWeb.Areas.yahooshopping.Controllers" }
            );
            routes.MapRoute(
                name: "zozo",
                url: "zozo/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }, namespaces: new string[] { "OhayooWeb.Areas.zozo.Controllers" }
            );
        }
    }
}
