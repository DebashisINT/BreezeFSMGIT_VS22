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

                // Reference
                List<ReferenceList> ReferenceList = new List<ReferenceList>();
                ReferenceList = APIHelperMethods.ToModelList<ReferenceList>(ds.Tables[5]);
                Dtls.ReferenceList = ReferenceList;

                // Stage
                List<StageList> StageList = new List<StageList>();
                StageList = APIHelperMethods.ToModelList<StageList>(ds.Tables[6]);
                Dtls.StageList = StageList;


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
                        orderby d.SEQ
                        select d;
                return q;
            }
            else
            {
                ReportsDataContext dc = new ReportsDataContext(connectionString);
                var q = from d in dc.CRM_CONTACT_LISTINGs
                        where d.USERID == Convert.ToInt32(Userid) && d.SEQ == 11111111119
                        orderby d.SEQ
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
                proc.AddPara("@ShopID", data.shop_id);
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

        public ActionResult EditCRMContact(String ShopID)
        {
            try
            {
                AddCrmContactData ret = new AddCrmContactData();

                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_FTSInsertUpdateCRMContact");
                proc.AddPara("@ACTION", "EDITCRMCONTACT");
                proc.AddPara("@ShopID", ShopID);
                dt = proc.GetTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    ret.shop_id = Convert.ToInt32(dt.Rows[0]["Shop_ID"].ToString());
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
                    ret.ReferenceId = Convert.ToInt32(dt.Rows[0]["Shop_CRMReferenceID"].ToString());
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
        public JsonResult DeleteCRMContact(string ShopId)
        {
            string output_msg = string.Empty;
            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_FTSInsertUpdateCRMContact");
                proc.AddPara("@ACTION", "DELETECRMCONTACT");
                proc.AddPara("@ShopID", ShopId);
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

    }
}