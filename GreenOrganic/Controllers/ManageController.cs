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

namespace SkyView.Controllers
{
    public class ManageController : WebUserController
    {
        OverlookDBService OverlookDB = new OverlookDBService();

        // 後台首頁-導向Login
        public ActionResult Index()
        {
            if(Convert.ToString(Session["IsLogined"]) == "Y")
            {
                return RedirectToAction("OverlookList"); 
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
                        user_info = OverlookDB.User_Info(account);
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
                    return RedirectToAction("OverlookList");
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

        // 空拍
        public ActionResult OverlookList(string txt_area_query = "", string txt_scenic_query = "", int page = 1,string txt_sort = "",string txt_a_d = "")
        {
            //定義變數
            string c_sort = "";
            DataTable dt;

            //排序設定
            if (txt_sort.Trim().Length > 0)
            {
                c_sort = c_sort + "a1." + txt_sort;
                if (txt_sort == "area")
                {
                    c_sort = c_sort + "_name";
                }
            }
            if(txt_a_d.Trim().Length > 0)
            {
                c_sort = c_sort + " " + txt_a_d;
            }

            //抓取景觀資料
            dt = OverlookDB.List("",c_sort,"","",txt_area_query,txt_scenic_query);

            //設定傳值
            ViewData["page"] = page;
            ViewData["dt"] = dt;
            ViewData["txt_area_query"] = txt_area_query;
            ViewData["txt_scenic_query"] = txt_scenic_query;
            ViewData["txt_sort"] = txt_sort;
            ViewData["txt_a_d"] = txt_a_d;

            return View();
        }
        
        //新增
        public ActionResult Add()
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            DataTable d_area = OverlookDB.AreaList();
            DataTable d_scenic = OverlookDB.List("", "");
            DataTable d_scenic_img_b = OverlookDB.Img_List("", "B");
            DataTable d_scenic_img_s = OverlookDB.Img_List("", "S");

            ViewData["d_area"] = d_area;
            ViewData["d_scenic"] = d_scenic;
            ViewData["d_scenic_img_b"] = d_scenic_img_b;
            ViewData["d_scenic_img_s"] = d_scenic_img_b;
            ViewData["action_sty"] = "add";
            
            return View("OverlookData");
        }

        //修改
        public ActionResult Edit(string scenic_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            DataTable d_area = OverlookDB.AreaList();
            DataTable d_scenic = OverlookDB.List(scenic_id, "");
            DataTable d_scenic_img_b = OverlookDB.Img_List(scenic_id,"B");
            DataTable d_scenic_img_s = OverlookDB.Img_List(scenic_id, "S");
            ViewData["d_area"] = d_area;
            ViewData["d_scenic"] = d_scenic;
            ViewData["d_scenic_img_b"] = d_scenic_img_b;
            ViewData["d_scenic_img_s"] = d_scenic_img_s;
            ViewData["action_sty"] = "edit";

            return View("OverlookData");
        }

        public ActionResult Del(string scenic_id = "")
        {
            //OverlookDBService OverlookDB = new OverlookDBService();
            OverlookDB.Del(scenic_id);
            return RedirectToAction("OverlookList");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Save(string action_sty, string area_id, string scenic_id, string scenic_name, string longitude, string latitude, string scenic_desc,string show,string img_no)
        {
            //OverlookDBService OverlookDB = new OverlookDBService();

            switch (action_sty)
            {
                case "add":
                    OverlookDB.Insert(area_id, scenic_name, longitude, latitude, scenic_desc,show,img_no);
                    break;
                case "edit":
                    OverlookDB.Update(scenic_id, area_id, scenic_name, longitude, latitude, scenic_desc,show);
                    break;
            }

            return RedirectToAction("OverlookList");
        }


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
            user_info = OverlookDB.User_Info(Account);
            //檢查登入密碼是否正確
            if(now_pwd == user_info.Rows[0]["pwd"].ToString())
            {
                OverlookDB.User_Update(Account, new_pwd);
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

        public ActionResult Upload(string img_no,string img_sta)
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

            chk_file = OverlookDB.Img_List(img_no, img_sta);

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
                    OverlookDB.Img_Insert(img_no, img_no + "_" + img_sta + "_" + filename, img_sta);
                    break;
                case "update":
                    OverlookDB.Img_Update(chk_file.Rows[0]["_SEQ_ID"].ToString(), img_no + "_" + img_sta + "_" + filename);

                    //刪除原本檔案
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
                    break;
            }

            //抓取資料
            img_file = OverlookDB.Img_List(img_no, img_sta);

            str_return = JsonConvert.SerializeObject(img_file, Newtonsoft.Json.Formatting.Indented);

            return Content(str_return);
         

        }

        public ActionResult Img_Del(string img_id,string img_sta,string img_no)
        {
            string str_return = "";
            DataTable img_file;
            DataTable chk_file;
            string filename = "";
            string file_path = "../Images/";
            string imgPath = "";

            chk_file = OverlookDB.Img_List(img_no, img_sta);
            filename = "";
            if (chk_file.Rows.Count > 0)
            {
                for(int i = 0; i< chk_file.Rows.Count; i++)
                {
                    if(img_id == chk_file.Rows[i]["_SEQ_ID"].ToString())
                    {
                        filename = chk_file.Rows[i]["img_file"].ToString();
                        break;
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

            OverlookDB.Img_Delete(img_id);
            //抓取資料
            img_file = OverlookDB.Img_List(img_no, img_sta);
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
    }

}