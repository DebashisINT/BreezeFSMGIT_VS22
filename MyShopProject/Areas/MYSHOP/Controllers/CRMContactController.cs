/***************************************************************************************
 * Written by Sanchita on 24/11/2023 for V2.0.43    A new design page is required as Contact (s) under CRM menu. 
 *                                                  Mantis: 27034 
 ****************************************************************************************/
using BusinessLogicLayer;
using BusinessLogicLayer.SalesmanTrack;
using DataAccessLayer;
using DevExpress.Utils;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Models;
using MyShop.Models;
using Newtonsoft.Json;
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
using System.Net.Http.Headers;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Xml;
using UtilityLayer;

namespace MyShop.Areas.MYSHOP.Controllers
{
    public class CRMContactController : Controller
    {
        // GET: MYSHOP/CRMContact
        NotificationBL notificationbl = new NotificationBL();
        DBEngine odbengine = new DBEngine();

        public ActionResult Index()
        {
            CRMContactModel Dtls = new CRMContactModel();
           
            Dtls.Fromdate = DateTime.Now.ToString("dd-MM-yyyy");
            Dtls.Todate = DateTime.Now.ToString("dd-MM-yyyy");
            Dtls.Is_PageLoad = "Ispageload";
            Dtls.user_id = Convert.ToString(Session["userid"]);

            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("PRC_FTSContactListingShow");
            proc.AddPara("@ACTION", "GetDropdownBindData");
            ds = proc.GetDataSet();
            
            if (ds != null)
            {
                // Company
                List<CompanyList> CompanyList = new List<CompanyList>();
                CompanyList = APIHelperMethods.ToModelList<CompanyList>(ds.Tables[0]);
                Dtls.CompanyList = CompanyList;

                // Assign To
                List<AssignToList> AssignToList = new List<AssignToList>();
                AssignToList = APIHelperMethods.ToModelList<AssignToList>(ds.Tables[1]);
                Dtls.AssignToList = AssignToList;

                // Type
                List<TypeList> TypeList = new List<TypeList>();
                TypeList = APIHelperMethods.ToModelList<TypeList>(ds.Tables[2]);
                Dtls.TypeList = TypeList;

                // Status
                List<StatusList> StatusList = new List<StatusList>();
                StatusList = APIHelperMethods.ToModelList<StatusList>(ds.Tables[3]);
                Dtls.StatusList = StatusList;

                // Source
                List<SourceList> SourceList = new List<SourceList>();
                SourceList = APIHelperMethods.ToModelList<SourceList>(ds.Tables[4]);
                Dtls.SourceList = SourceList;

                // Stage
                List<StageList> StageList = new List<StageList>();
                StageList = APIHelperMethods.ToModelList<StageList>(ds.Tables[5]);
                Dtls.StageList = StageList;

                // Reference
                //List<ReferenceList> ReferenceList = new List<ReferenceList>();
                //ReferenceList = APIHelperMethods.ToModelList<ReferenceList>(ds.Tables[5]);
                //Dtls.ReferenceList = ReferenceList;

            }

            EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/CRMContact/Index");
            ViewBag.CanAdd = rights.CanAdd;
            ViewBag.CanView = rights.CanView;
            ViewBag.CanExport = rights.CanExport;
            ViewBag.CanReassign = rights.CanReassign;
            ViewBag.CanAssign = rights.CanAssign;
            ViewBag.CanBulkUpdate = rights.CanBulkUpdate;

            return View(Dtls);
        }

