using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LMS.Areas.LMS.Controllers
{
    public class LMSSettingsController : Controller
    {
        // GET: LMS/LMSSettings
        public ActionResult Index()
        {
            return View();
        }
        public JsonResult ClearQuiz()
        {
            int output = 0;
            Int32 Userid = Convert.ToInt32(Session["userid"]);
            output = ClearQuizSave(Userid);
            return Json(output, JsonRequestBehavior.AllowGet);
        }
        public int ClearQuizSave(Int32 user)
        {
            ProcedureExecute proc;

            try
            {
                DataTable dt = new DataTable();
                String con = System.Configuration.ConfigurationSettings.AppSettings["DBConnectionDefault"];
                SqlCommand sqlcmd = new SqlCommand();
                SqlConnection sqlcon = new SqlConnection(con);
                sqlcon.Open();
                sqlcmd = new SqlCommand("PRC_LMS_SETTINGS", sqlcon);
                sqlcmd.Parameters.AddWithValue("@ACTION", "ClearQuiz");
                sqlcmd.Parameters.AddWithValue("@USER_ID", user);               
                SqlParameter output = new SqlParameter("@ReturnValue", SqlDbType.Int);
                output.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(output);

                sqlcmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                da.Fill(dt);
                sqlcon.Close();

                Int32 ReturnValue = Convert.ToInt32(output.Value);

                sqlcmd.Dispose();
                sqlcmd.Dispose();
                return ReturnValue;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                proc = null;
            }
        }


    }
}