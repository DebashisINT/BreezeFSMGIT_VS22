using DevExpress.Web;
using DevExpress.Web.Mvc;
using LMS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityLayer;
using static LMS.Models.LMSReportsModel;


namespace LMS.Areas.LMS.Controllers
{
    public class LMSReportsController : Controller
    {
        // GET: LMS/LMSReports
        LMSReportsModel obj = new LMSReportsModel();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetTopicList()
        {
            BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(string.Empty);
            try
            {
                GetTopic dataobj = new GetTopic();
                List<GetTopic> productdata = new List<GetTopic>();
                List<GetTopic> modelbranch = new List<GetTopic>();
                DataTable ComponentTable = new DataTable();

               
                ComponentTable = obj.GETDROPDOWNVALUE("GETTOPIC");
                modelbranch = APIHelperMethods.ToModelList<GetTopic>(ComponentTable);

                return PartialView("~/Areas/LMS/Views/LMSReports/_TopicLookUpPartial.cshtml", modelbranch);

            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public ActionResult GETUSERLIST()
        {
            BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(string.Empty);
            try
            {
                GetUser dataobj = new GetUser();
                List<GetUser> productdata = new List<GetUser>();
                List<GetUser> modelbranch = new List<GetUser>();
                DataTable ComponentTable = new DataTable();


                ComponentTable = obj.GETDROPDOWNVALUE("GETUSER");
                modelbranch = APIHelperMethods.ToModelList<GetUser>(ComponentTable);

                //DataSet output = new DataSet();
                //output = obj.EditQuestion(QUESTIONS_ID);
                //if (output.Tables[1].Rows.Count > 0)
                //{
                //    if (output.Tables[1] != null && output.Tables[1].Rows.Count > 0)
                //    {
                //        DataTable objData = output.Tables[1];
                //        foreach (DataRow row in objData.Rows)
                //        {
                //            dataobj = new GetTopic();
                //            dataobj.TOPICID = Convert.ToInt64(row["QUESTIONS_TOPICID"]);
                //            productdata.Add(dataobj);

                //        }
                //        ViewBag.QUESTIONS_TOPICIDS = productdata;
                //    }

                //}
                //if (TempData["TopicID"] != null)
                //{
                //    ViewBag.CONTENT_TOPICIDS = Convert.ToInt64(TempData["TopicID"]);
                //}


                return PartialView("~/Areas/LMS/Views/LMSReports/_UserLookUpPartial.cshtml", modelbranch);

            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }
        public ActionResult GetContentList()
        {
            BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(string.Empty);
            try
            {
                GetContent dataobj = new GetContent();
                List<GetContent> productdata = new List<GetContent>();
                List<GetContent> modelbranch = new List<GetContent>();
                DataTable ComponentTable = new DataTable();


                ComponentTable = obj.GETDROPDOWNVALUE("GetContent");
                modelbranch = APIHelperMethods.ToModelList<GetContent>(ComponentTable);

                return PartialView("~/Areas/LMS/Views/LMSReports/_ContentLookUpPartial.cshtml", modelbranch);

            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public PartialViewResult _PartialLateVisitGrid(string ispageload)
        {
            return PartialView(GetReport(ispageload));
        }


        public IEnumerable GetReport(string ispageload)
        {
            string connectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ERP_ConnectionString"]);
            string Userid = Convert.ToString(Session["userid"]);

            if (ispageload != "1")
            {
                LMSMasterDataContext dc = new LMSMasterDataContext(connectionString);
                var q = from d in dc.LMS_REPORTSLISTs
                        where Convert.ToString(d.USER_ID) == Userid
                        orderby d.SEQ descending
                        select d;
                return q;
            }
            else
            {
                LMSMasterDataContext dc = new LMSMasterDataContext(connectionString);
                var q = from d in dc.LMS_REPORTSLISTs
                        where 1 == 0
                        orderby d.SEQ descending
                        select d;
                return q;
            }
        }

        //Rev Debashis && BranchId added 0025198
        public JsonResult CreateLINQTable(string UserIds, string Topic_Id, string Content_Id,  DateTime fromdate, DateTime todate)
        {
            string output = "";
            //EmployeeLateVisit objEmployeeLateVisit = new EmployeeLateVisit();
            obj.CreateTable(UserIds, Topic_Id, Content_Id,fromdate, todate, Convert.ToString(Session["userid"]));
            return Json(output, JsonRequestBehavior.AllowGet);

        }



        public ActionResult ExporSummaryList(int type)
        {


            switch (type)
            {
                case 1:
                    return GridViewExtension.ExportToPdf(GetEmployeeBatchGridViewSettings(), GetReport(""));
                //break;
                case 2:
                    return GridViewExtension.ExportToXlsx(GetEmployeeBatchGridViewSettings(), GetReport(""));
                //break;
                case 3:
                    return GridViewExtension.ExportToXls(GetEmployeeBatchGridViewSettings(), GetReport(""));
                case 4:
                    return GridViewExtension.ExportToRtf(GetEmployeeBatchGridViewSettings(), GetReport(""));
                case 5:
                    return GridViewExtension.ExportToCsv(GetEmployeeBatchGridViewSettings(), GetReport(""));
                //break;

                default:
                    break;
            }

            return null;
        }

        private GridViewSettings GetEmployeeBatchGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "Employee Summary Report";
            settings.CallbackRouteValues = new { Action = "_PartialLateVisitGrid", Controller = "ShopVisitRegister" };
            // Export-specific settings
            settings.SettingsExport.ExportedRowType = GridViewExportedRowType.All;
            settings.SettingsExport.FileName = "Employee Late Visit Report";

            settings.Columns.Add(x =>
            {
                x.FieldName = "SL";
                x.Caption = "SL";
                // x.VisibleIndex = 1;
                x.Width = 0;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "VISIT_DATE";
                x.Caption = "Visit Date";
                x.VisibleIndex = 1;
                x.Width = 100;
                x.ColumnType = MVCxGridViewColumnType.DateEdit;
                (x.PropertiesEdit as DateEditProperties).DisplayFormatString = "dd-MM-yyyy";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "EMPID";
                x.Caption = "Emp. ID";
                x.VisibleIndex = 2;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "EMP_NAME";
                x.Caption = "Employee";
                x.VisibleIndex = 3;
                x.Width = 200;
            });

            //Rev Debashis 0025198
            settings.Columns.Add(x =>
            {
                x.FieldName = "BRANCHDESC";
                x.Caption = "Branch";
                x.VisibleIndex = 4;
                x.Width = 200;
            });
            //End of Rev Debashis 0025198

            settings.Columns.Add(x =>
            {
                x.FieldName = "STATE_NAME";
                x.Caption = "State Name";
                x.VisibleIndex = 5;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "DESIGNATION";
                x.Caption = "Designation";
                x.VisibleIndex = 6;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "SUPERVISOR_NAME";
                x.Caption = "Supervisor";
                x.VisibleIndex = 7;
                x.Width = 200;
                x.ColumnType = MVCxGridViewColumnType.TextBox;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "ATT_STATUS";
                x.Caption = "Attendance";
                x.VisibleIndex = 8;
                x.Width = 100;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "LOGIN_TIME";
                x.Caption = "Login Time";
                x.VisibleIndex = 9;
                x.Width = 150;
                x.ColumnType = MVCxGridViewColumnType.TimeEdit;
                (x.PropertiesEdit as TimeEditProperties).DisplayFormatString = "hh:mm tt";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "total_Count";
                x.Caption = "Total Visit Count";
                x.VisibleIndex = 10;
                x.Width = 100;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "PARTY_NAME_FIRST";
                x.Caption = "Party Name(First Call)";
                x.VisibleIndex = 11;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "MOBILE_FIRST";
                x.Caption = "Contact Number";
                x.VisibleIndex = 12;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "SHOP_ADDRESS_FIRST";
                x.Caption = "Address";
                x.VisibleIndex = 13;
                x.Width = 250;
            });

            //settings.Columns.Add(x =>
            //{
            //    x.FieldName = "PP_NAME_FIRST";
            //    x.Caption = "PP Name";
            //    x.VisibleIndex = 12;
            //    x.Width = 200;

            //});

            //settings.Columns.Add(x =>
            //{
            //    x.FieldName = "DD_NAME_FIRST";
            //    x.Caption = "DD Name";
            //    x.VisibleIndex = 13;
            //    x.Width = 200;
            //});

            settings.Columns.Add(x =>
            {
                x.FieldName = "FIRST_VISIT";
                x.Caption = "First Call time";
                x.VisibleIndex = 14;
                x.Width = 100;
                x.ColumnType = MVCxGridViewColumnType.DateEdit;
                (x.PropertiesEdit as DateEditProperties).DisplayFormatString = "hh:mm tt";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "FIRST_REMARKS";
                x.Caption = "Remarks (If visit time after 11.30 AM)";
                if (TempData["starttime"] != null)
                {
                    x.Caption = "Remarks (If visit time after " + TempData["starttime"] + ")";
                }
                x.VisibleIndex = 15;
                x.Width = 150;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "FIRST_DURATION";
                x.Caption = "First Call Duration (HH:MM)";
                x.VisibleIndex = 16;
                x.Width = 100;

            });


            settings.Columns.Add(x =>
            {
                x.FieldName = "ONE_HOUR_FIRST_REMARKS";
                x.Caption = "Remarks (Time Spent In First Call)";
                x.VisibleIndex = 17;
                x.Width = 150;
            });


            settings.Columns.Add(x =>
            {
                x.FieldName = "PARTY_NAME_LAST";
                x.Caption = "Party Name (Last Call)";
                x.VisibleIndex = 18;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "MOBILE_LAST";
                x.Caption = "Contact Number";
                x.VisibleIndex = 19;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "SHOP_ADDRESS_LAST";
                x.Caption = "Address";
                x.VisibleIndex = 20;
                x.Width = 250;
            });

            //settings.Columns.Add(x =>
            //{
            //    x.FieldName = "PP_NAME_LAST";
            //    x.Caption = "PP Name";
            //    x.VisibleIndex = 21;
            //    x.Width = 200;

            //});

            //settings.Columns.Add(x =>
            //{
            //    x.FieldName = "DD_NAME_LAST";
            //    x.Caption = "DD Name";
            //    x.VisibleIndex = 22;
            //    x.Width = 200;
            //});

            settings.Columns.Add(x =>
            {
                x.FieldName = "LAST_VISIT";
                x.Caption = "Last Shop Visit time";
                x.VisibleIndex = 21;
                x.Width = 100;
                x.ColumnType = MVCxGridViewColumnType.DateEdit;
                (x.PropertiesEdit as DateEditProperties).DisplayFormatString = "hh:mm tt";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "LAST_REMARKS";
                x.Caption = "Remarks (If visit time before 18:00 PM)";
                if (TempData["endtime"] != null)
                {
                    x.Caption = "Remarks (If visit time before " + TempData["endtime"] + ")";
                }
                x.VisibleIndex = 22;
                x.Width = 150;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "LAST_DURATION";
                x.Caption = "Last Shop Duration (HH:MM)";
                x.VisibleIndex = 23;
                x.Width = 100;

            });


            settings.Columns.Add(x =>
            {
                x.FieldName = "ONE_HOUR_LAST_REMARKS";
                x.Caption = "Remarks (Time Spent In Last Call)";
                x.VisibleIndex = 24;
                x.Width = 150;
            });



            settings.Columns.Add(x =>
            {
                x.FieldName = "LOGOUT_TIME";
                x.Caption = "Logout Time";
                x.VisibleIndex = 25;
                x.Width = 100;
                x.ColumnType = MVCxGridViewColumnType.DateEdit;
                (x.PropertiesEdit as DateEditProperties).DisplayFormatString = "hh:mm tt";
            });


            settings.Columns.Add(x =>
            {
                x.FieldName = "ORDER_VALUE";
                x.Caption = "Total Order Value";
                x.VisibleIndex = 26;
                x.Width = 100;
                x.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
                x.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
                x.PropertiesEdit.DisplayFormatString = "0.00";
            });



            ///Summary
            settings.TotalSummary.Add(DevExpress.Data.SummaryItemType.Sum, "ORDER_VALUE").DisplayFormat = "0.00";
            settings.TotalSummary.Add(DevExpress.Data.SummaryItemType.Sum, "total_Count").DisplayFormat = "0";

            TempData.Keep();


            settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            settings.SettingsExport.LeftMargin = 20;
            settings.SettingsExport.RightMargin = 20;
            settings.SettingsExport.TopMargin = 20;
            settings.SettingsExport.BottomMargin = 20;

            return settings;
        }


    }
}