using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUser.Controller;
using SkyView.Service;
using Lib.Service;
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
        OverlookDBService OverlookDB = new OverlookDBService();
        DBService DB = new DBService();

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
            string lang = "cn";
            DataTable d_news;
            d_news = DB.News_List("", "", "Y", "", "", "", "cn");
            ViewData["d_news"] = d_news;
            return View();
        }
        public ActionResult NewsData()
        {
            return View();
        }

        // 系列產品
        public ActionResult ProductList()
        {
            DataTable d_prod_cate;
            string lang = "cn";
            d_prod_cate = DB.Prod_Cate_List(lang);
            ViewData["d_prod_cate"] = d_prod_cate;
            return View();
        }
        public ActionResult ProductSublist(string cate_id = "")
        {
            DataTable d_prod;
            DataTable d_prod_img;
            string lang = "cn";
            d_prod = DB.Prod_List("","","Y","",lang,cate_id);
            d_prod_img = DB.Prod_Img_List("ALL");
            ViewData["d_prod"] = d_prod;
            ViewData["d_prod_img"] = d_prod_img;
            return View();
        }
        public ActionResult ProductData(string prod_id = "")
        {
            DataTable d_prod;
            DataTable d_prod_img;
            DataTable d_proj_prod;
            string lang = "cn";
            d_prod = DB.Prod_List(prod_id, "", "Y", "", lang, "");
            d_prod_img = DB.Prod_Img_List(prod_id);
            d_proj_prod = DB.Proj_Prod_List("", "", "Y", "", lang, prod_id, "");
            ViewData["d_prod"] = d_prod;
            ViewData["d_prod_img"] = d_prod_img;
            ViewData["d_proj_prod"] = d_proj_prod;
            return View();
        }

        // 海外實績
        public ActionResult WitnessList()
        {
            return View();
        }

        public ActionResult WitnessData(string proj_id = "",string country_id ="",string area_id = "")
        {
            DataTable d_proj;
            DataTable d_proj_prod;
            DataTable d_prod;
            DataTable d_proj_img;

            string lang = "cn";
            string prod_id = "";

            d_proj = DB.Proj_List(proj_id,"","Y","",lang,country_id,area_id);
            d_proj_img = DB.Proj_Img_List(proj_id);
            d_proj_prod = DB.Proj_Prod_List("", "", "Y", "", lang, "", proj_id);

            if(d_proj_prod.Rows.Count > 0)
            {
                prod_id = d_proj_prod.Rows[0]["prod_id"].ToString();
            }

            d_prod = DB.Prod_List(prod_id);

            ViewData["d_proj"] = d_proj;
            ViewData["d_proj_img"] = d_proj_img;
            ViewData["d_prod"] = d_prod;

            return View();
        }

        // 聯繫我們
        public ActionResult ContactUs()
        {
            return View();
        }
    }
}