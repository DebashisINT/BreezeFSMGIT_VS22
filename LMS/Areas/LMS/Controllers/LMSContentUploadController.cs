using BusinessLogicLayer;
using DataAccessLayer;
using DevExpress.Web.Mvc;
using DevExpress.Web;
using LMS.Models;
using Models;
using MyShop.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using UtilityLayer;
using DevExpress.Charts.Native;
using DevExpress.Data.XtraReports.Wizard.Presenters;
using NReco.VideoConverter;
using DevExpress.Utils.Drawing.Helpers;
using System.Net;
using System.Web.Script.Serialization;
using System.Text;

namespace LMS.Areas.LMS.Controllers
{
    public class LMSContentUploadController : Controller
    {
       
        DBEngine odbengine = new DBEngine();


        // GET: LMS/ContentUpload
        public ActionResult Index()
        {
            //List<VideoFiles> videolist = new List<VideoFiles>();

            //string CS = System.Configuration.ConfigurationSettings.AppSettings["DBConnectionDefault"];
            //using (SqlConnection con = new SqlConnection(CS))
            //{
            //    SqlCommand cmd = new SqlCommand("PRC_LMSCONTENTMASTER", con);
            //    cmd.Parameters.AddWithValue("@ACTION", "GETLISTINGDATA");
            //    cmd.CommandType = CommandType.StoredProcedure;
            //    con.Open();
            //    SqlDataReader rdr = cmd.ExecuteReader();
            //    while (rdr.Read())
            //    {
            //        VideoFiles video = new VideoFiles();
            //        video.ID = Convert.ToInt32(rdr["ID"]);
            //        video.Name = rdr["Name"].ToString();
            //        video.FileSize = Convert.ToInt32(rdr["FileSize"]);
            //        video.FilePath = rdr["FilePath"].ToString();
            //        video.Filetype = rdr["FileType"].ToString();
            //        video.FileDescription = rdr["FileDescription"].ToString();
            //        video.FilePathIcon = Convert.ToString(rdr["FilePathIcon"]);
            //        video.IsActive = Convert.ToString(rdr["IsActive"]);
            //        videolist.Add(video);
            //    }
            //}

            LMSContentModel Dtls = new LMSContentModel();

            EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/LMSContentUpload/Index");
            ViewBag.CanAdd = rights.CanAdd;
            ViewBag.CanView = rights.CanView;
            ViewBag.CanExport = rights.CanExport;
            ViewBag.CanEdit = rights.CanEdit;
            ViewBag.CanDelete = rights.CanDelete;

            DBEngine obj1 = new DBEngine();
            ViewBag.LMSVideoUploadSize = Convert.ToString(obj1.GetDataTable("select [value] from FTS_APP_CONFIG_SETTINGS WHERE [Key]='LMSVideoUploadSize'").Rows[0][0]);

            //List<TopicList> modelTopic = new List<TopicList>();

            DataSet ds = new DataSet();

            //ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            //proc.AddPara("@ACTION", "GETDROPDOWNBINDDATA");
            //ds = proc.GetDataSet();

            //if (ds != null)
            //{
            //    // Company
            //    List<TopicList> TopicList = new List<TopicList>();
            //    TopicList = APIHelperMethods.ToModelList<TopicList>(ds.Tables[0]);
            //    Dtls.TopicList = TopicList;
            //}

            DataTable dt = new DataTable();
            dt = GetTopicListData();

            if (dt != null)
            {
                List<TopicList> TopicList = new List<TopicList>();
                TopicList = APIHelperMethods.ToModelList<TopicList>(dt);
                Dtls.TopicList = TopicList;
            }



            ProcedureExecute proc1 = new ProcedureExecute("PRC_LMSTOPICSMASTER");
            proc1.AddPara("@ACTION", "GETDROPDOWNBINDDATA");
            ds = proc1.GetDataSet();

            if (ds != null)
            {
                // Company
                List<TopicBasedOnList> TopicBasedOnList = new List<TopicBasedOnList>();
                TopicBasedOnList = APIHelperMethods.ToModelList<TopicBasedOnList>(ds.Tables[0]);
                Dtls.TopicBasedOnList = TopicBasedOnList;
            }


            return View(Dtls);
        }

