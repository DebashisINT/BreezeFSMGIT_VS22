using DataAccessLayer;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using LMS.Models;
using MyShop.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using UtilityLayer;

namespace LMS.Areas.LMS.Controllers
{
    public class LMSTopicsController : Controller
    {
        // GET: LMS/Topics
        public ActionResult Index()
        {
            LMSTopicsModel Dtls = new LMSTopicsModel();
            
            EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/LMSTopics/Index");
            ViewBag.CanAdd = rights.CanAdd;
            ViewBag.CanView = rights.CanView;
            ViewBag.CanExport = rights.CanExport;
            ViewBag.CanEdit = rights.CanEdit;
            ViewBag.CanDelete = rights.CanDelete;

            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("PRC_LMSTOPICSMASTER");
            proc.AddPara("@ACTION", "GETDROPDOWNBINDDATA");
            ds = proc.GetDataSet();

            if (ds != null)
            {
                // Company
                List<TopicBasedOnList> TopicBasedOnList = new List<TopicBasedOnList>();
                TopicBasedOnList = APIHelperMethods.ToModelList<TopicBasedOnList>(ds.Tables[0]);
                Dtls.TopicBasedOnList = TopicBasedOnList;
            }

            return View(Dtls);
        }

        public ActionResult PartialTopicGridList(LMSTopicsModel model)
        {
            try
            {
                EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/LMSTopics/Index");
                ViewBag.CanAdd = rights.CanAdd;
                ViewBag.CanView = rights.CanView;
                ViewBag.CanExport = rights.CanExport;
                ViewBag.CanEdit = rights.CanEdit;
                ViewBag.CanDelete = rights.CanDelete;

                if (model.Is_PageLoad == "TotalTopics" || model.Is_PageLoad == "UsedTopics" || model.Is_PageLoad == "UnusedTopics")
                {
                    string Is_PageLoad = model.Is_PageLoad;

                    model.Is_PageLoad = "Ispageload";

                    return PartialView("PartialTopicGridList", GetTopicDetails(Is_PageLoad));
                }
                else
                {
                    string Is_PageLoad = string.Empty;

                    if (model.Is_PageLoad == "Ispageload")
                    {
                        Is_PageLoad = "is_pageload";

                    }


                    GetTopicListing(Is_PageLoad);

                    model.Is_PageLoad = "Ispageload";

                    return PartialView("PartialTopicGridList", GetTopicDetails(Is_PageLoad));
                }
                

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        public void GetTopicListing(string Is_PageLoad)
        {
            string user_id = Convert.ToString(Session["userid"]);

            string action = string.Empty;
            DataTable formula_dtls = new DataTable();
            DataSet dsInst = new DataSet();

            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSTOPICSMASTER");
                proc.AddPara("@ACTION", "GETLISTINGDATA");
                proc.AddPara("@IS_PAGELOAD", Is_PageLoad);
                proc.AddPara("@USERID", Convert.ToInt32(user_id));
                dt = proc.GetTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable GetTopicDetails(string Is_PageLoad)
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
                if (Is_PageLoad == "UsedTopics")
                {
                    LMSMasterDataContext dc = new LMSMasterDataContext(connectionString);
                    var q = from d in dc.LMS_TOPICSMASTER_LISTINGs
                            where d.USERID == Convert.ToInt32(Userid) && d.QUESTIONS_ID != 0
                            orderby d.SEQ
                            select d;
                    return q;
                }
                else if (Is_PageLoad == "UnusedTopics")
                {
                    LMSMasterDataContext dc = new LMSMasterDataContext(connectionString);
                    var q = from d in dc.LMS_TOPICSMASTER_LISTINGs
                            where d.USERID == Convert.ToInt32(Userid) && d.QUESTIONS_ID == 0
                            orderby d.SEQ
                            select d;
                    return q;
                }
                else
                {
                    LMSMasterDataContext dc = new LMSMasterDataContext(connectionString);
                    var q = from d in dc.LMS_TOPICSMASTER_LISTINGs
                            where d.USERID == Convert.ToInt32(Userid)
                            orderby d.SEQ
                            select d;
                    return q;
                }
                
            }
            else
            {
                LMSMasterDataContext dc = new LMSMasterDataContext(connectionString);
                var q = from d in dc.LMS_TOPICSMASTER_LISTINGs
                        where d.USERID == Convert.ToInt32(Userid) && d.SEQ == 1111119
                        orderby d.SEQ 
                        select d;
                return q;
            }


        }

        public JsonResult GetTopicCount()
        {


            LMSTopicsModel dtl = new LMSTopicsModel();
            try
            {
                DataSet ds = new DataSet();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSTOPICSMASTER");
                proc.AddPara("@Action", "GETTOPICSCOUNTDATA");
                proc.AddPara("@userid", Convert.ToString(HttpContext.Session["userid"]));
                ds = proc.GetDataSet();


                int TotalTopics = 0;
                int TotalUsedTopics = 0;
                int TotalUnusedTopics = 0;

                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    TotalTopics = Convert.ToInt32(item["cnt_TotalTopics"]);
                    TotalUsedTopics = Convert.ToInt32(item["cnt_TotalUsedTopics"]);
                    TotalUnusedTopics = Convert.ToInt32(item["cnt_TotalUnusedTopics"]);

                }

                dtl.TotalTopics = TotalTopics;
                dtl.TotalUsedTopics = TotalUsedTopics;
                dtl.TotalUnusedTopics = TotalUnusedTopics;

            }
            catch
            {
            }
            return Json(dtl);
        }

