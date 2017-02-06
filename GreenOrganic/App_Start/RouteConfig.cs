using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SkyView
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
              name: "Manager",
              url: "_SysAdm",
              defaults: new { controller = "Home", action = "Login" }
              ).DataTokens["UseNamespaceFallback"] = false;

            ////網址加入多國語系寫法。
            //routes.MapRoute(
            //   name:"Default_Language",
            //   url:"{language}/{controller}/{action}/{id}",
            //   defaults:new {controller="Home",action="Index",language="",id=UrlParameter.Optional}    
            //);

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
