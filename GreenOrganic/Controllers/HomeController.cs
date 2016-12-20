using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUser.Controller;
using GreenOrganic.Service;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Specialized;
/*Json.NET相關的命名空間*/
using Newtonsoft.Json;

namespace GreenOrganic.Controllers
{
    public class HomeController : WebUserController
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }
        // 導向後台首頁(登入頁)
        public RedirectResult Login()
        {
            return Redirect("~/Manage");
        }

        // 360環景
        public ActionResult Overview()
        {
            return View();
        }

        // 關於本站
        public ActionResult AboutUs()
        {
            return View();
        }

        // 網站導覽
        public ActionResult Sitemap()
        {
            return View();
        }
    }
}