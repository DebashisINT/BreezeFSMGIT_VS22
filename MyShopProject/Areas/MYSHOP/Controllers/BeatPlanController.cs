/*************************************************************************************************************
Written by  Sanchita for   V2.0.40    15/05/2023      Beat Paln
*****************************************************************************************************************/

using BusinessLogicLayer;
using BusinessLogicLayer.SalesmanTrack;
using ClosedXML.Excel;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DevExpress.XtraSpreadsheet.Forms;
//using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using MyShop.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UtilityLayer;
using Excel = Microsoft.Office.Interop.Excel;

namespace MyShop.Areas.MYSHOP.Controllers
{
    public class BeatPlanController : Controller
    {
        // GET: MYSHOP/BeatPlan
        public ActionResult Index()
        {
            try
            {
                BeatPlanModel Dtls = new BeatPlanModel();
                DataSet ds = new DataSet();
                ProcedureExecute proc = new ProcedureExecute("PRC_BEATPLAN");
                proc.AddPara("@ACTION", "GetAreaRouteBeat");
                ds = proc.GetDataSet();

                if (ds != null)
                {
                    List<clsBeat> Beat = new List<clsBeat>();
                    Beat = APIHelperMethods.ToModelList<clsBeat>(ds.Tables[0]);

                    List<clsArea> Area = new List<clsArea>();
                    Area = APIHelperMethods.ToModelList<clsArea>(ds.Tables[1]);

                    List<clsRoute> Route = new List<clsRoute>();
                    Route = APIHelperMethods.ToModelList<clsRoute>(ds.Tables[2]);


                    Dtls.Fromdate = DateTime.Now.ToString("dd-MM-yyyy");
                    Dtls.Todate = DateTime.Now.ToString("dd-MM-yyyy");

                    Dtls.date_AssignedFrom = DateTime.Now.ToString("dd-MM-yyyy");
                    Dtls.date_AssignedTo = DateTime.Now.ToString("dd-MM-yyyy");

                    Dtls.BeatList = Beat;
                    Dtls.AreaList = Area;
                    Dtls.RouteList = Route;


                }
                
                EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/BeatPlan/Index");
                ViewBag.CanView = rights.CanView;
                ViewBag.CanExport = rights.CanExport;
                ViewBag.CanBulkUpdate = rights.CanBulkUpdate;
               

                return View(Dtls);
            }
            catch (Exception ex)
            {
                return Json("Error occurred. Error details: " + ex.Message);

            }
        }
        public ActionResult GetFromToDate(String Plan)
        {
            BeatPlanModel ret = new BeatPlanModel();

            if (Plan == "Daily" || Plan == "Custom")
            {
                ret.date_AssignedFrom = DateTime.Now.ToString("dd-MM-yyyy");
                ret.date_AssignedTo = DateTime.Now.ToString("dd-MM-yyyy");
            }
            else if(Plan == "Weekly")
            {
                ret.date_AssignedFrom = DateTime.Now.ToString("dd-MM-yyyy");
                ret.date_AssignedTo = DateTime.Now.AddDays(6).ToString("dd-MM-yyyy");
            }
            else if(Plan == "Monthly")
            {
                var month = DateTime.Now.Month;
                var year = DateTime.Now.Year;
                var daysInmonth = DateTime.DaysInMonth(year,month);
                var startdate = "01-"+ DateTime.Now.ToString("MM") + "-"+ DateTime.Now.ToString("yyyy");
                var enddate = daysInmonth.ToString().PadLeft(2,'0') + "-" + DateTime.Now.ToString("MM") + "-" + DateTime.Now.ToString("yyyy");

                ret.date_AssignedFrom = startdate;
                ret.date_AssignedTo = enddate;
            }
            
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetAreaRouteChain(String code_type, String code_id)
        {
            BeatPlanModel ret = new BeatPlanModel();

            BeatPlanModel Dtls = new BeatPlanModel();
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_BEATPLAN");
            proc.AddPara("@ACTION", "GetAreaRouteChain");
            proc.AddPara("@CODE_TYPE", code_type);
            proc.AddPara("@CODE_ID", code_id);
            dt = proc.GetTable();

            if (dt != null)
            {
                ret.AreaId = dt.Rows[0]["AreaId"].ToString();
                ret.RouteId = dt.Rows[0]["RouteId"].ToString();
            }

            return Json(ret, JsonRequestBehavior.AllowGet);

        }
    }
}