        public ActionResult ExporRegisterList()
        {
            return GridViewExtension.ExportToXlsx(GetDoctorBatchGridViewSettings(), GetTopicDetails(""));
        }

        private GridViewSettings GetDoctorBatchGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gridLMSTopic";
            settings.SettingsExport.ExportedRowType = GridViewExportedRowType.All;
            settings.SettingsExport.FileName = "LMSTopics";

            settings.Columns.Add(x =>
            {
                x.FieldName = "SEQ";
                x.Caption = "Sr. No";
                x.VisibleIndex = 1;
                x.ExportWidth = 80;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "TOPICNAME";
                x.Caption = "Topic Name";
                x.VisibleIndex = 2;
                x.ExportWidth = 350;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "TOPICBASEDON";
                x.Caption = "Based On";
                x.VisibleIndex = 3;
                x.ExportWidth = 150;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "TOPICSTATUS";
                x.Caption = "Active";
                x.VisibleIndex = 4;
                x.ExportWidth = 150;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "CREATEDBY";
                x.Caption = "Created by";
                x.VisibleIndex = 5;
                x.ExportWidth = 150;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "CREATEDON";
                x.Caption = "Created on";
                x.VisibleIndex = 6;
                x.ExportWidth = 150;
                x.PropertiesEdit.DisplayFormatString = "dd-MM-yyyy";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "UPDATEDBY";
                x.Caption = "Modified by";
                x.VisibleIndex = 7;
                x.ExportWidth = 150;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "UPDATEDON";
                x.Caption = "Modified on";
                x.VisibleIndex = 8;
                x.ExportWidth = 150;
                x.PropertiesEdit.DisplayFormatString = "dd-MM-yyyy";
            });

            settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            settings.SettingsExport.LeftMargin = 20;
            settings.SettingsExport.RightMargin = 20;
            settings.SettingsExport.TopMargin = 20;
            settings.SettingsExport.BottomMargin = 20;

            return settings;

        }

