using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUser.Controller;
using SkyView.Service;
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

        // 品牌故事
        public ActionResult AboutUs()
        {
            return View();
        }

        // 新聞資訊
        public ActionResult NewsList()
        {
            return View();
        }
        public ActionResult NewsData()
        {
            return View();
        }

        // 系列產品
        public ActionResult ProductList()
        {
            return View();
        }
        public ActionResult ProductData()
        {
            return View();
        }

        // 海外實績
        public ActionResult WitnessList()
        {
            return View();
        }
        public ActionResult WitnessData()
        {
            return View();
        }

        // 聯繫我們
        public ActionResult ContactUs()
        {
            return View();
        }
    }
}