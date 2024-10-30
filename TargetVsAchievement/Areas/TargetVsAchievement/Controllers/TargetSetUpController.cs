using DevExpress.XtraReports.Templates;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TargetVsAchievement.Models;

namespace TargetVsAchievement.Areas.TargetVsAchievement.Controllers
{
    public class TargetSetUpController : Controller
    {
        // GET: TargetVsAchievement/TargetSetUp
        public ActionResult Index()
        {
            return View();
        }
        public PartialViewResult _PartialTargetSetUpListing(TargetLevelSetupModel dd)
        {
            string ACTION = "";
            if (dd.TargetType== "SalesTarget")
            {
                ACTION = "SALESTARGETASSIGN";
            }
            

            DataTable dt = new DataTable();
            String con = System.Configuration.ConfigurationSettings.AppSettings["DBConnectionDefault"];
            SqlCommand sqlcmd = new SqlCommand();
            SqlConnection sqlcon = new SqlConnection(con);
            sqlcon.Open();
            sqlcmd = new SqlCommand("FSM_TARGETASSIGN_LISTING", sqlcon);
            sqlcmd.Parameters.Add("@ACTION", ACTION);
            sqlcmd.Parameters.Add("@USERID", Convert.ToString(Session["userid"]));
          
            sqlcmd.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
            da.Fill(dt);
            sqlcon.Close();
            //TempData["LMSDashboardGridView"] = dt;
            //TempData.Keep();
            return PartialView(dt);
        }


        
    }


    
}