        public ActionResult GetContactFrom()
        {
            try
            {
                List<GetEnquiryFrom> modelEnquiryFrom = new List<GetEnquiryFrom>();

                DataTable dtEnquiryFrom = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_FTSContactListingShow");
                proc.AddPara("@ACTION_TYPE", "GetContactFrom");
                dtEnquiryFrom = proc.GetTable();

                modelEnquiryFrom = APIHelperMethods.ToModelList<GetEnquiryFrom>(dtEnquiryFrom);

                return PartialView("~/Areas/MYSHOP/Views/CRMEnquiries/_EnquiryFromPartial.cshtml", modelEnquiryFrom);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public ActionResult PartialCRMContactGridList(CRMContactModel model)
        {
            try
            {
                EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/CRMContact/Index");
                ViewBag.CanAdd = rights.CanAdd;
                ViewBag.CanView = rights.CanView;
                ViewBag.CanExport = rights.CanExport;
                ViewBag.CanReassign = rights.CanReassign;
                ViewBag.CanAssign = rights.CanAssign;
                ViewBag.CanBulkUpdate = rights.CanBulkUpdate;
               
                string ContactFrom = "";
                int i = 1;

                if (model.ContactFromDesc != null && model.ContactFromDesc.Count > 0)
                {
                    foreach (string item in model.ContactFromDesc)
                    {
                        if (i > 1)
                            ContactFrom = ContactFrom + "," + item;
                        else
                            ContactFrom = item;
                        i++;
                    }

                }


                string Is_PageLoad = string.Empty;

                if (model.Is_PageLoad == "Ispageload")
                {
                    Is_PageLoad = "is_pageload";

                }

                string datfrmat = model.Fromdate.Split('-')[2] + '-' + model.Fromdate.Split('-')[1] + '-' + model.Fromdate.Split('-')[0];
                string dattoat = model.Todate.Split('-')[2] + '-' + model.Todate.Split('-')[1] + '-' + model.Todate.Split('-')[0];


                GetCRMContactListing(ContactFrom, datfrmat, dattoat, Is_PageLoad);

                model.Is_PageLoad = "Ispageload";

                return PartialView("PartialCRMContactGridList", GetCRMContactDetails(Is_PageLoad));

            }
            catch (Exception ex)
            {
                throw ex;
              
            }

        }

        public void GetCRMContactListing(string ContactFrom, string FromDate, string ToDate, string Is_PageLoad)
        {
            string user_id = Convert.ToString(Session["userid"]);

            string action = string.Empty;
            DataTable formula_dtls = new DataTable();
            DataSet dsInst = new DataSet();

            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_FTSContactListingShow");
                proc.AddPara("@ACTION", "GETLISTINGDATA");
                proc.AddPara("@IS_PAGELOAD", Is_PageLoad);
                proc.AddPara("@USERID", Convert.ToInt32(user_id));
                proc.AddPara("@CONTACTSFROM", ContactFrom);
                proc.AddPara("@FROMDATE", FromDate);
                proc.AddPara("@TODATE", ToDate);
                dt = proc.GetTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable GetCRMContactDetails(string Is_PageLoad)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ERP_ConnectionString"].ConnectionString;
            string Userid = Convert.ToString(Session["userid"]);
            
            ////////DataTable dtColmn = GetPageRetention(Session["userid"].ToString(), "CRM Contact");
            ////////if (dtColmn != null && dtColmn.Rows.Count > 0)
            ////////{
            ////////    ViewBag.RetentionColumn = dtColmn;//.Rows[0]["ColumnName"].ToString()  DataTable na class pathao ok wait
            ////////}
            
            if (Is_PageLoad != "is_pageload")
            {
                ReportsDataContext dc = new ReportsDataContext(connectionString);
                var q = from d in dc.CRM_CONTACT_LISTINGs
                        where d.USERID == Convert.ToInt32(Userid)
                        orderby d.SEQ descending
                        select d;
                return q;
            }
            else
            {
                ReportsDataContext dc = new ReportsDataContext(connectionString);
                var q = from d in dc.CRM_CONTACT_LISTINGs
                        where d.USERID == Convert.ToInt32(Userid) && d.SEQ == 11111111119
                        orderby d.SEQ descending
                        select d;
                return q;
            }


        }

        [HttpPost]
        public ActionResult AddCRMContact(AddCrmContactData data)
        {
            try
            {
                string DOB = null;
                string Anniversary = null;
                string NextFollowDate = null;

                if (data.DOB != null && data.DOB != "01-01-0100")
                {
                    DOB = data.DOB.Split('-')[2] + '-' + data.DOB.Split('-')[1] + '-' + data.DOB.Split('-')[0];
                }

                if (data.Anniversarydate != null && data.Anniversarydate != "01-01-0100")
                {
                    Anniversary = data.Anniversarydate.Split('-')[2] + '-' + data.Anniversarydate.Split('-')[1] + '-' + data.Anniversarydate.Split('-')[0];
                }

                if (data.NextFollowDate != null && data.NextFollowDate != "01-01-0100")
                {
                    NextFollowDate = data.NextFollowDate.Split('-')[2] + '-' + data.NextFollowDate.Split('-')[1] + '-' + data.NextFollowDate.Split('-')[0];
                }

                
                string rtrnvalue = "";
                string Userid = Convert.ToString(Session["userid"]);
                ProcedureExecute proc = new ProcedureExecute("PRC_FTSInsertUpdateCRMContact");
                proc.AddPara("@ACTION", data.Action);
                proc.AddPara("@ShopCode", data.shop_code);
                proc.AddPara("@FirstName", data.FirstName);
                proc.AddPara("@LastName", data.LastName);
                proc.AddPara("@PhoneNo", data.PhoneNo);
                proc.AddPara("@Email", data.Email);
                proc.AddPara("@Address", data.Address);
                proc.AddPara("@DOB", DOB);
                proc.AddPara("@Anniversarydate", Anniversary);
                proc.AddPara("@JobTitle", data.JobTitle);
                proc.AddPara("@CompanyId", data.CompanyId);
                proc.AddPara("@AssignedTo", data.AssignedTo);
                proc.AddPara("@TypeId", data.TypeId);
                proc.AddPara("@StatusId", data.StatusId);
                proc.AddPara("@SourceId", data.SourceId);
                proc.AddPara("@ReferenceId", data.ReferenceId);
                proc.AddPara("@StageId", data.StageId);
                proc.AddPara("@Remarks", data.Remarks);
                proc.AddPara("@ExpSalesValue", data.ExpSalesValue);
                proc.AddPara("@NextFollowDate", NextFollowDate);
                proc.AddPara("@Active", data.Active);
                proc.AddPara("@user_id", data.user_id);
                proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                int k = proc.RunActionQuery();
                rtrnvalue = Convert.ToString(proc.GetParaValue("@RETURN_VALUE"));
                return Json(rtrnvalue, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public ActionResult EditCRMContact(String ShopCode)
        {
            try
            {
                AddCrmContactData ret = new AddCrmContactData();

                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_FTSInsertUpdateCRMContact");
                proc.AddPara("@ACTION", "EDITCRMCONTACT");
                proc.AddPara("@ShopCode", ShopCode);
                dt = proc.GetTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    ret.shop_code = dt.Rows[0]["shop_code"].ToString();
                    ret.FirstName = dt.Rows[0]["Shop_FirstName"].ToString();
                    ret.LastName = dt.Rows[0]["Shop_LastName"].ToString();
                    ret.PhoneNo = dt.Rows[0]["Shop_Owner_Contact"].ToString();
                    ret.Email = dt.Rows[0]["Shop_Owner_Email"].ToString();
                    ret.Address = dt.Rows[0]["Address"].ToString();
                    ret.DOB = dt.Rows[0]["DOB"].ToString();
                    ret.Anniversarydate = dt.Rows[0]["date_aniversary"].ToString();
                    ret.CompanyId = Convert.ToInt32(dt.Rows[0]["Shop_CRMCompID"].ToString());
                    ret.JobTitle = dt.Rows[0]["Shop_JobTitle"].ToString();
                    ret.AssignedTo = dt.Rows[0]["user_name"].ToString();
                    ret.AssignedToId = Convert.ToInt32(dt.Rows[0]["Shop_CreateUser"].ToString());
                    ret.TypeId = Convert.ToInt32(dt.Rows[0]["Shop_CRMTypeID"].ToString());
                    ret.StatusId = Convert.ToInt32(dt.Rows[0]["Shop_CRMStatusID"].ToString());
                    ret.SourceId = Convert.ToInt32(dt.Rows[0]["Shop_CRMSourceID"].ToString());
                    ret.ReferenceName = dt.Rows[0]["REFERENCE_NAME"].ToString();
                    ret.ReferenceId = dt.Rows[0]["Shop_CRMReferenceID"].ToString();
                    ret.StageId = Convert.ToInt32(dt.Rows[0]["Shop_CRMStageID"].ToString());
                    ret.Remarks = dt.Rows[0]["Remarks"].ToString();
                    ret.ExpSalesValue = Convert.ToDecimal(dt.Rows[0]["Amount"].ToString());
                    ret.NextFollowDate = dt.Rows[0]["Shop_NextFollowupDate"].ToString();
                    ret.Active = Convert.ToInt32(dt.Rows[0]["Entity_Status"]);
                    
                }
                return Json(ret, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        [HttpPost]
        public JsonResult DeleteCRMContact(string ShopCode)
        {
            string output_msg = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_FTSInsertUpdateCRMContact");
                proc.AddPara("@ACTION", "DELETECRMCONTACT");
                proc.AddPara("@ShopCode", ShopCode);
                dt = proc.GetTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    output_msg = dt.Rows[0]["MSG"].ToString();
                }
            }
            catch (Exception ex)
            {
                output_msg = "Please try again later";
            }

            return Json(output_msg, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExporRegisterList(int type)
        {
            switch (type)
            {
                case 1:
                    return GridViewExtension.ExportToPdf(GetDoctorBatchGridViewSettings(), GetCRMContactDetails(""));
                case 2:
                    return GridViewExtension.ExportToXlsx(GetDoctorBatchGridViewSettings(), GetCRMContactDetails(""));
                case 3:
                    return GridViewExtension.ExportToXls(GetDoctorBatchGridViewSettings(), GetCRMContactDetails(""));
                case 4:
                    return GridViewExtension.ExportToRtf(GetDoctorBatchGridViewSettings(), GetCRMContactDetails(""));
                case 5:
                    return GridViewExtension.ExportToCsv(GetDoctorBatchGridViewSettings(), GetCRMContactDetails(""));
                default:
                    break;
            }
            return null;
        }

        private GridViewSettings GetDoctorBatchGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gridCRMContact";
            settings.SettingsExport.ExportedRowType = GridViewExportedRowType.All;
            settings.SettingsExport.FileName = "Contact Details";

            settings.Columns.Add(x =>
            {
                x.FieldName = "SEQ";
                x.Caption = "Sr. No";
                x.VisibleIndex = 1;
                x.Width = 20;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_FirstName";
                x.Caption = "First Name";
                x.VisibleIndex = 2;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_LastName";
                x.Caption = "Last Name";
                x.VisibleIndex = 3;
                x.Width = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_Owner_Contact";
                x.Caption = "Phone";
                x.VisibleIndex = 4;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_Owner_Email";
                x.Caption = "Email";
                x.VisibleIndex = 5;
                x.Width = 150;
            });
            //Mantis Issue 24928
            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_Address";
                x.Caption = "Address";
                x.VisibleIndex = 6;
                x.Width = 250;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_DOB";
                x.Caption = "Date of Birth";
                x.VisibleIndex = 7;
                x.Width = 100;
                x.ColumnType = MVCxGridViewColumnType.DateEdit;
                x.PropertiesEdit.DisplayFormatString = "dd-MM-yyyy";
                (x.PropertiesEdit as DateEditProperties).EditFormatString = "dd-MM-yyyy";
            });
            //End of Mantis Issue 24928
            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_date_aniversary";
                x.Caption = "Date of Anniversary";
                x.VisibleIndex = 8;
                x.Width = 150;
                x.ColumnType = MVCxGridViewColumnType.DateEdit;
                x.PropertiesEdit.DisplayFormatString = "dd-MM-yyyy";
                (x.PropertiesEdit as DateEditProperties).EditFormatString = "dd-MM-yyyy";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_CompanyName";
                x.Caption = "Company";
                x.VisibleIndex = 9;
                x.Width = 150;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_JobTitle";
                x.Caption = "Job Title";
                x.VisibleIndex = 10;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_CreateUserName";
                x.Caption = "Assign To";
                x.VisibleIndex = 11;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_TypeName";
                x.Caption = "Type";
                x.VisibleIndex = 12;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_StatusName";
                x.Caption = "Status";
                x.VisibleIndex = 13;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_SourceName";
                x.Caption = "Source";
                x.VisibleIndex = 14;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_ReferenceName";
                x.Caption = "Reference";
                x.VisibleIndex = 15;
                x.Width = 150;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_StageName";
                x.Caption = "Stages";
                x.VisibleIndex = 16;
                x.Width = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_Remarks";
                x.Caption = "Remarks";
                x.VisibleIndex = 17;
                x.Width = 250;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_Amount";
                x.Caption = "Expected Sales Value";
                x.VisibleIndex = 18;
                x.Width = 100;
                x.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
                x.CellStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
                x.PropertiesEdit.DisplayFormatString = "0.00";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_NextFollowupDate";
                x.Caption = "Next follow Up Date";
                x.VisibleIndex = 19;
                x.Width = 200;
                x.ColumnType = MVCxGridViewColumnType.DateEdit;
                x.PropertiesEdit.DisplayFormatString = "dd-MM-yyyy";
                (x.PropertiesEdit as DateEditProperties).EditFormatString = "dd-MM-yyyy";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Shop_Entity_Status";
                x.Caption = "Active";
                x.VisibleIndex = 20;
                x.Width = 100;
            });

            settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            settings.SettingsExport.LeftMargin = 20;
            settings.SettingsExport.RightMargin = 20;
            settings.SettingsExport.TopMargin = 20;
            settings.SettingsExport.BottomMargin = 20;

            return settings;
        }

    }
}