        public DataTable GetTopicListData()
        {
            DataTable dt = new DataTable();

            ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            proc.AddPara("@ACTION", "GETDROPDOWNBINDDATA");
            dt = proc.GetTable();
            return dt;
        }

        public JsonResult GetTopicList()
        {
            LMSContentModel Dtls = new LMSContentModel();
            DataTable dt = new DataTable();
            dt = GetTopicListData();

            if (dt != null)
            {
                // Company
                List<TopicList> TopicList = new List<TopicList>();
                TopicList = APIHelperMethods.ToModelList<TopicList>(dt);
                Dtls.TopicList = TopicList;
            }
            return Json(Dtls.TopicList, JsonRequestBehavior.AllowGet);
        }

        
        //public ActionResult GetContentListing()
        //{
        //    List<VideoFiles> videolist = new List<VideoFiles>();

        //    string CS = System.Configuration.ConfigurationSettings.AppSettings["DBConnectionDefault"];
        //    using (SqlConnection con = new SqlConnection(CS))
        //    {
        //        SqlCommand cmd = new SqlCommand("PRC_LMSCONTENTMASTER", con);
        //        cmd.Parameters.AddWithValue("@ACTION", "GETLISTINGDATA");
        //        cmd.CommandType = CommandType.StoredProcedure;
        //        con.Open();
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            VideoFiles video = new VideoFiles();
        //            video.ID = Convert.ToInt32(rdr["ID"]);
        //            video.Name = rdr["Name"].ToString();
        //            video.FileSize = Convert.ToInt32(rdr["FileSize"]);
        //            video.FilePath = rdr["FilePath"].ToString();
        //            video.Filetype = rdr["FileType"].ToString();
        //            video.FileDescription = rdr["FileDescription"].ToString();
        //            video.FilePathIcon = Convert.ToString(rdr["FilePathIcon"]);
        //            video.IsActive = Convert.ToString(rdr["IsActive"]);
        //            videolist.Add(video);
        //        }
        //    }

        //    return View(videolist);
        //}

        public JsonResult GetContentGridListData(string TopicID)
        {
            DataTable dt = new DataTable();
            //  LMSContentAddModel ret = new LMSContentAddModel();
            List<ContentListingData> ContentList1 = new List<ContentListingData>();

            ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            proc.AddPara("@ACTION", "GETLISTINGDATA");
            proc.AddPara("@BRANCHID", Convert.ToString(Session["userbranchHierarchy"]));
            proc.AddPara("@TOPICID", TopicID);
            proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));
            dt = proc.GetTable();

            if (dt != null && dt.Rows.Count > 0)
            {
                ContentList1 = APIHelperMethods.ToModelList<ContentListingData>(dt);
            }

