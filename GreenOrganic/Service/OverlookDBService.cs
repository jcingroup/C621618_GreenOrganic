using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace SkyView.Service
{
    public class OverlookDBService
    {
        string conn_str = WebConfigurationManager.ConnectionStrings["conn_string"].ConnectionString.ToString();
        string csql = "";
        DataSet ds = new DataSet();

        #region 消息資料抓取 List
        public DataTable News_List(string news_id = "", string sort = "", string status = "", string title_query = "",string start_date = "", string end_date = "",string lang = "")
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
            
            csql = "select distinct "
                 + " a1.n_id, a1.n_title, a1.n_date, a1.n_url, a1.n_desc, a1.lang, a1.is_index, a1.sort, a1.status, a1.lang, a2.lang_name "
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
                    csql = csql + "and  a1.n_title like @str_title_query" + i.ToString() + " ";
                }
                csql = csql + ") ";
            }

            if(start_date.Trim().Length > 0)
            {
                
                csql = csql + "and a1.n_date >= convert(datetime, @start_date, 111) ";
            }

            if (end_date.Trim().Length > 0)
            {

                csql = csql + "and a1.n_date <= convert(datetime, @end_date, 111) ";
            }

            if (lang.Trim().Length > 0)
            {
                csql = csql + "and a1.lang = '@lang' ";
            }

            if (sort.Trim().Length > 0)
            {
                csql = csql + " order by " + sort + " ";
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
                cmd.Parameters.AddWithValue("@end_date", start_date);
            }

            if (lang.Trim().Length > 0)
            {
                cmd.Parameters.AddWithValue("@lang", lang);
            }

            if (news_id.Trim().Length > 0)
            {
                for (int i = 0; i < Array_news_id.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_scenic_id" + i.ToString(), Array_news_id[i]);
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

        #region 語系資料
        public DataTable LangList(string lang_id = "")
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





        #region 景觀資料刪除 Del
        public string Del(string scenic_id = "")
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
                     + "  scenic_bt "
                     + "where "
                     + "  _SEQ_ID = @scenic_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@scenic_id", scenic_id);

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

        #region 景觀狀態更新 Upd_Status
        public string Upd_Status(string scenic_id = "", string status = "")
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
                     + "  scenic_bt "
                     + "set "
                     + "  status = @status "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  _SEQ_ID = @scenic_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@status", status);
                cmd.Parameters.AddWithValue("@scenic_id", scenic_id);

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

        #region 桃園市區域資料 AreaList
        public DataTable AreaList(string area_id = "")
        {
            string[] carea_id;
            string str_area_id = "";
            carea_id = area_id.Split(',');
            for (int i = 0; i < carea_id.Length; i++)
            {
                if (i > 0)
                {
                    str_area_id = str_area_id + ",";
                }
                str_area_id = str_area_id + carea_id[i];
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
                 + "  area_bt "
                 + "where "
                 + "  status = 'Y' ";

            
            if (area_id.Trim().Length > 0)
            {
                csql = csql + " and _SEQ_ID in (";
                for (int i = 0; i < carea_id.Length; i++)
                {
                    if (i > 0)
                    {
                        csql = csql + ",";
                    }
                    csql = csql + "@str_area_id" + i.ToString();
                }
                csql = csql + ") ";
            }
            csql = csql + "order by _SEQ_ID ";

            cmd.CommandText = csql;

            if (area_id.Trim().Length > 0)
            {
                cmd.Parameters.Clear();
               for (int i = 0; i < carea_id.Length; i++)
                {
                    cmd.Parameters.AddWithValue("@str_area_id" + i.ToString(), carea_id[i]);
                }
            }
            

            if (ds.Tables["area"] != null)
            {
                ds.Tables["area"].Clear();
            }

            SqlDataAdapter scenic_ada = new SqlDataAdapter();
            scenic_ada.SelectCommand = cmd;
            scenic_ada.Fill(ds, "area");
            scenic_ada = null;

            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
            conn = null;

            return ds.Tables["area"];
        }
        #endregion

        #region 景觀圖片新增 Img_Insert
        public string Img_Insert(string img_no = "", string img_file = "", string img_sty = "")
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
                csql = @"insert into scenic_img(img_no, img_file, img_sty) "
                     + "values(@img_no ,@img_file ,@img_sty)";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_no", img_no);
                cmd.Parameters.AddWithValue("@img_file", img_file);
                cmd.Parameters.AddWithValue("@img_sty", img_sty);

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

        #region 景觀圖片刪除 Img_Delete
        public string Img_Delete(string img_id = "")
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
                csql = @"delete from scenic_img where _SEQ_ID = @img_id ";

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

        #region 景觀觀看次數 add_count
        public string add_count(string scenic_id="")
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
                     + "  scenic_bt "
                     + "set "
                     + "  scenic_count = scenic_count + 1 "
                     + ", _UPD_ID = 'System' "
                     + ", _UPD_DT = getdate() "
                     + "where "
                     + "  _SEQ_ID = @scenic_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@scenic_id", scenic_id);
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

        #region 景觀圖片更新 Img_Update
        public string Img_Update(string img_id = "", string img_file = "")
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
                     + "  scenic_img "
                     + "set "
                     + "  img_file = @img_file "
                     + "where "
                     + "  _SEQ_ID = @img_id ";

                cmd.CommandText = csql;

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@img_file", img_file);
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

        #region 景觀圖片陳列 Img_List
        public DataTable Img_List(string img_no = "", string img_sty = "B")
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

            csql = "select * from scenic_img where status = 'Y' and img_no in (";
            for (int i = 0; i < cimg_no.Length; i++)
            {
                if (i > 0)
                {
                    csql = csql + ",";
                }
                csql = csql + "@str_img_no" + i.ToString() + " ";
            }
            csql = csql + ") and img_sty= @img_sty order by _SEQ_ID ";

            cmd.CommandText = csql;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@img_sty",img_sty);
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

   
    }
}