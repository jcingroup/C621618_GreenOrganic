using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace Lib.Service
{
    public class DBService
    {
        string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string csql = "";

        DataSet ds = new DataSet();

        #region 消息資料抓取 News_List
        public DataTable News_List(string news_id = "", string sort = "", string status = "", string title_query = "", string start_date = "", string end_date = "", string lang = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string[] Array_news_id;
            string[] Array_title_query;

            Array_news_id = news_id.Split(',');
            Array_title_query = title_query.Split(',');

            csql = "select "
                 + "  a1.* "
                 + "from "
                 + "("
                 + "select distinct "
                 + "  a1.n_id, a1.n_title, convert(nvarchar(10),a1.n_date,23) as n_date, a1.n_url, a1.n_desc "
                 + ", a1.lang, a1.is_index, a1.sort, a1.status, a2.lang_name "
                 + "from "
                 + "   news a1 "
                 + "left join lang a2 on a1.lang = a2.lang_id "
                 + "where "
                 + "  a1.status <> 'D' ";

            if (status.Trim().Length > 0)
            {
                csql = csql + " and a1.status = @status ";
            }

            if (news_id.Trim().Length > 0)
            {
                csql = csql + " and a1.n_id in (";
                for (int i = 0; i < Array_news_id.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + ",";
                    }
                    csql = csql + "@str_news_id" + i.ToString();
                }
                csql = csql + ") ";
            }

            if (title_query.Trim().Length > 0)
            {
                csql = csql + " and (";
                for (int i = 0; i < Array_title_query.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + " or ";
                    }
                    csql = csql + " a1.n_title like @str_title_query" + i.ToString() + " ";
                }
                csql = csql + ") ";
            }

            if (start_date.Trim().Length > 0)
            {

                csql = csql + "and a1.n_date >= @start_date ";
            }

            if (end_date.Trim().Length > 0)
            {

                csql = csql + "and a1.n_date <= @end_date ";
            }

            if (lang.Trim().Length > 0)
            {
                csql = csql + "and a1.lang = @lang ";
            }

            csql = csql + ")a1 ";

            if (sort.Trim().Length > 0)
            {
                csql = csql + " order by " + sort + " ";
            }
            else
            {
                csql = csql + " order by a1.sort desc ";
            }

            cmd.CommandText = csql;

            //---------------------------------------------------------------//
            cmd.Parameters.Clear();
            if (status.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@status", status);
            }

            if (start_date.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@start_date", start_date);
            }

            if (end_date.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@end_date", end_date);
            }

            if (lang.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@lang", lang);
            }

            if (news_id.Trim().Length > 0)
            {
                for (int i = 0; i < Array_news_id.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_news_id" + i.ToString(), Array_news_id[i]);
                }
            }

            if (title_query.Trim().Length > 0)
            {
                for (int i = 0; i < Array_title_query.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_title_query" + i.ToString(), "%" + Array_title_query[i] + "%");
                }
            }

            //--------------------------------------------------------------//

            if (ds.Tables["news"] != null)
            {
                ds.Tables["news"].Clear();
            }

            SqlDataAdapter news_ada = new SqlDataAdapter();
            news_ada.SelectCommand = cmd;
            news_ada.Fill(ds, "news");
            news_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["news"];
        }
        #endregion

        #region 語系資料 Lang_List
        public DataTable Lang_List(string lang_id = "")
        {
            string[] clang_id;
            string str_lang_id = "";
            clang_id = lang_id.Split(',');
            for (int i = 0; i < clang_id.Length; i++)
            {
                if (i > 0)
                {
                    str_lang_id = str_lang_id + ",";
                }
                str_lang_id = str_lang_id + clang_id[i];
            }

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select "
                 + "  * "
                 + "from "
                 + "  lang "
                 + "where "
                 + "  status = 'Y' ";


            if (lang_id.Trim().Length > 0)
            {
                csql = csql + " and lang_id in (";
                for (int i = 0; i < clang_id.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + ",";
                    }
                    csql = csql + "'@str_lang_id" + i.ToString() + "'";
                }
                csql = csql + ") ";
            }
            csql = csql + "order by sort ";

            cmd.CommandText = csql;

            if (lang_id.Trim().Length > 0)
            {
                cmd.Parameters.Clear();
                for (int i = 0; i < lang_id.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_lang_id" + i.ToString(), lang_id[i]);
                }
            }


            if (ds.Tables["lang"] != null)
            {
                ds.Tables["lang"].Clear();
            }

            SqlDataAdapter lang_ada = new SqlDataAdapter();
            lang_ada.SelectCommand = cmd;
            lang_ada.Fill(ds, "lang");
            lang_ada = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["lang"];
        }
        #endregion

        #region 消息資料 News_Insert
        public string News_Insert(string  n_title= "", string n_date = "", string n_desc = "", string lang = "", string show = "", string hot = "", string sort = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"insert into News(n_title,n_date,n_desc,lang,is_index,sort,status) "
                     + "values(@n_title,@n_date,@n_desc,@lang,@is_index,@sort,@status)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_title", n_title);
                cmd.Parameters.AddWithValue("@n_date", n_date);
                cmd.Parameters.AddWithValue("@n_desc", n_desc);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.Parameters.AddWithValue("@is_index", hot);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@status", show);

                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 消息資料更新 News_Update
        //更新內容
        public string News_Update(string n_id = "", string n_title = "", string n_date = "", string n_desc = "", string lang = "", string show = "", string hot = "",string sort = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  news "
                     + "set "
                     + "  n_title = @n_title "
                     + ", n_date = @n_date "
                     + ", n_desc = @n_desc "
                     + ", lang = @lang "
                     + ", status = @status "
                     + ", is_index = @is_index "
                     + ", sort = @sort "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  n_id = @n_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_id", n_id);
                cmd.Parameters.AddWithValue("@n_title", n_title);
                cmd.Parameters.AddWithValue("@n_date", n_date);
                cmd.Parameters.AddWithValue("@n_desc", n_desc);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.Parameters.AddWithValue("@is_index", hot);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@status", show);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }
        #endregion

        #region 消息資料刪除 News_Del
        public string News_Del(string n_id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"delete from "
                     + "  news "
                     + "where "
                     + "  n_id = @n_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@n_id", n_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 首頁影片資料 Video_List
        public DataTable Video_List()
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select "
                 + "  a1.* "
                 + "from "
                 + "  Advertisement a1 "
                 + "where "
                 + "  ad_id = 1";

            cmd.CommandText = csql;

            if (ds.Tables["video"] != null)
            {
                ds.Tables["video"].Clear();
            }

            SqlDataAdapter video_ada = new SqlDataAdapter();
            video_ada.SelectCommand = cmd;
            video_ada.Fill(ds, "video");
            video_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["video"];
        }
        #endregion

        #region 首頁影片資料更新 Video_Update
        public string Video_Update(string mv)
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "update "
                     + "  Advertisement "
                     + "set "
                     + "  ad_mv = @mv "
                     + "where "
                     + "  ad_id = 1";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@mv", mv);
                
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }
        #endregion

        #region 關於我們 Com_List
        //Com_List("AboutUs", lang)
        public DataTable Com_List(string category = "",string lang = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select "
                 + "  a1.* "
                 + "from "
                 + "  Company_Info a1 "
                 + "where "
                 + "    a1.category = @category "
                 + "and a1.lang = @lang";

            cmd.CommandText = csql;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@category", category);
            cmd.Parameters.AddWithValue("@lang", lang);

            if (ds.Tables["com_info"] != null)
            {
                ds.Tables["com_info"].Clear();
            }

            SqlDataAdapter com_info_ada = new SqlDataAdapter();
            com_info_ada.SelectCommand = cmd;
            com_info_ada.Fill(ds, "com_info");
            com_info_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["com_info"];
        }
        #endregion

        #region 關於我們更新 Com_Update
        //DB.Com_Update("AboutUs", lang, com_desc);
        public string Com_Update(string category = "", string lang = "", string com_desc = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = "update "
                     + "  Company_Info "
                     + "set "
                     + "  com_desc = @com_desc "
                     + "where "
                     + "    lang = @lang "
                     + "and category = @category";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@com_desc", com_desc);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.Parameters.AddWithValue("@category", category);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }
        #endregion

        #region 產品種類資料 Prod_Cate_list
        public DataTable Prod_Cate_List(string lang = "")
        {

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select "
                 + "  * "
                 + "from "
                 + "  Prod_category "
                 + "where "
                 + "    status = 'Y' ";
            if(lang.Trim().Length > 0)
            {
                csql = csql + "and lang = @lang ";
            }
            csql = csql + "order by "
                 + "  lang,sort ";
            
            cmd.CommandText = csql;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@lang", lang);
            

            if (ds.Tables["prod_category"] != null)
            {
                ds.Tables["prod_category"].Clear();
            }

            SqlDataAdapter prod_category_ada = new SqlDataAdapter();
            prod_category_ada.SelectCommand = cmd;
            prod_category_ada.Fill(ds, "prod_category");
            prod_category_ada = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["prod_category"];
        }
        #endregion

        #region 產品資料 Prod_List
        public DataTable Prod_List(string prod_id = "", string sort = "", string status = "", string title_query = "",  string lang = "",string cate_id = "")
        {
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            string[] Array_prod_id;
            string[] Array_title_query;
            string[] Array_cate_id;

            Array_prod_id = prod_id.Split(',');
            Array_title_query = title_query.Split(',');
            Array_cate_id = cate_id.Split(',');

            csql = "select "
                 + "  a1.* "
                 + "from "
                 + "("
                 + "select "
                 + "  a1.*,a2.cate_name,a3.lang_name "
                 + "from "
                 + "   prod a1 "
                 + "left join prod_category a2 on a1.cate_id = a2.cate_id "
                 + "left join lang a3 on a1.lang = a3.lang_id "
                 + "where "
                 + "  a1.status <> 'D' ";


            if (status.Trim().Length > 0)
            {
                csql = csql + " and a1.status = @status ";
            }

            if (prod_id.Trim().Length > 0)
            {
                csql = csql + " and a1.prod_id in (";
                for (int i = 0; i < Array_prod_id.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + ",";
                    }
                    csql = csql + "@str_prod_id" + i.ToString();
                }
                csql = csql + ") ";
            }

            if (cate_id.Trim().Length > 0)
            {
                csql = csql + " and a1.cate_id in (";
                for (int i = 0; i < Array_cate_id.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + ",";
                    }
                    csql = csql + "@str_cate_id" + i.ToString();
                }
                csql = csql + ") ";
            }

            if (title_query.Trim().Length > 0)
            {
                csql = csql + " and (";
                for (int i = 0; i < Array_title_query.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + " or ";
                    }
                    csql = csql + " a1.prod_name like @str_title_query" + i.ToString() + " ";
                }
                csql = csql + ") ";
            }

          
            if (lang.Trim().Length > 0)
            {
                csql = csql + "and a1.lang = @lang ";
            }

            csql = csql + ") a1 ";

            if (sort.Trim().Length > 0)
            {
                csql = csql + " order by " + sort + " ";
            }
            else
            {
                csql = csql + " order by a1.sort desc ";
            }

            cmd.CommandText = csql;

            //---------------------------------------------------------------//
            cmd.Parameters.Clear();
            if (status.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@status", status);
            }


            if (lang.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@lang", lang);
            }

            if (prod_id.Trim().Length > 0)
            {
                for (int i = 0; i < Array_prod_id.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_prod_id" + i.ToString(), Array_prod_id[i]);
                }
            }

            if (title_query.Trim().Length > 0)
            {
                for (int i = 0; i < Array_title_query.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_title_query" + i.ToString(), "%" + Array_title_query[i] + "%");
                }
            }

            //--------------------------------------------------------------//

            if (ds.Tables["prod"] != null)
            {
                ds.Tables["prod"].Clear();
            }

            SqlDataAdapter news_ada = new SqlDataAdapter();
            news_ada.SelectCommand = cmd;
            news_ada.Fill(ds, "prod");
            news_ada = null;

            cmd = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["prod"];
        }
        #endregion

        #region 產品資料新增 Prod_Insert
        //prod_name, manure_no, manure_info, manure_ingredients , manure_trait , prod_desc , lang, show, sort
        public string Prod_Insert(string cate = "", string prod_name = "", string manure_no = "", string manure_info = "", string manure_ingredients = "", string manure_trait = "", string prod_desc = "", string lang = "", string show = "", string sort = "", string img_no = "")
        {
            string c_msg = "";
            string prod_id = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"insert into Prod(cate_id, prod_name, manure_no, manure_info, manure_ingredients, manure_trait, prod_desc, lang, sort ,status) "
                     + "values(@cate,@prod_name,@manure_no,@manure_info,@manure_ingredients,@manure_trait,@prod_desc,@lang,@sort,@status)";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate", cate);
                cmd.Parameters.AddWithValue("@prod_name", prod_name);
                cmd.Parameters.AddWithValue("@manure_no", manure_no);
                cmd.Parameters.AddWithValue("@manure_info", manure_info);
                cmd.Parameters.AddWithValue("@manure_ingredients", manure_ingredients);
                cmd.Parameters.AddWithValue("@manure_trait", manure_trait);
                cmd.Parameters.AddWithValue("@prod_desc", prod_desc);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@status", show);

                cmd.ExecuteNonQuery();

                //抓取其編號
                csql = @"select distinct "
                     + "  _SEQ_ID "
                     + "from "
                     + "   Prod "
                     + "where "
                     + "    cate_id = @cate "
                     + "and prod_name = @prod_name "
                     + "and manure_no = @manure_no "
                     + "and manure_info = @manure_info "
                     + "and prod_name = @prod_name "
                     + "and manure_no = @manure_no "
                     + "and manure_info = @manure_info "
                     + "and manure_ingredients = @manure_ingredients "
                     + "and manure_trait = @manure_trait "
                     + "and prod_desc = @prod_desc "
                     + "and lang = @lang "
                     + "and sort = @sort "
                     + "and status = @status ";

                cmd.CommandText = csql;

                ////讓ADO.NET自行判斷型別轉換
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@cate", cate);
                cmd.Parameters.AddWithValue("@prod_name", prod_name);
                cmd.Parameters.AddWithValue("@manure_no", manure_no);
                cmd.Parameters.AddWithValue("@manure_info", manure_info);
                cmd.Parameters.AddWithValue("@manure_ingredients", manure_ingredients);
                cmd.Parameters.AddWithValue("@manure_trait", manure_trait);
                cmd.Parameters.AddWithValue("@prod_desc", prod_desc);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@status", show);

                if (ds.Tables["chk_prod"] != null)
                {
                    ds.Tables["chk_prod"].Clear();
                }

                SqlDataAdapter prod_chk_ada = new SqlDataAdapter();
                prod_chk_ada.SelectCommand = cmd;
                prod_chk_ada.Fill(ds, "chk_prod");
                prod_chk_ada = null;

                if (ds.Tables["chk_prod"].Rows.Count > 0)
                {
                    prod_id = ds.Tables["chk_prod"].Rows[0]["_SEQ_ID"].ToString();
                    if (img_no.Trim().Length > 0)
                    {
                        csql = @"update "
                             + "  prod_img "
                             + "set "
                             + "  img_no = @prod_id "
                             + "where "
                             + "  img_no = @img_no ";

                        cmd.CommandText = csql;

                        ////讓ADO.NET自行判斷型別轉換
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@prod_id", prod_id);
                        cmd.Parameters.AddWithValue("@img_no", img_no);

                        cmd.ExecuteNonQuery();
                    }
                }


            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 產品資料更新 Prod_Update
        //更新內容
        public string Prod_Update(string prod_id = "",string cate = "", string prod_name = "", string manure_no = "", string manure_info = "", string manure_ingredients = "", string manure_trait = "", string prod_desc = "", string lang = "", string show = "", string sort = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  prod "
                     + "set "
                     + "  cate_id = @cate "
                     + ", prod_name = @prod_name "
                     + ", manure_no = @manure_no "
                     + ", manure_info = @manure_info "
                     + ", manure_ingredients = @manure_ingredients "
                     + ", manure_trait = @manure_trait "
                     + ", prod_desc = @prod_desc "
                     + ", lang = @lang "
                     + ", status = @status "
                     + ", sort = @sort "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  prod_id = @prod_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@prod_id", prod_id);
                cmd.Parameters.AddWithValue("@cate", cate);
                cmd.Parameters.AddWithValue("@prod_name", prod_name);
                cmd.Parameters.AddWithValue("@manure_no", manure_no);
                cmd.Parameters.AddWithValue("@manure_info", manure_info);
                cmd.Parameters.AddWithValue("@manure_ingredients", manure_ingredients);
                cmd.Parameters.AddWithValue("@manure_trait", manure_trait);
                cmd.Parameters.AddWithValue("@prod_desc", prod_desc);
                cmd.Parameters.AddWithValue("@lang", lang);
                cmd.Parameters.AddWithValue("@sort", sort);
                cmd.Parameters.AddWithValue("@status", show);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;

        }
        #endregion

        #region 產品資料刪除 Prod_Del
        public string Prod_Del(string prod_id = "")
        {
            string c_msg = "";

            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"delete from "
                     + "  prod "
                     + "where "
                     + "  prod_id = @prod_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@prod_id", prod_id);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 產品資料圖片陳列 Prod_Img_List
        public DataTable Prod_Img_List(string img_no = "")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);
            string[] cimg_no;
            string str_img_no = "";
            cimg_no = img_no.Split(',');

            for (int i = 0; i < cimg_no.Length; i++)
            {
                if (i > 0)
                {
                    str_img_no = str_img_no + ",";
                }
                str_img_no = str_img_no + "'" + cimg_no[i] + "'";
            }

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            csql = "select * from prod_img where status = 'Y' and img_no in (";
            for (int i = 0; i < cimg_no.Length; i++)
            {
                if (i > 0)
                {
                    csql = csql + ",";
                }
                csql = csql + "@str_img_no" + i.ToString() + " ";
            }
            csql = csql + ") ";

            cmd.CommandText = csql;

            cmd.Parameters.Clear();
            for (int i = 0; i < cimg_no.Length; i++)
            {
                cmd.Parameters.AddWithValue("@str_img_no" + i.ToString(), cimg_no[i]);
            }


            if (dt.Tables["img"] != null)
            {
                dt.Tables["img"].Clear();
            }

            SqlDataAdapter scenic_ada = new SqlDataAdapter();
            scenic_ada.SelectCommand = cmd;
            scenic_ada.Fill(dt, "img");
            scenic_ada = null;

            d_t = dt.Tables["img"];

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;
            dt = null;

            return d_t;
        }
        #endregion

        #region 產品資料圖片刪除 Prod_Img_Delete
        public string Prod_Img_Delete(string img_id = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"delete from prod_img where _SEQ_ID = @img_id ";

                cmd.CommandText = csql;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_id", img_id);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 產品資料圖片更新 Prod_Img_Update
        public string Prod_Img_Update(string img_no = "", string img_file = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  prod_img "
                     + "set "
                     + "  img_file = @img_file "
                     + "where "
                     + "  img_no = @img_no ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_file", img_file);
                cmd.Parameters.AddWithValue("@img_no", img_no);
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 產品資料圖片新增 Prod_Img_Insert
        public string Prod_Img_Insert(string img_no = "", string img_file = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"insert into prod_img(img_no, img_file) "
                     + "values(@img_no ,@img_file)";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_no", img_no);
                cmd.Parameters.AddWithValue("@img_file", img_file);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion

        #region 使用者資訊 User_Info
        public DataTable User_Info(string account = "")
        {
            DataSet dt = new DataSet();
            DataTable d_t;
            SqlConnection conn = new SqlConnection(conn_str);

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            csql = @"select "
                 + "  * "
                 + "from "
                 + "  member "
                 + "where "
                 + "   status <> 'D' "
                 + "and account = @account "
                 + "order by "
                 + "  _SEQ_ID ";



            if (dt.Tables["user_info"] != null)
            {
                dt.Tables["user_info"].Clear();
            }

            SqlDataAdapter user_info_ada = new SqlDataAdapter();
            SqlCommand CMD = new SqlCommand(csql, conn);

            ////定義parameter型別
            CMD.Parameters.Clear();
            //CMD.Parameters.AddWithValue(@account, account);
            CMD.Parameters.Add("@account", SqlDbType.NVarChar, 15).Value = account; //(參數,宣考型態,長度)

            user_info_ada.SelectCommand = CMD;
            user_info_ada.Fill(dt, "user_info");
            user_info_ada = null;

            d_t = dt.Tables["user_info"];

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;
            dt = null;

            return d_t;
        }
        #endregion

        #region 變更密碼 User_Update
        public string User_Update(string account = "", string pwd = "")
        {
            string c_msg = "";
            SqlConnection conn = new SqlConnection(conn_str);
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            try
            {
                csql = @"update "
                     + "  member "
                     + "set "
                     + "  pwd = @pwd "
                     + "where "
                     + "  account = @account ";


                cmd.CommandText = csql;

                ////定義parameter型別
                cmd.Parameters.Clear();

                cmd.Parameters.Add("@account", SqlDbType.NVarChar, 30).Value = account; //(參數,宣考型態,長度)
                cmd.Parameters.Add("@pwd", SqlDbType.NVarChar, 30).Value = pwd; //(參數,宣考型態,長度)

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                c_msg = ex.Message;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
                cmd = null;
                conn = null;
            }

            return c_msg;
        }
        #endregion
    }
}