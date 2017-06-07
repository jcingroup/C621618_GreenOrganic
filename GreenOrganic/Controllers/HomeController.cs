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
            string lang = get_lang();
            string cview = get_viewname("Index",lang);

            //設定變數
            DataTable d_proj;
            DataTable d_country;
            DataTable d_area;
            DataTable d_proj_prod;
            DataTable d_prod;
            DataTable d_news;
            DataTable d_prod_cate;
            DataTable d_ad;

            //抓取資料
            d_proj = DB.Proj_List("", "", "Y", "", lang, "", "","Y");
            d_country = DB.Country_List(lang, "", "Y", "");
            d_area = DB.Area_List(lang, "", "", "Y", "");
            d_proj_prod = DB.Proj_Prod_List("", "", "Y", "", lang, "", "");
            d_prod = DB.Prod_List("", "", "Y", "", lang, "");
            d_news = DB.News_List("", "", "Y", "", "", "", lang, "Y");
            d_prod_cate = DB.Prod_Cate_List(lang);
            d_ad = DB.Video_List();

            ViewData["d_proj"] = d_proj;
            ViewData["d_country"] = d_country;
            ViewData["d_area"] = d_area;
            ViewData["d_proj_prod"] = d_proj_prod;
            ViewData["d_prod"] = d_prod;
            ViewData["d_ad"] = d_ad;
            ViewData["d_prod_cate"] = d_prod_cate;
            ViewData["d_news"] = d_news;

            return View(cview);
        }
        // 導向後台首頁(登入頁)
        public RedirectResult Login()
        {
            return Redirect("~/Manage");
        }

        // 品牌故事
        public ActionResult AboutUs()
        {
            string lang = get_lang();
            string cview = get_viewname("AboutUS", lang);
            DataTable d_com_info = DB.Com_List("AboutUs", lang);
            ViewData["d_com_info"] = d_com_info;
            return View(cview);
        }

        // 新聞資訊
        public ActionResult NewsList()
        {
            string lang = get_lang();
            string cview = get_viewname("NewsList", lang);
            DataTable d_news;
            d_news = DB.News_List("", "", "Y", "", "", "", lang);
            ViewData["d_news"] = d_news;
            return View(cview);
        }
        public ActionResult NewsData(string n_id = "")
        {
            string lang = get_lang();
            string cview = get_viewname("NewsData", lang);
            //string cview2 = get_viewname("NewsList", lang);
            DataTable d_news = DB.News_List(n_id,"","Y","","","",lang);
            
            if(d_news.Rows.Count > 0)
            {
                ViewData["d_news"] = d_news;
                return View(cview);
            }
            else
            {
                return RedirectToAction("NewsList");
            }

        }
        // 證照專區
        public ActionResult Certificate()
        {
            string lang = get_lang();
            string cview = get_viewname("Certificate", lang);
            DataTable d_com_info = DB.Com_List("Certificate", lang);
            ViewData["d_com_info"] = d_com_info;
            return View(cview);
        }
        // 影音專區
        public ActionResult Video()
        {
            string lang = get_lang();
            string cview = get_viewname("Video", lang);
            DataTable d_com_info = DB.Com_List("Video", lang);
            ViewData["d_com_info"] = d_com_info;
            return View(cview);
        }

        // 系列產品
        public ActionResult ProductList()
        {
            DataTable d_prod_cate;
            string lang = get_lang();
            string cview = get_viewname("ProductList", lang);

            d_prod_cate = DB.Prod_Cate_List(lang);
            ViewData["d_prod_cate"] = d_prod_cate;
            return View(cview);
        }
        public ActionResult ProductSublist(string cate_id = "")
        {
            DataTable d_prod;
            DataTable d_prod_img;
            string lang = get_lang();
            string cview = get_viewname("ProductSublist", lang);
            //string cview2 = get_viewname("ProductList", lang);

            d_prod = DB.Prod_List("","","Y","",lang,cate_id);
            d_prod_img = DB.Prod_Img_List("ALL");
            if(d_prod.Rows.Count > 0)
            {
                ViewData["d_prod"] = d_prod;
                ViewData["d_prod_img"] = d_prod_img;
                return View(cview);
            }
            else
            {
                return RedirectToAction("ProductList");
            }

        }
        public ActionResult ProductData(string prod_id = "")
        {
            DataTable d_prod;
            DataTable d_prod_img;
            DataTable d_proj_prod;
            string lang = get_lang();
            string cview = get_viewname("ProductData", lang);
            //string cview2 = get_viewname("ProductList", lang);

            d_prod = DB.Prod_List(prod_id, "", "Y", "", lang, "");
            d_prod_img = DB.Prod_Img_List(prod_id);
            d_proj_prod = DB.Proj_Prod_List("", "", "Y", "", lang, prod_id, "");
            if(d_prod.Rows.Count > 0)
            {
                ViewData["d_prod"] = d_prod;
                ViewData["d_prod_img"] = d_prod_img;
                ViewData["d_proj_prod"] = d_proj_prod;
                return View(cview);
            }
            else
            {
                return RedirectToAction("ProductList");
            }

        }

        // 海外實績
        public ActionResult WitnessList(string country = "", string area = "",string prod = "", string plant="",string txt_title_query = "")
        {
            //設定變數
            DataTable d_proj;
            DataTable d_country;
            DataTable d_area;
            DataTable d_proj_prod;
            DataTable d_prod;
            string lang = get_lang();
            string cview = get_viewname("WitnessList", lang);
            string proj_id = "";
            string plant_name = "";
            //抓取資料
            d_proj = DB.Proj_List("", "", "Y", txt_title_query, lang, country, area,"");
            d_country = DB.Country_List(lang, "", "Y", "");
            d_area = DB.Area_List(lang, "", "", "Y", "");

            if(d_proj.Rows.Count > 0)
            {
                proj_id = "";
                for(int i = 0; i < d_proj.Rows.Count; i++)
                {
                    plant_name = d_proj.Rows[i]["plant_name"].ToString();
                    if(plant_name.IndexOf(plant) >= 0)
                    {
                        if(proj_id.Length > 0)
                        {
                            proj_id = proj_id + ",";
                        }
                        proj_id = proj_id + d_proj.Rows[i]["proj_id"].ToString();
                    }
                }
            }

            d_proj_prod = DB.Proj_Prod_List("", "", "Y", "", lang, prod, proj_id);
            d_prod = DB.Prod_List("", "", "Y", "", lang, "");
            ViewData["d_proj"] = d_proj;
            ViewData["d_country"] = d_country;
            //ViewData["d_area"] = d_area;
            ViewData["d_proj_prod"] = d_proj_prod;
            ViewData["d_prod"] = d_prod;

            return View(cview);
        }

        public ActionResult WitnessData(string proj_id = "",string country_id ="",string area_id = "")
        {
            DataTable d_proj;
            DataTable d_proj_prod;
            DataTable d_prod;
            DataTable d_proj_img;

            string lang = get_lang();
            string cview = get_viewname("WitnessData", lang);
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

            return View(cview);
        }

        // 聯繫我們
        public ActionResult ContactUs()
        {
            string lang = get_lang();
            string cview = get_viewname("ContactUs", lang);
            return View(cview);
        }

        #region 國家資料取得 Country_Get
        public ActionResult Country_Get(string lang = "")
        {
            string str_return = "";
            DataTable Country;
            lang = get_lang();
            Country = DB.Country_List(lang);
            str_return = JsonConvert.SerializeObject(Country, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 省份取得 Area_Get
        public ActionResult Area_Get(string lang = "",string country = "")
        {
            string str_return = "";
            DataTable Area;
            lang = get_lang();
            Area = DB.Area_List(lang, country);
            str_return = JsonConvert.SerializeObject(Area, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 專案資料取得 Proj_Get
        public ActionResult Proj_Get(string lang = "")
        {
            string str_return = "";
            DataTable Proj;
            lang = get_lang();
            Proj = DB.Proj_List("", "", "", "", lang, "", "");
            str_return = JsonConvert.SerializeObject(Proj, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 產品資料取得 Prod_Get
        public ActionResult Prod_Get(string lang = "")
        {
            string str_return = "";
            DataTable Prod;
            lang = get_lang();
            Prod = DB.Prod_List("", "", "", "", lang, "");
            str_return = JsonConvert.SerializeObject(Prod, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 海外實績-產品取得 Proj_Prod_Get
        public ActionResult Proj_Prod_Get(string country = "",string area = "",string is_index = "")
        {
            string str_return = "";

            DataTable Proj_Prod;
            DataTable Prod;
            string lang = "";
            string c_prod = "";
            lang = get_lang();
            Proj_Prod = DB.Proj_Prod_List("", "", "Y", "", lang, "", "");
            if(Proj_Prod.Rows.Count > 0)
            {
                c_prod = "";
                for(int i = 0; i < Proj_Prod.Rows.Count; i++)
                {
                    if (c_prod.Trim().Length > 0)
                    {
                        c_prod = c_prod + ",";
                    }

                    c_prod = c_prod + Proj_Prod.Rows[i]["prod_id"].ToString();
                }
            }

            Prod = DB.Prod_List(c_prod, "", "Y", "", lang, "");

            str_return = JsonConvert.SerializeObject(Prod, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 海外實績-植物取得 Proj_Plant_Get
        public ActionResult Proj_Plant_Get(string country = "", string area = "")
        {
            string str_return = "";

            DataTable Proj;
            DataTable Plant = new DataTable();
            string c_plant = "";
            string c_chk = "";

            //kk.Columns.Add("c_dates", System.Type.GetType("System.String")); //時間
            //kk.Columns.Add("c_memo", System.Type.GetType("System.String")); //訊息
            Plant.Columns.Add("plant_name",System.Type.GetType("System.String")); //植物名稱
            string lang = "";
            string c_prod = "";
            lang = get_lang();
            Proj = DB.Proj_List("","","Y","",lang,country,area,"");
            if (Proj.Rows.Count > 0)
            {
                c_plant = "";
                for (int i = 0; i < Proj.Rows.Count; i++)
                {
                    if (c_plant.Trim().Length > 0)
                    {
                        c_plant = c_plant + ",";
                    }

                    c_plant = c_plant + Proj.Rows[i]["plant_name"].ToString();
                }
            }

            string[] Array_plant = c_plant.Split(',');
            for(int i = 0; i < Array_plant.Length; i++)
            {
                c_chk = "N";
                //檢查是否有加入資料
                if (Plant.Rows.Count > 0)
                {
                    for(int j = 0; j < Plant.Rows.Count; j++)
                    {
                        if(Plant.Rows[j]["plant_name"].ToString() == Array_plant[i])
                        {
                            c_chk = "Y";
                            break;
                        }
                    }
                }

                if(c_chk == "N")
                {
                    DataRow new_row = Plant.NewRow();
                    new_row[0] = Array_plant[i];
                    Plant.Rows.Add(new_row);
                }
            }

            str_return = JsonConvert.SerializeObject(Plant, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 變更語系
        public ActionResult Lang_Change(string lang = "")
        {
            string str_return = "";

            if(lang =="")
            {
                lang = "cn";
            }

            Session["lang"] = lang;

            return Content(str_return);
        }
        #endregion

        #region 語系取得
        public string get_lang()
        {
            string clang = "";
            //設定語系
            if (Convert.ToString(Session["lang"]) == "")
            {
                Session["lang"] = "cn";
                
            }

            clang = Convert.ToString(Session["lang"]);

            return clang;
        }
        #endregion

        #region 頁面取得
        public string get_viewname(string view_form = "",string lang = "")
        {
            string cview = "";
            switch (lang)
            {
                case "cn":
                    cview = view_form + "";
                    break;
                case "zh-tw":
                    cview = view_form + ".zh-TW";
                    break;
                case "en":
                    cview = view_form + ".en-US";
                    break;
            }
            return cview;
        }
        #endregion
    }
}