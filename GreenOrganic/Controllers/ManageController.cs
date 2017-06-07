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
    public class ManageController : WebUserController
    {
        OverlookDBService OverlookDB = new OverlookDBService();
        DBService DB = new DBService();

        // 後台首頁-導向Login
        public ActionResult Index()
        {
            if(Convert.ToString(Session["IsLogined"]) == "Y")
            {
                return RedirectToAction("ProductList"); 
            }
            else
            {
                return View("Login");
            }

        }

        // 後台登入
        public ActionResult Login()
        {
            return View();
        }

        //登入檢查
        [HttpPost]
        public ActionResult Login_Chk(string account, string pwd,string ValidCode)
        {
            DataTable user_info;
            string cmsg = "";
            try
            {
                if (string.IsNullOrWhiteSpace(account))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入帳號";
                }

                if (string.IsNullOrWhiteSpace(pwd))
                {
                    if (cmsg.Trim().Length > 0)
                    {
                        cmsg = cmsg + "\n";
                    }
                    cmsg = cmsg + "請輸入密碼";
                }

                if (cmsg.Trim().Length == 0)
                {
                    //比對驗證碼
                    if (ValidCode != Session["ValidateCode"].ToString())
                    {
                        if (cmsg.Trim().Length > 0)
                        {
                            cmsg = cmsg + "\n";
                        }
                        cmsg = cmsg + "驗證碼不正確";
                    }
                    else
                    {
                        //抓取user資料
                        user_info = DB.User_Info(account);
                        //驗證使用者有無資料
                        if (user_info.Rows.Count == 0)
                        {
                            if (cmsg.Trim().Length > 0)
                            {
                                cmsg = cmsg + "\n";
                            }
                            cmsg = cmsg + "無此帳號，請再確認。";
                        }
                        else
                        {
                            if (user_info.Rows[0]["status"].ToString() == "N")
                            {
                                if (cmsg.Trim().Length > 0)
                                {
                                    cmsg = cmsg + "\n";
                                }
                                cmsg = cmsg + "此帳號停用，請再確認。";
                            }
                            else
                            {
                                if (pwd == user_info.Rows[0]["pwd"].ToString())
                                {
                                    //輸入值
                                    Session["IsLogined"] = "Y";
                                    Session["Account"] = account;
                                    Session["Account_Name"] = user_info.Rows[0]["account_name"].ToString();
                                    Session["Rank"] = user_info.Rows[0]["rank"].ToString();
                                }
                                else
                                {
                                    if (cmsg.Trim().Length > 0)
                                    {
                                        cmsg = cmsg + "\n";
                                    }
                                    cmsg = cmsg + "密碼錯誤，請重新輸入。";
                                }
                            }
                        }
                    }
                }

                if (cmsg.Trim().Length > 0)
                {
                    TempData["message"] = cmsg;
                    return View("Login");
                }
                else
                {
                    return RedirectToAction("ProductList");
                }

            }
            catch
            {
                return View("Login");
            }
        }

        //後台登出
        public ActionResult Logout()
        {
            //清除 Session();
            Session.Remove("IsLogined");
            Session.Remove("Account");
            Session.Remove("Account_Name");
            Session.Remove("Rank");

            return Redirect(Url.Content("~/Manage"));
        }

        #region 最新消息

        #region 消息陳列 NewList
        public ActionResult NewsList(string txt_title_query = "", int page = 1,string txt_sort = "",string txt_a_d = "",string txt_start_date = "",string txt_end_date = "",string txt_lang = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if(txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取消息資料
            dt = DB.News_List("",c_sort,"",txt_title_query,txt_start_date,txt_end_date,txt_lang);
            d_lang = DB.Lang_List("");

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_start_date"] = txt_start_date;
            ViewData["txt_end_date"] = txt_end_date;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;

            return View();
        }
        #endregion

        #region 最新消息新增 News_Add
        public ActionResult News_Add()
        {
            DataTable d_lang = DB.Lang_List();

            ViewData["d_lang"] = d_lang;
            ViewData["action_sty"] = "add";
            
            return View("NewsData");
        }
        #endregion

        #region 最新消息修改 News_Edit
        public ActionResult News_Edit(string n_id = "")
        {
            DataTable d_news = DB.News_List(n_id);
            DataTable d_lang = DB.Lang_List();
           
            ViewData["d_lang"] = d_lang;
            ViewData["d_news"] = d_news;
            ViewData["action_sty"] = "edit";

            return View("NewsData");
        }
        #endregion

        #region 最新消息刪除 News_Del
        public ActionResult News_Del(string n_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            DB.News_Del(n_id);
            return RedirectToAction("NewsList");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult News_Save(string action_sty, string n_id, string n_title, string n_date, string lang, string n_desc, string show,string hot,string sort)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    DB.News_Insert(n_title,n_date,n_desc,lang,show,hot,sort);
                    break;
                case "edit":
                    DB.News_Update(n_id,n_title, n_date, n_desc, lang, show, hot, sort);
                    break;
            }

            return RedirectToAction("NewsList");
        }

        #endregion

        #endregion

        // 變更密碼
        public ActionResult ChangePW()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePW(string now_pwd,string new_pwd, string chk_new_pwd)
        {
            string Account = Convert.ToString(Session["Account"]);
            string cmsg = "";
            DataTable user_info;
            //抓取資料
            user_info = DB.User_Info(Account);
            //檢查登入密碼是否正確
            if(now_pwd == user_info.Rows[0]["pwd"].ToString())
            {
                DB.User_Update(Account, new_pwd);
            }
            else
            {
                if (cmsg.Trim().Length > 0)
                {
                    cmsg = cmsg + "\n";
                }
                cmsg = cmsg + "密碼錯誤，請重新輸入。";
            }

            if(cmsg.Trim().Length == 0)
            {
                cmsg = "Y";
            }

            TempData["message"] = cmsg;
            return View();

        }

        public ActionResult Upload(string img_no,string img_sta, string img_cate)
        {
            DataTable img_file;
            DataTable chk_file;

            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            string imgPath = "";
            string filename = "";
            string file_name = "";
            string file_path = "../Images/";
            string str_return = "";
            string[] files;
            string chk_sty = "";
            string pre_filename = "";
            int files_count = 0;
            string cmsg = "";

            if (hfc.Count > 0)
            {
                    file_name = hfc[0].FileName;
                    files = file_name.Split('\\');
                    files_count = files.Count();
                    filename = files[files_count - 1];
                    
                    imgPath = file_path + img_no + "_" + img_sta + "_" + filename;
                    string PhysicalPath = Server.MapPath(imgPath);
                    hfc[0].SaveAs(PhysicalPath);
            }
            switch(img_cate)
            {
                case "Prod":
                    chk_file = DB.Prod_Img_List(img_no);
                    break;
                case "Proj":
                    chk_file = DB.Proj_Img_List(img_no);
                    break;
                case "Ad":
                    chk_file = DB.Ad_Img_List(img_no);
                    break;
                default:
                    chk_file = OverlookDB.Img_List(img_no, img_sta);
                    break;
            }
            

            chk_sty = "add";
            if(img_sta == "S")
            {
                if (chk_file.Rows.Count > 0)
                {
                    pre_filename = file_path + chk_file.Rows[0]["img_file"].ToString();

                    chk_sty = "update";
                }
            }

            switch(chk_sty)
            {
                case "add": //加入到資料庫
                    switch(img_cate)
                    {
                        case "Prod":
                            DB.Prod_Img_Insert(img_no, img_no + "_" + img_sta + "_" + filename);
                            break;
                        case "Proj":
                            DB.Proj_Img_Insert(img_no, img_no + "_" + img_sta + "_" + filename);
                            break;
                        case "Ad":
                            if(chk_file.Rows.Count < 3)
                            {
                                DB.Ad_Img_Insert(img_no, img_no + "_" + img_sta + "_" + filename);
                            }
                            break;
                        default:
                            //OverlookDB.Img_Insert(img_no, img_no + "_" + img_sta + "_" + filename, img_sta);
                            break;
                    }
                    
                    break;
                case "update":
                    switch (img_cate)
                    {
                        case "Prod":
                            DB.Prod_Img_Update(img_no, img_no + "_" + img_sta + "_" + filename);
                            break;
                        case "Proj":
                            DB.Proj_Img_Update(img_no, img_no + "_" + img_sta + "_" + filename);
                            break;
                        case "Ad":
                            DB.Ad_Img_Update(img_no, img_no + "_" + img_sta + "_" + filename);
                            break;
                        default:
                            //OverlookDB.Img_Update(chk_file.Rows[0]["_SEQ_ID"].ToString(), img_no + "_" + img_sta + "_" + filename);
                            break;
                    }

                    //刪除原本檔案
                    if (pre_filename == "")
                    {
                        string Pre_Path = Server.MapPath(pre_filename);

                        // Delete a file by using File class static method...
                        if (System.IO.File.Exists(Pre_Path))
                        {
                            // Use a try block to catch IOExceptions, to
                            // handle the case of the file already being
                            // opened by another process.
                            try
                            {
                                System.IO.File.Delete(Pre_Path);
                            }
                            catch (System.IO.IOException e)
                            {
                                str_return = str_return + e.Message;
                            }
                        }
                    }


                    break;
            }

            //抓取資料
            switch(img_cate)
            {
                case "Prod":
                    img_file = DB.Prod_Img_List(img_no);
                    break;
                case "Proj":
                    img_file = DB.Proj_Img_List(img_no);
                    break;
                case "Ad":
                    img_file = DB.Ad_Img_List(img_no);
                    break;
                default:
                    img_file = OverlookDB.Img_List(img_no, img_sta);
                    break;
            }
            
            str_return = JsonConvert.SerializeObject(img_file, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
         

        }

        public ActionResult Img_Del(string img_id,string img_sta,string img_no, string img_cate)
        {
            string str_return = "";
            DataTable img_file;
            DataTable chk_file;
            string filename = "";
            string file_path = "../Images/";
            string imgPath = "";

            switch (img_cate)
            {
                case "Prod":
                    chk_file = DB.Prod_Img_List(img_no);
                    break;
                case "Proj":
                    chk_file = DB.Proj_Img_List(img_no);
                    break;
                case "Ad":
                    chk_file = DB.Ad_Img_List(img_no);
                    break;
                default:
                    chk_file = OverlookDB.Img_List(img_no, img_sta);
                    break;
            }
            
            filename = "";

            if (chk_file.Rows.Count > 0)
            {
                for(int i = 0; i< chk_file.Rows.Count; i++)
                {
                    if(img_cate == "Ad")
                    {
                        if (img_id == chk_file.Rows[i]["ad_id"].ToString())
                        {
                            filename = chk_file.Rows[i]["ad_img"].ToString();
                            break;
                        }
                    }
                    else
                    {
                        if (img_id == chk_file.Rows[i]["img_id"].ToString())
                        {
                            filename = chk_file.Rows[i]["img_file"].ToString();
                            break;
                        }
                    }

                }
            }

            imgPath = file_path + filename;

            string PhysicalPath = Server.MapPath(imgPath);

            // Delete a file by using File class static method...
            if (System.IO.File.Exists(PhysicalPath))
            {
                // Use a try block to catch IOExceptions, to
                // handle the case of the file already being
                // opened by another process.
                try
                {
                    System.IO.File.Delete(PhysicalPath);
                }
                catch (System.IO.IOException e)
                {
                    str_return = str_return + e.Message;
                }
            }

            switch (img_cate)
            {
                case "Prod":
                    DB.Prod_Img_Delete(img_id);
                    break;
                case "Proj":
                    DB.Proj_Img_Delete(img_id);
                    break;
                case "Ad":
                    DB.Ad_Img_Delete(img_id);
                    break;
                default:
                    OverlookDB.Img_Delete(img_id);
                    break;
            }

            //抓取資料
            switch (img_cate)
            {
                case "Prod":
                    img_file = DB.Prod_Img_List(img_no);
                    break;
                case "Proj":
                    img_file = DB.Proj_Img_List(img_no);
                    break;
                case "Ad":
                    img_file = DB.Ad_Img_List(img_no);
                    break;
                default:
                    img_file = OverlookDB.Img_List(img_no, img_sta);
                    break;
            }
            str_return = JsonConvert.SerializeObject(img_file, Newtonsoft.Json.Formatting.Indented);
            return Content(str_return);
        }

        /// <summary>
        /// ckeditor上傳圖片
        /// </summary>
        /// <param name="upload">預設參數叫upload</param>
        /// <param name="CKEditorFuncNum"></param>
        /// <param name="CKEditor"></param>
        /// <param name="langCode"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadPicture(HttpPostedFileBase upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            string result = "";
            if (upload != null && upload.ContentLength > 0)
            {
                //儲存圖片至Server
                upload.SaveAs(Server.MapPath("~/Images/" + upload.FileName));


                var imageUrl = Url.Content("~/Images/" + upload.FileName);

                var vMessage = string.Empty;

                result = @"<html><body><script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + imageUrl + "\", \"" + vMessage + "\");</script></body></html>";

            }

            return Content(result);
        }

        //設置將生成的驗證碼存入Session，並輸出驗證碼圖片
        [AllowAnonymous]
        public ActionResult GetValidateCode()
        {
            ValidateCode vCode = new ValidateCode();
            string code = vCode.CreateValidateCode(5);
            Session["ValidateCode"] = code;
            byte[] bytes = vCode.CreateValidateGraphic(code);
            return File(bytes, @"image/jpeg");
        }

        #region 海外實績

        public ActionResult ResultsKind1(string txt_country_query="", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "",string action_sty = "",string country_id = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            switch(action_sty)
            {
                case "del":
                    DB.Country_Del(country_id);
                    action_sty = "";
                    break;
            }

            //抓取產品資料
            dt = DB.Country_List(txt_lang,c_sort,"",txt_country_query);
            d_lang = DB.Lang_List("");

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["txt_country_query"] = txt_country_query;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["action_sty"] = action_sty;
            ViewData["country_id"] = country_id;

            return View();
        }

        public ActionResult ResultsKind1_Save(string txt_country_query, int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "", string action_sty = "", string country_id = "",string country_name = "",string lang="", string show = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            switch(action_sty)
            {
                case "add":
                    DB.Country_Add(country_name,lang,show);
                    break;
                case "edit":
                    DB.Country_Update(country_id, country_name, lang, show);
                    break;
            }


            //抓取海外實績-國家資料
            dt = DB.Country_List(txt_lang);
            d_lang = DB.Lang_List("");

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["txt_country_query"] = txt_country_query;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["action_sty"] = "";
            ViewData["txt_country_id"] = "";

            return View("ResultsKind1");
        }

        public ActionResult ResultsKind2(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "", string txt_country = "",string action_sty = "", string area_id = "")
        {

            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_country;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            switch (action_sty)
            {
                case "del":
                    DB.Area_Del(area_id);
                    action_sty = "";
                    break;
            }

            //抓取產品資料
            dt = DB.Area_List(txt_lang,txt_country, c_sort, "", txt_title_query);
            d_lang = DB.Lang_List("");
            d_country = DB.Country_List(txt_lang);
           
            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_country"] = d_country;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_country"] = txt_country;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["action_sty"] = action_sty;
            ViewData["area_id"] = area_id;

            return View();
        }

        public ActionResult ResultsKind2_Save(string txt_title_query, int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "", string txt_country = "", string action_sty = "", string area_id = "", string area_name = "", string lang = "", string country = "",string show = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_country;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            switch (action_sty)
            {
                case "add":
                    DB.Area_Add(area_name, lang, country, show);
                    break;
                case "edit":
                    DB.Area_Update(area_id, area_name, lang, country, show);
                    break;
            }


            //抓取海外實績-國家資料
            dt = DB.Area_List(txt_lang, txt_country, c_sort, "", txt_title_query);
            d_lang = DB.Lang_List("");
            d_country = DB.Country_List(txt_lang);

            //抓取產品資料
            dt = DB.Area_List(txt_lang, txt_country, c_sort, "", txt_title_query);
            d_lang = DB.Lang_List("");
            d_country = DB.Country_List(txt_lang);

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_country"] = d_country;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_country"] = txt_country;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["action_sty"] = "";
            ViewData["area_id"] = area_id;
            return View("ResultsKind2");
        }

        public ActionResult ResultsPair(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "", string txt_prod = "", string action_sty = "", string idxno = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_proj;
            DataTable d_prod;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            switch (action_sty)
            {
                case "del":
                    DB.Proj_Prod_Del(idxno);
                    action_sty = "";
                    break;
            }

            //抓取專案-產品資料
            dt = DB.Proj_Prod_List(idxno,c_sort,"",txt_title_query,txt_lang, txt_prod, "");
            d_lang = DB.Lang_List("");
            d_proj = DB.Proj_List("","","","",txt_lang,"","");
            d_prod = DB.Prod_List("", "", "", "", txt_lang, "");

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_lang"] = d_lang;
            ViewData["d_proj"] = d_proj;
            ViewData["d_prod"] = d_prod;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_prod"] = txt_prod;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;
            ViewData["action_sty"] = action_sty;
            ViewData["idxno"] = idxno;

            return View();
        }

        public ActionResult ResultsPair_Save(string idxno = "", string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "", string txt_prod = "", string action_sty = "", string proj = "", string lang = "", string s_prod = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_lang;
            DataTable d_proj;
            DataTable d_prod;

            
            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            switch (action_sty)
            {
                case "add":
                    DB.Proj_Prod_Add(proj, s_prod);

                    dt = DB.Proj_Prod_List("", "", "", "", lang, "", "", "");
                    d_lang = DB.Lang_List("");
                    d_proj = DB.Proj_List("", "", "", "", lang, "", "");
                    d_prod = DB.Prod_List("", "", "", "", lang, "");
                    //抓取專案-產品資料
                    dt = DB.Proj_Prod_List("", "", "", "", lang, "", "", "");
                    d_lang = DB.Lang_List("");
                    d_proj = DB.Proj_List("", "", "", "", lang, "", "");
                    d_prod = DB.Prod_List("", "", "", "", lang, "");

                    //設定傳值
                    ViewData["page"] = page;
                    ViewData["dt"] = dt;
                    ViewData["d_lang"] = d_lang;
                    ViewData["d_proj"] = d_proj;
                    ViewData["d_prod"] = d_prod;
                    ViewData["txt_title_query"] = "";
                    ViewData["txt_lang"] = lang;
                    ViewData["txt_prod"] = "";
                    ViewData["txt_sort"] = "";
                    ViewData["txt_a_d"] = "";
                    ViewData["action_sty"] = "";
                    ViewData["idxno"] = "";
                    break;
                case "edit":
                    DB.Proj_Prod_Update(idxno, proj, s_prod);

                    dt = DB.Proj_Prod_List("", c_sort, "", txt_title_query, txt_lang, txt_prod, "", "");
                    d_lang = DB.Lang_List("");
                    d_proj = DB.Proj_List("", "", "", "", txt_lang, "", "");
                    d_prod = DB.Prod_List("", "", "", "", txt_lang, "");
                    //抓取專案-產品資料
                    dt = DB.Proj_Prod_List("", c_sort, "", txt_title_query, txt_lang, txt_prod, "", "");
                    d_lang = DB.Lang_List("");
                    d_proj = DB.Proj_List("", "", "", "", txt_lang, "", "");
                    d_prod = DB.Prod_List("", "", "", "", txt_lang, "");

                    //設定傳值
                    ViewData["page"] = page;
                    ViewData["dt"] = dt;
                    ViewData["d_lang"] = d_lang;
                    ViewData["d_proj"] = d_proj;
                    ViewData["d_prod"] = d_prod;
                    ViewData["txt_title_query"] = txt_title_query;
                    ViewData["txt_lang"] = txt_lang;
                    ViewData["txt_prod"] = txt_prod;
                    ViewData["txt_sort"] = txt_sort;
                    ViewData["txt_a_d"] = txt_a_d;
                    ViewData["action_sty"] = "";
                    ViewData["idxno"] = idxno;
                    break;
            }

            return View("ResultsPair");
        }

        #region 海外實績 專案 ResultList
        public ActionResult ResultsList(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "", string txt_country = "",string txt_area = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_country;
            DataTable d_lang;
            DataTable d_area;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取專案資料
            dt = DB.Proj_List("", c_sort, "", txt_title_query, txt_lang, txt_country,txt_area);
            d_lang = DB.Lang_List("");
            d_country = DB.Country_List(txt_lang);
            d_area = DB.Area_List(txt_lang, txt_country);

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_country"] = d_country;
            ViewData["d_lang"] = d_lang;
            ViewData["d_area"] = d_area;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_country"] = txt_country;
            ViewData["txt_area"] = txt_area;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;

            return View();
        }
        #endregion

        #region 專案資料新增 Proj_Add
        public ActionResult Proj_Add()
        {
            DataTable d_lang;
            DataTable d_country;
            DataTable d_area;
            DataTable d_img;
            string str_lang = "";
            string str_country = "";


            d_lang = DB.Lang_List();
            if(d_lang.Rows.Count > 0)
            {
                str_lang = d_lang.Rows[0]["lang_id"].ToString();
            }

            d_country = DB.Country_List(str_lang);
            if(d_country.Rows.Count > 0)
            {
                str_country = d_country.Rows[0]["country_id"].ToString();
            }

            d_area = DB.Area_List(str_lang, str_country);
            d_img = DB.Prod_Img_List("");

            ViewData["d_lang"] = d_lang;
            ViewData["d_country"] = d_country;
            ViewData["d_area"] = d_area;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("ResultsData");
        }
        #endregion

        #region 專案資料修改 Proj_Edit
        public ActionResult Proj_Edit(string proj_id = "")
        {
            DataTable d_proj = DB.Proj_List(proj_id);
            DataTable d_lang = DB.Lang_List();
            DataTable d_country = DB.Country_List(d_proj.Rows[0]["lang"].ToString());
            DataTable d_area = DB.Area_List(d_proj.Rows[0]["lang"].ToString(), d_proj.Rows[0]["country_id"].ToString());
            DataTable d_img = DB.Proj_Img_List(proj_id);

            ViewData["d_lang"] = d_lang;
            ViewData["d_proj"] = d_proj;
            ViewData["d_country"] = d_country;
            ViewData["d_area"] = d_area;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("ResultsData");
        }
        #endregion

        #region 專案資料儲存 Proj_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Proj_Save(string action_sty, string proj_id, string proj_name, string plant_name, string proj_short_desc, string proj_desc, string lang,string country, string area, string show, string sort, string img_no, string hot)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    DB.Proj_Insert(proj_name, plant_name, proj_short_desc, proj_desc, lang, country, area, show, sort, img_no,hot);
                    break;
                case "edit":
                    DB.Proj_Update(proj_id, proj_name, plant_name, proj_short_desc, proj_desc, country, area, lang, show, sort,hot);
                    break;
            }

            return RedirectToAction("ResultsList");
        }
        #endregion

        #region 專案資料刪除 Proj_Del
        public ActionResult Proj_Del(string proj_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            DB.Proj_Del(proj_id);
            return RedirectToAction("ResultsList");
        }
        #endregion

        #region 國家資料取得 Country_Get
        public ActionResult Country_Get(string lang)
        {
            string str_return = "";
            DataTable Country;
            Country = DB.Country_List(lang);
            str_return = JsonConvert.SerializeObject(Country, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 省份取得 Area_Get
        public ActionResult Area_Get(string lang,string country)
        {
            string str_return = "";
            DataTable Area;
            Area = DB.Area_List(lang,country);
            str_return = JsonConvert.SerializeObject(Area, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 專案資料取得 Proj_Get
        public ActionResult Proj_Get(string lang)
        {
            string str_return = "";
            DataTable Proj;
            Proj = DB.Proj_List("", "", "", "", lang, "", "");
            str_return = JsonConvert.SerializeObject(Proj, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #region 產品資料取得 Prod_Get
        public ActionResult Prod_Get(string lang)
        {
            string str_return = "";
            DataTable Prod;
            Prod = DB.Prod_List("", "", "", "", lang, "");
            str_return = JsonConvert.SerializeObject(Prod, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #endregion

        #region 產品

        #region 產品陳列 ProductList        
        public ActionResult ProductList(string txt_title_query = "", int page = 1, string txt_sort = "", string txt_a_d = "", string txt_lang = "",string txt_cate = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;
            DataTable d_cate;
            DataTable d_lang;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
            }
            if (txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取產品資料
            dt = DB.Prod_List("", c_sort, "", txt_title_query, txt_lang, txt_cate);
            d_lang = DB.Lang_List("");
            d_cate = DB.Prod_Cate_List(txt_lang);

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["d_cate"] = d_cate;
            ViewData["d_lang"] = d_lang;
            ViewData["txt_title_query"] = txt_title_query;
            ViewData["txt_lang"] = txt_lang;
            ViewData["txt_cate_id"] = txt_cate;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;

            return View();
        }
        #endregion

        #region 產品資料新增 Prod_Add
        // 產品資料 新增
        public ActionResult Prod_Add()
        {
            DataTable d_lang = DB.Lang_List();
            DataTable d_cate = DB.Prod_Cate_List(d_lang.Rows[0]["lang_id"].ToString());
            DataTable d_img = DB.Prod_Img_List("");
            ViewData["d_lang"] = d_lang;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "add";

            return View("ProductData");
        }
        #endregion

        #region 產品資料修改 Prod_Edit
        public ActionResult Prod_Edit(string prod_id = "")
        {
            DataTable d_prod = DB.Prod_List(prod_id);
            DataTable d_lang = DB.Lang_List();
            DataTable d_cate = DB.Prod_Cate_List(d_prod.Rows[0]["lang"].ToString());
            DataTable d_img = DB.Prod_Img_List(prod_id);

            ViewData["d_lang"] = d_lang;
            ViewData["d_prod"] = d_prod;
            ViewData["d_cate"] = d_cate;
            ViewData["d_img"] = d_img;
            ViewData["action_sty"] = "edit";

            return View("ProductData");
        }
        #endregion

        #region 產品資料刪除 Prod_Del
        public ActionResult Prod_Del(string prod_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            DB.Prod_Del(prod_id);
            return RedirectToAction("ProductList");
        }
        #endregion

        #region 產品資料儲存 Prod_Save
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Prod_Save(string action_sty, string prod_id,string cate, string prod_name, string manure_no, string manure_info, string manure_ingredients, string manure_trait, string prod_desc, string lang, string show, string sort,string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    DB.Prod_Insert(cate, prod_name, manure_no, manure_info, manure_ingredients, manure_trait, prod_desc, lang, show, sort,img_no);
                    break;
                case "edit":
                    DB.Prod_Update(prod_id ,cate, prod_name, manure_no, manure_info, manure_ingredients, manure_trait, prod_desc, lang, show, sort);
                    break;
            }

            return RedirectToAction("ProductList");
        }
        #endregion

        #region 產品類別取得 Prod_Cate_Get
        public ActionResult Prod_Cate_Get(string lang)
        {
            string str_return = "";
            DataTable Prod_Cate;
            Prod_Cate = DB.Prod_Cate_List(lang);
            str_return = JsonConvert.SerializeObject(Prod_Cate, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
        }
        #endregion

        #endregion

        // 首頁影片
        public ActionResult IndexVideo()
        {
            DataTable d_video = DB.Ad_Img_List();
            ViewData["d_video"] = d_video;
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IndexVideo_Update(string mv = "")
        {
            DB.Video_Update(mv);

            return RedirectToAction("IndexVideo");
        }

        //public ActionResult Mv_Upload(string img_no, string img_sta, string img_cate)
        public ActionResult Mv_Upload()
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("rec_time", System.Type.GetType("System.String")); //Empid
            dt.Columns.Add("filename", System.Type.GetType("System.String")); //Ae_Code

            HttpFileCollection hfc = System.Web.HttpContext.Current.Request.Files;
            string imgPath = "";
            string filename = "";
            string file_name = "";
            string file_path = "../Images/";
            string str_return = "";
            string[] files;
            //string chk_sty = "";
            //string pre_filename = "";
            int files_count = 0;

            if (hfc.Count > 0)
            {
                file_name = hfc[0].FileName;
                files = file_name.Split('\\');
                files_count = files.Count();
                filename = files[files_count - 1];

                imgPath = file_path + filename;
                string PhysicalPath = Server.MapPath(imgPath);
                hfc[0].SaveAs(PhysicalPath);

                //-------------------------------------------------//
                DataRow new_row = dt.NewRow();
                new_row[0] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                new_row[1] = filename;
                dt.Rows.Add(new_row);
                //-------------------------------------------------//
            }


            str_return = JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);


        }

 
        // 關於我們
        public ActionResult AboutUs(string lang="cn")
        {
            DataTable d_lang = DB.Lang_List();
            DataTable d_com_info = DB.Com_List("AboutUs", lang);
            ViewData["d_lang"] = d_lang;
            ViewData["d_com_info"] = d_com_info;
            ViewData["lang"] = lang;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult AboutUs_Update(string lang = "",string com_desc = "")
        {
            DB.Com_Update("AboutUs", lang, com_desc);

            DataTable d_lang = DB.Lang_List();
            DataTable d_com_info = DB.Com_List("AboutUs", lang);
            ViewData["d_lang"] = d_lang;
            ViewData["d_com_info"] = d_com_info;
            ViewData["lang"] = lang;

            return View("AboutUs");
        }

        // 證照專區
        public ActionResult Certificate(string lang = "cn")
        {
            DataTable d_lang = DB.Lang_List();
            DataTable d_com_info = DB.Com_List("Certificate", lang);
            ViewData["d_lang"] = d_lang;
            ViewData["d_com_info"] = d_com_info;
            ViewData["lang"] = lang;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Certificate_Update(string lang = "", string com_desc = "")
        {
            DB.Com_Update("Certificate", lang, com_desc);

            DataTable d_lang = DB.Lang_List();
            DataTable d_com_info = DB.Com_List("Certificate", lang);
            ViewData["d_lang"] = d_lang;
            ViewData["d_com_info"] = d_com_info;
            ViewData["lang"] = lang;

            return View("Certificate");
        }

        // 影音專區
        public ActionResult Video(string lang = "cn")
        {
            DataTable d_lang = DB.Lang_List();
            DataTable d_com_info = DB.Com_List("Video", lang);
            ViewData["d_lang"] = d_lang;
            ViewData["d_com_info"] = d_com_info;
            ViewData["lang"] = lang;

            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Video_Update(string lang = "", string com_desc = "")
        {
            DB.Com_Update("Video", lang, com_desc);

            DataTable d_lang = DB.Lang_List();
            DataTable d_com_info = DB.Com_List("Video", lang);
            ViewData["d_lang"] = d_lang;
            ViewData["d_com_info"] = d_com_info;
            ViewData["lang"] = lang;

            return View("Video");
        }

        // 分公司(可被海外實績選取並顯示在聯絡我們
        public ActionResult BranchsList()
        {
            return View();
        }
        public ActionResult BranchsData()
        {
            return View();
        }


        // 操作影片
        public ActionResult Tutorial()
        {
            return View();
        }
    }

}