        public JsonResult GetBasedOnList(string topic_basedon)
        {
            LMSTopicsModel model = new LMSTopicsModel();
            List<TopicMapList> TopicMapList1 = new List<TopicMapList>();

            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_LMSTOPICSMASTER");
            proc.AddPara("@Action", "GETBASEDONDATALIST");
            proc.AddPara("@TOPICBASEDON_ID", topic_basedon);
            proc.AddPara("@BRANCHID", Convert.ToString(Session["userbranchHierarchy"]));
            proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));

            dt = proc.GetTable();

            if (dt != null)
            {
                TopicMapList1 = APIHelperMethods.ToModelList<TopicMapList>(dt);
            }

            return Json(TopicMapList1, JsonRequestBehavior.AllowGet); 
        }

        [HttpPost]
        public ActionResult SaveTopic(LMSTopicsModel data)
        {
            try
            {
                //string rtrnduplicatevalue = "";
                //string Userid = Convert.ToString(Session["userid"]);
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSTOPICSMASTER");
                proc.AddPara("@ACTION", data.Action);
                proc.AddPara("@TOPICID", data.TopicID);
                proc.AddPara("@TOPICNAME", data.TopicName);
                proc.AddPara("@TOPICSTATUS", data.TopicStatus);
                proc.AddPara("@TOPICBASEDON_ID", data.TopicBasedOnId);
                proc.AddPara("@SELECTEDTOPICBASEDONMAPLIST", data.selectedTopicBasedOnMapList);
                proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));
                proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                proc.AddVarcharPara("@RETURN_DUPLICATEMAPNAME", -1, "", QueryParameterDirection.Output);
                int k = proc.RunActionQuery();
                data.RETURN_VALUE = Convert.ToString(proc.GetParaValue("@RETURN_VALUE")); // WILL RETURN NEW TOPIC ID
                data.RETURN_DUPLICATEMAPNAME = Convert.ToString(proc.GetParaValue("@RETURN_DUPLICATEMAPNAME"));

                // Send Notification
                if ( (data.Action=="ADDTOPIC" && data.TopicStatus == "true") || (data.TopicStatus=="true" && data.TopicStatus!=data.TopicStatusOld) )  // If published
                {
                    FireNotification(data.TopicName, data.RETURN_VALUE);
                }

                if (data.Action == "ADDTOPIC" && Convert.ToInt16(data.RETURN_VALUE) > 0)
                {
                    data.RETURN_VALUE = "1";
                }
                // End of Send Notification

                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        // Send Notification
        public void FireNotification(string TopicTitle, string TopicId)
        {
            string Mssg = "HI! A new Topic " + TopicTitle + " has been assigned to you. Please check your learning dashboard to start watching.";
            var imgNotification_Icon = Server.MapPath("~/Commonfolder/LMS/Notification_Icon.jpg");
            //string SalesMan_Nm = "";
            string SalesMan_Phn = "";

            //DataTable dt_SalesMan = odbengine.GetDataTable("select user_loginId,user_name from tbl_master_user  where user_id=" + SalesmanId + "");
            DataTable dtAssignUser = new DataTable();

            ProcedureExecute procA = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            procA.AddPara("@ACTION", "GETCONTENTASSIGNUSER");
            procA.AddPara("@TOPICID", TopicId);
            procA.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));
            dtAssignUser = procA.GetTable();

            if (dtAssignUser.Rows.Count > 0)
            {
                SalesMan_Phn = dtAssignUser.Rows[0]["user_loginId"].ToString();

                SendNotification(SalesMan_Phn, Mssg, imgNotification_Icon);
            }
        }

        public JsonResult SendNotification(string Mobiles, string messagetext, string imgNotification_Icon)
        {

            string status = string.Empty;
            try
            {
                //int returnmssge = notificationbl.Savenotification(Mobiles, messagetext);

                int s = 0;
                ProcedureExecute proc = new ProcedureExecute("Proc_FCM_NotificationManage");
                proc.AddPara("@Mobiles", Mobiles);
                proc.AddPara("@Message", messagetext);
                s = proc.RunActionQuery();

                //DataTable dt = odbengine.GetDataTable("select device_token,musr.user_name,musr.user_id  from tbl_master_user as musr inner join tbl_FTS_devicetoken as token on musr.user_id=token.UserID  where musr.user_loginId in (select items from dbo.SplitString('" + Mobiles + "',',')) and musr.user_inactive='N'");
                //DataTable dt = odbengine.GetDataTable("select device_token,musr.user_name,musr.user_id  from tbl_master_user as musr inner join tbl_FTS_devicetoken as token on musr.user_id=token.UserID  where musr.user_loginId in (" + Mobiles + ") and musr.user_inactive='N'");

                Mobiles = Mobiles.Replace("'", "");

                DataTable dt = new DataTable();
                ProcedureExecute procA = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                procA.AddPara("@ACTION", "GETDEVICETOKENINFO");
                procA.AddPara("@MOBILES", Mobiles);
                dt = procA.GetTable();

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (Convert.ToString(dt.Rows[i]["device_token"]) != "")
                        {
                            SendPushNotification(messagetext, Convert.ToString(dt.Rows[i]["device_token"]), Convert.ToString(dt.Rows[i]["user_name"]), Convert.ToString(dt.Rows[i]["user_id"]), imgNotification_Icon);

                        }
                    }
                    status = "200";
                }


                else
                {

                    status = "202";
                }



                return Json(status, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                status = "300";
                return Json(status, JsonRequestBehavior.AllowGet);
            }
        }

        public static void SendPushNotification(string message, string deviceid, string Customer, string Requesttype, string imgNotification_Icon)
        {
            try
            {
                //string applicationID = "AAAAS0O97Kk:APA91bH8_KgkJzglOUHC1ZcMEQFjQu8fsj1HBKqmyFf-FU_I_GLtXL_BFUytUjhlfbKvZFX9rb84PWjs05HNU1QyvKy_TJBx7nF70IdIHBMkPgSefwTRyDj59yXz4iiKLxMiXJ7vel8B";
                //string senderId = "323259067561";
                string applicationID = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["AppID"]);
                string senderId = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SenderID"]);
                //string senderId = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["SenderID"]);
                string deviceId = deviceid;
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var data2 = new
                {
                    to = deviceId,

                    data = new
                    {
                        UserName = Customer,
                        UserID = Requesttype,
                        body = message,
                        type = "lms_content_assign",
                        imgNotification_Icon = imgNotification_Icon
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data2);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;
                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string str = ex.Message;
            }
        }
        // End of Send Notification

        public ActionResult ShowTopicDetails(String TopicID)
        {
            try
            {
                DataTable dt = new DataTable();
                LMSTopicsModel ret = new LMSTopicsModel();
                List<TopicMapList> TopicMapList1 = new List<TopicMapList>();

                ProcedureExecute proc = new ProcedureExecute("PRC_LMSTOPICSMASTER");
                proc.AddPara("@ACTION", "SHOWTOPIC");
                proc.AddPara("@TOPICID", TopicID);
                proc.AddPara("@BRANCHID", Convert.ToString(Session["userbranchHierarchy"]));
                proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));
                dt = proc.GetTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    TopicMapList1 = APIHelperMethods.ToModelList<TopicMapList>(dt);
                }

                return Json(TopicMapList1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        [HttpPost]
        public JsonResult TopicDelete(string TopicID)
        {
            string output_msg = string.Empty;
           
            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSTOPICSMASTER");
                proc.AddPara("@ACTION", "DELETETOPICS");
                proc.AddPara("@TOPICID", TopicID);
                proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                dt = proc.GetTable();
                output_msg = Convert.ToString(proc.GetParaValue("@RETURN_VALUE"));
            }
            catch (Exception ex)
            {
                output_msg = "Please try again later";
            }

            return Json(output_msg, JsonRequestBehavior.AllowGet);
        }
    }
}