            return Json(ContentList1, JsonRequestBehavior.AllowGet);


        }

        public JsonResult GetTopicListForBoxData()
        {
            List<TopicListForBoxData> TopicList = new List<TopicListForBoxData>();
            DataTable dt = new DataTable();


            ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            proc.AddPara("@ACTION", "GETTOPICLISTFORBOXDATA");
            dt = proc.GetTable();

            if (dt != null)
            {
                // Company
                
                TopicList = APIHelperMethods.ToModelList<TopicListForBoxData>(dt);
                
            }
            return Json(TopicList, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetLastPlaySeq(string topicid)
        {
            LMSContentEditModel model = new LMSContentEditModel();
           
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            proc.AddPara("@Action", "GETLASTPLAYSEQ");
            proc.AddPara("@TOPICID", topicid);
            dt = proc.GetTable();

            if (dt != null && dt.Rows.Count>0 )
            {
                model.PlaySequence = Convert.ToString(dt.Rows[0]["PlaySequence"]);
            }

            return Json(model, JsonRequestBehavior.AllowGet);
        }

        

        [HttpPost]
        //public ActionResult SaveContent(HttpPostedFileBase fileupload, string hdnAddEditMode, string hdnContentID, string hdnFileDuration,
        //            string txtContentTitle, string txtContentDesc, string numPlaySequence, string hdnTopicID,
        //            string chkStatus, string chkAllowLike , string chkAllowComments, string chkAllowShare)
        public ActionResult SaveContent(HttpPostedFileBase fileupload, HttpPostedFileBase fileuploadicon, string hdnAddEditMode, string hdnContentID, string hdnFileDuration,
                    string txtContentTitle, string txtContentDesc, string numPlaySequence, LMSContentModel data,
                    string chkStatus, string chkAllowLike, string chkAllowComments, string chkAllowShare)
        {
            try
            {
                string RETURN_VALUE = string.Empty;
                string RETURN_DUPLICATEMAPNAME = string.Empty;
                string RETURN_CONTENTID = string.Empty;
                int IsValid = 1;

                if (chkStatus != null && chkStatus == "on")
                {
                    chkStatus = "1";
                }
                else { 
                    chkStatus = "0"; 
                }


                if (chkAllowLike != null && chkAllowLike == "on")
                {
                    chkAllowLike = "1";
                }
                else
                {
                    chkAllowLike = "0";
                }

                if (chkAllowComments != null && chkAllowComments == "on")
                {
                    chkAllowComments = "1";
                }
                else
                {
                    chkAllowComments = "0";
                }

                if (chkAllowShare != null && chkAllowShare == "on")
                {
                    chkAllowShare = "1";
                }
                else
                {
                    chkAllowShare = "0";
                }

                var allowedExtensions = new[] {".mp4"};
                var allowedExtensionsicon = new[] { ".jpg"};

                string fileName = "";
                int fileSize = 0;
                //int Size = fileSize / 1000;
                string FileType = "";

                string fileNameicon = "";
                int fileSizeicon = 0;
                //int Size = fileSize / 1000;
                string FileTypeicon = "";

                var _thumbnailPath = "";

                DBEngine obj1 = new DBEngine();
                var LMSVideoUploadSize = Convert.ToString(obj1.GetDataTable("select [value] from FTS_APP_CONFIG_SETTINGS WHERE [Key]='LMSVideoUploadSize'").Rows[0][0]);

                //LMSContentAddModel data = new LMSContentAddModel();

                if (fileupload != null)
                {
                    fileName = Path.GetFileName(fileupload.FileName);
                    fileSize = fileupload.ContentLength;
                    //int Size = fileSize / 1000;
                    FileType = System.IO.Path.GetExtension(fileName);


                    if (!allowedExtensions.Contains(FileType.ToLower()))
                    {
                        IsValid = 0;
                        RETURN_VALUE = "Invalid file. Only .mp4 types of files shall be supported.";
                    }
                    else
                    {
                        if (fileSize > (Convert.ToDecimal(LMSVideoUploadSize) * 1024 * 1024))
                        {
                            IsValid = 0;
                            RETURN_VALUE = "Maximum file size shall be " + LMSVideoUploadSize + " MB.";
                        }
                        else
                        {
                            IsValid = 1;
                            
                        }
                    }

                    if (IsValid == 1 && fileuploadicon != null)
                    {

                        fileNameicon = Path.GetFileName(fileuploadicon.FileName);
                        fileSizeicon = fileuploadicon.ContentLength;
                        //int Size = fileSize / 1000;
                        FileTypeicon = System.IO.Path.GetExtension(fileNameicon);


                        if (!allowedExtensionsicon.Contains(FileTypeicon.ToLower()))
                        {
                            IsValid = 0;
                            RETURN_VALUE = "Invalid thumbnail file. Only .jpg types of files shall be supported.";
                        }
                        else
                        {
                            if (fileSizeicon > (1024 * 1024)) 
                            {
                                IsValid = 0;
                                RETURN_VALUE = "Maximum thumbnail file size shall be 1MB.";
                            }
                            else
                            {
                                IsValid = 1;

                            }
                        }
                    }

                }
                else
                {
                    if (hdnAddEditMode == "ADDCONTENT")
                    {
                        IsValid = 0;
                        RETURN_VALUE = "Please select file.";
                    }
                    else if (hdnAddEditMode == "EDITCONTENT")
                    {
                        IsValid = 1;
                        hdnFileDuration = "0";
                    }
                    
                }

                if (hdnFileDuration == "")
                {
                    hdnFileDuration = "0";
                }

                if (IsValid == 1)
                {
                    if (fileName != null && fileName != "")
                    {
                        if (!System.IO.Directory.Exists(Server.MapPath("~/Commonfolder/LMS/ContentUpload/")))
                        {
                            // If Folder doesnot exists, CREATE the folder
                            System.IO.Directory.CreateDirectory(Server.MapPath("~/Commonfolder/LMS/ContentUpload/"));
                        }
                        else if (System.IO.File.Exists(Server.MapPath("~/Commonfolder/LMS/ContentUpload/" + fileName)))
                        {
                            // If file name already exists, RENAME the file by concatinating it by datetime
                            fileName = DateTime.Now.ToString("hhmmss") + fileName;
                        }

                        
                    }


                    if (!System.IO.Directory.Exists(Server.MapPath("~/Commonfolder/LMS/Thumbnails/")))
                    {
                        // If Folder doesnot exists, CREATE the folder
                        System.IO.Directory.CreateDirectory(Server.MapPath("~/Commonfolder/LMS/Thumbnails/"));
                    }
                    else if (System.IO.File.Exists(Server.MapPath("~/Commonfolder/LMS/Thumbnails/" + fileNameicon)))
                    {
                        // If thumbnail file name already exists, RENAME the file by concatinating it by datetime
                        fileNameicon = DateTime.Now.ToString("hhmmss") + fileNameicon;
                    }

                    // var _thumbnailPath = Path.Combine("~/Commonfolder/LMS/Thumbnails/", Path.GetFileNameWithoutExtension(fileName.Replace(' ','_')) + ".jpg");

                    if (fileNameicon != "")
                    {
                        _thumbnailPath = Path.Combine("~/Commonfolder/LMS/Thumbnails/", Path.GetFileName(fileNameicon.Replace(' ', '_')) );
                    }
                    else
                    {
                        _thumbnailPath = Path.Combine("~/Commonfolder/LMS/Thumbnails/", Path.GetFileNameWithoutExtension(fileName.Replace(' ', '_')) + ".jpg");
                    }
                    


                    ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                    proc.AddPara("@ACTION", hdnAddEditMode);
                    proc.AddPara("@CONTENTID", hdnContentID);
                    proc.AddPara("@CONTENTTITLE", txtContentTitle);
                    proc.AddPara("@CONTENTDESC", txtContentDesc);
                    proc.AddPara("@TOPICID", data.TopicId);
                    proc.AddPara("@PLAYSEQUENCE", numPlaySequence);
                    proc.AddPara("@STATUS", chkStatus);
                    proc.AddPara("@ALLOWLIKE", chkAllowLike);
                    proc.AddPara("@ALLOWCOMMENTS", chkAllowComments);
                    proc.AddPara("@ALLOWSHARE", chkAllowShare);
                    proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));

                    proc.AddPara("@CONTENT_FILENAME", fileName);
                    proc.AddPara("@CONTENT_FILESIZE", fileSize);
                    proc.AddPara("@CONTENT_FILETYPE", FileType);
                    proc.AddPara("@CONTENT_FILEDURATION", hdnFileDuration);
                    proc.AddPara("@CONTENT_FILEPATH", "~/Commonfolder/LMS/ContentUpload/" + fileName);                    
                    proc.AddPara("@CONTENT_ICONFILEPATH", _thumbnailPath);
                    proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                    proc.AddVarcharPara("@RETURN_DUPLICATEMAPNAME", -1, "", QueryParameterDirection.Output);
                    proc.AddVarcharPara("@RETURN_CONTENTID", 500, "", QueryParameterDirection.Output);
                    proc.AddVarcharPara("@RETURN_ASSIGNUSERIDS", -1, "", QueryParameterDirection.Output);
                    int k = proc.RunActionQuery();
                    RETURN_VALUE = Convert.ToString(proc.GetParaValue("@RETURN_VALUE"));
                    RETURN_CONTENTID = Convert.ToString(proc.GetParaValue("@RETURN_CONTENTID"));
                    //RETURN_DUPLICATEMAPNAME = Convert.ToString(proc.GetParaValue("@RETURN_DUPLICATEMAPNAME"));

                    if (RETURN_VALUE == "Content added succesfully." || RETURN_VALUE == "Content updated succesfully."){
                        if (fileName != null && fileName != "")
                        {
                            //fileupload.SaveAs(Server.MapPath("~/Commonfolder/LMS/ContentUpload/" + fileName));

                            string uploadsFolder = Server.MapPath("~/Commonfolder/LMS/ContentUpload/");
                            //Directory.CreateDirectory(uploadsFolder);

                            string originalFilePath = Path.Combine(uploadsFolder, "org_" + Path.GetFileName(fileName));
                            fileupload.SaveAs(originalFilePath);


                            string compressedFilePath = Path.Combine(uploadsFolder, Path.GetFileName(fileName));

                            var ffMpegC = new FFMpegConverter();
                            ffMpegC.ConvertMedia(originalFilePath, compressedFilePath, "mp4");

                            if (System.IO.File.Exists(Server.MapPath("~/Commonfolder/LMS/ContentUpload/" + "org_" + fileName)))
                            {
                                System.IO.File.Delete(Server.MapPath("~/Commonfolder/LMS/ContentUpload/" + "org_" + fileName));

                            }


                            
                            //Thumbnails Image save
                            //if (!System.IO.Directory.Exists(Server.MapPath("~/Commonfolder/LMS/Thumbnails/")))
                            //{
                            //    // If Folder doesnot exists, CREATE the folder
                            //    System.IO.Directory.CreateDirectory(Server.MapPath("~/Commonfolder/LMS/Thumbnails/"));
                            //}

                            if (fileNameicon != "")
                            {
                                // Upload thumbnail
                                var thumbnailPath = Path.Combine(Server.MapPath("~/Commonfolder/LMS/Thumbnails"), Path.GetFileName(fileNameicon.Replace(' ', '_')) );
                                fileuploadicon.SaveAs(thumbnailPath);
                            }
                            else
                            {
                                // Auto generate Thumbnail
                                var videoPath = Server.MapPath("~/Commonfolder/LMS/ContentUpload/" + fileName);
                                var thumbnailPath = Path.Combine(Server.MapPath("~/Commonfolder/LMS/Thumbnails"), Path.GetFileNameWithoutExtension(fileName.Replace(' ', '_')) + ".jpg");
                                var ffMpeg = new NReco.VideoConverter.FFMpegConverter();
                                ffMpeg.GetVideoThumbnail(videoPath, thumbnailPath);
                            }

                            //Thumbnails Image save End

                        }

                        // Send Notification
                        if (chkStatus == "1")  // If published
                        {
                           // FireNotification(txtContentTitle, Convert.ToString(data.TopicId));
                        }
                        // End of Send Notification
                    }
                }

                TempData["result"] = RETURN_VALUE+"~"+ RETURN_CONTENTID;

                return Json(TempData["result"], JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }


        // Send Notification
        public void FireNotification(string ContentTitle, string TopicId)
        {
            string Mssg = "HI! A new video titled " + ContentTitle + " has been assigned to you. Please check your learning dashboard to start watching.";
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

        public ActionResult ShowContentDetails(String ContentId)
        {
            try
            {
                DataTable dt = new DataTable();
              //  LMSContentAddModel ret = new LMSContentAddModel();
                List<LMSContentEditModel> TopicMapList1 = new List<LMSContentEditModel>();

                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "SHOWCONTENT");
                proc.AddPara("@CONTENTID", ContentId);
                proc.AddPara("@SERVERMAPPATH", Server.MapPath("~/Commonfolder/LMS/ContentUpload/"));
                proc.AddPara("@BRANCHID", Convert.ToString(Session["userbranchHierarchy"]));
                proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));
                dt = proc.GetTable();

                if (dt != null && dt.Rows.Count > 0)
                {
                    TopicMapList1 = APIHelperMethods.ToModelList<LMSContentEditModel>(dt);
                }

                //TopicMapList1.VideoPath = Url.Content("~/App_Data/Uploads/" + fileName);

               


                return Json(TopicMapList1, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public ActionResult Play(string fileName)
        {
            var filePath = Path.Combine(Server.MapPath(""), fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            return File(filePath, "video/mp4");
        }

        [HttpPost]
        public JsonResult DeleteContent(string ContentId)
        {
            string output_msg = string.Empty;

            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "DELETECONTENTS");
                proc.AddPara("@CONTENTID", ContentId);
                proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                dt = proc.GetTable();
                output_msg = Convert.ToString(proc.GetParaValue("@RETURN_VALUE"));


                if (output_msg != "-10" && output_msg != null && output_msg != "")
                {
                    string fileName = output_msg;

                    if (System.IO.File.Exists(Server.MapPath("~/Commonfolder/LMS/ContentUpload/" + fileName)))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Commonfolder/LMS/ContentUpload/" + fileName));

                    }

                    //REV thumbnail DELETE
                    var thumbnailPath = Path.GetFileNameWithoutExtension(fileName) + ".jpg";
                    if (System.IO.File.Exists(Server.MapPath("~/Commonfolder/LMS/Thumbnails/" + thumbnailPath)))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Commonfolder/LMS/Thumbnails/" + thumbnailPath));

                    }
                    //REV thumbnail DELETE END


                    output_msg = "1";
                }

            }
            catch (Exception ex)
            {
                output_msg = "Please try again later";
            }

            return Json(output_msg, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetContentCount()
        {
            LMSContentModel dtl = new LMSContentModel();
            try
            {
                DataSet ds = new DataSet();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@Action", "GETCONTENTCOUNTDATA");
                proc.AddPara("@userid", Convert.ToString(HttpContext.Session["userid"]));
                ds = proc.GetDataSet();

                int TotalContents = 0;
                int ActiveContents = 0;
                int InactiveContents = 0;
                
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    TotalContents = Convert.ToInt32(item["cnt_TotalContents"]);
                    ActiveContents = Convert.ToInt32(item["cnt_ActiveContents"]);
                    InactiveContents = Convert.ToInt32(item["cnt_InactiveContents"]);
                }

                dtl.TotalContents = TotalContents;
                dtl.ActiveContents = ActiveContents;
                dtl.InactiveContents = InactiveContents;
                
            }
            catch
            {
            }
            return Json(dtl);
        }

        public ActionResult PartialQuestionMapGridList(LMSContentModel model)
        {
            try
            {
                GetQuestionMapGridListDetails(model.Is_ContentId);
                return PartialView("PartialQuestionMapGridList", QuestionMapGridListDetails());

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        public void GetQuestionMapGridListDetails(string ContentID)
        {
            string user_id = Convert.ToString(Session["userid"]);

            string action = string.Empty;
            DataTable formula_dtls = new DataTable();
            DataSet dsInst = new DataSet();

            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "GET_CONTENTQUESTIONMAP_LISTINGDATA");
                proc.AddPara("@CONTENTID", ContentID);
                proc.AddPara("@USERID", Convert.ToInt32(user_id));
                dt = proc.GetTable();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable QuestionMapGridListDetails()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["ERP_ConnectionString"].ConnectionString;
            string Userid = Convert.ToString(Session["userid"]);

            ////////DataTable dtColmn = GetPageRetention(Session["userid"].ToString(), "CRM Contact");
            ////////if (dtColmn != null && dtColmn.Rows.Count > 0)
            ////////{
            ////////    ViewBag.RetentionColumn = dtColmn;//.Rows[0]["ColumnName"].ToString()  DataTable na class pathao ok wait
            ////////}

            LMSMasterDataContext dc = new LMSMasterDataContext(connectionString);
            var q = from d in dc.LMS_CONTENTQUESTIONMAP_LISTINGs
                    where d.USERID == Convert.ToInt32(Userid)
                    orderby d.SEQ
                    select d;
            return q;


        }

        public ActionResult GetLMSTopicList()
        {
            try
            {
                List<TopicList> modelTopic = new List<TopicList>();

                DataSet ds = new DataSet();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "GETDROPDOWNBINDDATA");
                ds = proc.GetDataSet();

                if (ds != null)
                {
                    // Company
                    List<TopicList> TopicList = new List<TopicList>();
                    TopicList = APIHelperMethods.ToModelList<TopicList>(ds.Tables[0]);
                    modelTopic = TopicList;
                }


                return PartialView("~/Areas/LMS/Views/LMSContentUpload/_LMSTopicPartial.cshtml", modelTopic);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public ActionResult GetLMSCategoriesList()
        {
            try
            {
                List<CategoryList> modelCategory = new List<CategoryList>();

                DataSet ds = new DataSet();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "GETCATEGORYDROPDOWNBINDDATA");
                ds = proc.GetDataSet();

                if (ds != null)
                {
                    // Company
                    List<CategoryList> CategoryList = new List<CategoryList>();
                    CategoryList = APIHelperMethods.ToModelList<CategoryList>(ds.Tables[0]);
                    modelCategory = CategoryList;
                }


                return PartialView("~/Areas/LMS/Views/LMSContentUpload/_LMSCategoryPartial.cshtml", modelCategory);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }

        public JsonResult GetQuestionListForMap(string TopicIds, string CategoryIds, string ContentId)
        {
            List<QuestionListForMap> modelQuestionListForMap = new List<QuestionListForMap>();

            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            proc.AddPara("@Action", "GETQUESTIONLISTFORMAP");
            proc.AddPara("@CONTENTID", ContentId);
            proc.AddPara("@TOPICIDS", TopicIds);
            proc.AddPara("@CATEGORYIDS", CategoryIds);
            proc.AddPara("@BRANCHID", Convert.ToString(Session["userbranchHierarchy"]));
            proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));

            dt = proc.GetTable();

            if (dt != null)
            {
                modelQuestionListForMap = APIHelperMethods.ToModelList<QuestionListForMap>(dt);
            }

            return Json(modelQuestionListForMap, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveContentQuestionMap(LMSContentAddModel data)
        {
            try
            {
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", data.Action);
                proc.AddPara("@CONTENTID", data.ContentID);
                proc.AddPara("@SELECTEDQUESTIONMAPLIST", data.SelectedQuestionMapList);
                proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));
                proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                proc.AddVarcharPara("@RETURN_DUPLICATEMAPNAME", -1, "", QueryParameterDirection.Output);
                int k = proc.RunActionQuery();
                data.RETURN_VALUE = Convert.ToString(proc.GetParaValue("@RETURN_VALUE"));
                data.RETURN_DUPLICATEMAPNAME = Convert.ToString(proc.GetParaValue("@RETURN_DUPLICATEMAPNAME"));
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }
        public JsonResult ShowContentDetailsForEdit(string ContentId)
        {
            List<QuestionListForMap> modelQuestionListForMap = new List<QuestionListForMap>();

            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
            proc.AddPara("@Action", "GETCONTENTDETAILSFOREDIT");
            proc.AddPara("@CONTENTID", ContentId);
            proc.AddPara("@BRANCHID", Convert.ToString(Session["userbranchHierarchy"]));
            proc.AddPara("@USERID", Convert.ToString(HttpContext.Session["userid"]));

            dt = proc.GetTable();

            if (dt != null)
            {
                modelQuestionListForMap = APIHelperMethods.ToModelList<QuestionListForMap>(dt);
            }

            return Json(modelQuestionListForMap, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult QuestionMapDelete(string ContentID)
        {
            string output_msg = string.Empty;

            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "QUESTIONMAPDELETE");
                proc.AddPara("@CONTENTID", ContentID);
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

        public ActionResult PartialMappedQuesForView(LMSContentModel model)
        {
            try
            {
                string user_id = Convert.ToString(Session["userid"]);
                List<QuestionMappedViewModel> qmapmodel = new List<QuestionMappedViewModel>();

                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "GETMAPPEDQUESTIONSFORVIEW");
                proc.AddPara("@CONTENTID", model.Is_ContentId);
                proc.AddPara("@USERID", Convert.ToInt32(user_id));
                dt = proc.GetTable();
                
                if (dt != null)
                {
                    qmapmodel = APIHelperMethods.ToModelList<QuestionMappedViewModel>(dt);
                }

                return PartialView("PartialMappedQuesForView", qmapmodel);

            }
            catch (Exception ex)
            {
                throw ex;

            }

        }

        public ActionResult ExporRegisterList()
        {
            return GridViewExtension.ExportToXlsx(GetDoctorBatchGridViewSettings(), QuestionMapGridListDetails());
        }

        private GridViewSettings GetDoctorBatchGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gridQuestionMapGridList";
            settings.SettingsExport.ExportedRowType = GridViewExportedRowType.All;
            settings.SettingsExport.FileName = "LMSContentUpload";

            settings.Columns.Add(x =>
            {
                x.FieldName = "SEQ";
                x.Caption = "Sr. No";
                x.VisibleIndex = 1;
                x.ExportWidth = 80;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "CONTENTTITLE";
                x.Caption = "Content Title";
                x.VisibleIndex = 2;
                x.ExportWidth = 300;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "CONTENTDESC";
                x.Caption = "Description";
                x.VisibleIndex = 3;
                x.ExportWidth = 300;

            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "QUESTIONSMAP_COUNT";
                x.Caption = "Mapped Questions";
                x.VisibleIndex = 4;
                x.ExportWidth = 350;

            });

            
            settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            settings.SettingsExport.LeftMargin = 20;
            settings.SettingsExport.RightMargin = 20;
            settings.SettingsExport.TopMargin = 20;
            settings.SettingsExport.BottomMargin = 20;

            return settings;

        }

        [HttpPost]
        public JsonResult UpdatePlaySequence(string ContentIdOld, string PlaySeqOld, string ContentIdNew, string PlaySeqNew)
        {
            string output_msg = string.Empty;

            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "UPDATEPLAYSEQUENCE");
                proc.AddPara("@CONTENTIDOLD", ContentIdOld);
                proc.AddPara("@PLAYSEQOLD", PlaySeqOld);
                proc.AddPara("@CONTENTIDNEW", ContentIdNew);
                proc.AddPara("@PLAYSEQNEW", PlaySeqNew);
                proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                dt = proc.GetTable();
                output_msg = Convert.ToString(proc.GetParaValue("@RETURN_VALUE"));

                //output_msg = "1";


            }
            catch (Exception ex)
            {
                output_msg = "Please try again later";
            }

            return Json(output_msg, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateStatusPublish(string ContentId, string TopicId, string ContentTitle, string chkStatus)
        {
            string output_msg = string.Empty;

            if (chkStatus != null && chkStatus == "Yes")
            {
                chkStatus = "1";
            }
            else
            {
                chkStatus = "0";
            }

            try
            {
                DataTable dt = new DataTable();
                ProcedureExecute proc = new ProcedureExecute("PRC_LMSCONTENTMASTER");
                proc.AddPara("@ACTION", "UPDATESTATUSPUBLISH");
                proc.AddPara("@CONTENTID", ContentId);
                proc.AddPara("@STATUS", chkStatus);
                
                proc.AddVarcharPara("@RETURN_VALUE", 500, "", QueryParameterDirection.Output);
                dt = proc.GetTable();
                output_msg = Convert.ToString(proc.GetParaValue("@RETURN_VALUE"));

                if (output_msg == "1" && chkStatus=="0")
                {
                    //FireNotification(ContentTitle, TopicId);
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