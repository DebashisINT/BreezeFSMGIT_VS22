#region======================================Revision History=========================================================
//Written By : Debashis Talukder On 02/07/2024
//Purpose: LMS Info Details.Row: 945,949 & 950
#endregion===================================End of Revision History==================================================

using Newtonsoft.Json;
using ShopAPI.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.IO;
using System.Web;

namespace ShopAPI.Controllers
{
    public class LMSInfoDetailsController : ApiController
    {
        [HttpPost]
        public HttpResponseMessage TopicLists(TopicListInput model)
        {
            TopicListOutputModel omodel = new TopicListOutputModel();
            List<TopiclistOutput> Tview = new List<TopiclistOutput>();

            try
            {
                if (!ModelState.IsValid)
                {
                    omodel.status = "213";
                    omodel.message = "Some input parameters are missing.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, omodel);
                }
                else
                {
                    DataSet ds = new DataSet();
                    String con = System.Configuration.ConfigurationManager.AppSettings["DBConnectionDefault"];
                    SqlCommand sqlcmd = new SqlCommand();
                    SqlConnection sqlcon = new SqlConnection(con);
                    sqlcon.Open();
                    sqlcmd = new SqlCommand("PRC_FSMLMSINFODETAILS", sqlcon);
                    sqlcmd.Parameters.AddWithValue("@ACTION", "TOPICLISTS");
                    sqlcmd.Parameters.AddWithValue("@USER_ID", model.user_id);

                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                    da.Fill(ds);
                    sqlcon.Close();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        omodel.status = "200";
                        omodel.message = "Successfully Get List.";
                        Tview = APIHelperMethods.ToModelList<TopiclistOutput>(ds.Tables[0]);
                        omodel.user_id = model.user_id;
                        omodel.topic_list = Tview;
                    }
                    var message = Request.CreateResponse(HttpStatusCode.OK, omodel);
                    return message;
                }
            }
            catch (Exception ex)
            {
                omodel.status = "204";
                omodel.message = ex.Message;
                var message = Request.CreateResponse(HttpStatusCode.OK, omodel);
                return message;
            }
        }

        [HttpPost]
        public HttpResponseMessage TopicWiseLists(TopicWiseListsInput model)
        {
            TopicWiseListsOutput omodel = new TopicWiseListsOutput();
            int MultiContent=0;

            try
            {
                if (!ModelState.IsValid)
                {
                    omodel.status = "213";
                    omodel.message = "Some input parameters are missing.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, omodel);
                }
                else
                {
                    String AttachmentUrl = System.Configuration.ConfigurationManager.AppSettings["DocumentAttachment"];
                    DataSet ds = new DataSet();
                    String con = System.Configuration.ConfigurationManager.AppSettings["DBConnectionDefault"];
                    SqlCommand sqlcmd = new SqlCommand();
                    SqlConnection sqlcon = new SqlConnection(con);
                    sqlcon.Open();
                    sqlcmd = new SqlCommand("PRC_FSMLMSINFODETAILS", sqlcon);
                    sqlcmd.Parameters.AddWithValue("@ACTION", "TOPICWISELISTS");
                    sqlcmd.Parameters.AddWithValue("@TOPIC_ID", model.topic_id);

                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                    da.Fill(ds);
                    sqlcon.Close();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            for (int j = 0; j < ds.Tables[1].Rows.Count; j++)
                            {
                                List<ContentlistOutput> Coview = new List<ContentlistOutput>();
                                MultiContent = 1;
                                for (int k = 0; k < ds.Tables[2].Rows.Count; k++)
                                {
                                    List<QuestionlistOutput> Qoview = new List<QuestionlistOutput>();
                                    for (int l = 0; l < ds.Tables[3].Rows.Count; l++)
                                    {
                                        List<OptionlistOutput> Ooview = new List<OptionlistOutput>();
                                        if (Convert.ToInt64(ds.Tables[3].Rows[l]["topic_id"]) == Convert.ToInt64(ds.Tables[1].Rows[j]["topic_id"]) &&
                                            Convert.ToInt64(ds.Tables[3].Rows[l]["content_id"]) == Convert.ToInt64(ds.Tables[2].Rows[k]["content_id"]) 
                                            //&&
                                            //Convert.ToInt64(ds.Tables[3].Rows[l]["question_id"]) == Convert.ToInt64(ds.Tables[2].Rows[k]["question_id"])
                                            )
                                        {
                                            Ooview.Add(new OptionlistOutput()
                                            {
                                                question_id = Convert.ToInt64(ds.Tables[3].Rows[l]["question_id"]),
                                                option_id = Convert.ToInt64(ds.Tables[3].Rows[l]["option_id"]),
                                                option_no_1 = Convert.ToString(ds.Tables[3].Rows [l]["option_no_1"]),
                                                option_point_1 = Convert.ToInt64(ds.Tables[3].Rows[l]["option_point_1"]),
                                                isCorrect_1 = Convert.ToBoolean(ds.Tables[3].Rows[l]["isCorrect_1"]),
                                                option_no_2 = Convert.ToString(ds.Tables[3].Rows[l]["option_no_2"]),
                                                option_point_2 = Convert.ToInt64(ds.Tables[3].Rows[l]["option_point_2"]),
                                                isCorrect_2 = Convert.ToBoolean(ds.Tables[3].Rows[l]["isCorrect_2"]),
                                                option_no_3 = Convert.ToString(ds.Tables[3].Rows[l]["option_no_3"]),
                                                option_point_3 = Convert.ToInt64(ds.Tables[3].Rows[l]["option_point_3"]),
                                                isCorrect_3 = Convert.ToBoolean(ds.Tables[3].Rows[l]["isCorrect_3"]),
                                                option_no_4 = Convert.ToString(ds.Tables[3].Rows[l]["option_no_4"]),
                                                option_point_4 = Convert.ToInt64(ds.Tables[3].Rows[l]["option_point_4"]),
                                                isCorrect_4 = Convert.ToBoolean(ds.Tables[3].Rows[l]["isCorrect_4"])
                                            });
                                        }

                                        if (Convert.ToInt64(ds.Tables[1].Rows[j]["topic_id"]) == Convert.ToInt64(ds.Tables[2].Rows[k]["topic_id"]) &&
                                        Convert.ToInt64(ds.Tables[1].Rows[j]["content_id"]) == Convert.ToInt64(ds.Tables[2].Rows[k]["content_id"])
                                        //&&
                                        //Convert.ToInt64(ds.Tables[3].Rows[l]["question_id"]) == Convert.ToInt64(ds.Tables[2].Rows[k]["question_id"])
                                        )
                                        {
                                            Qoview.Add(new QuestionlistOutput()
                                            {
                                                topic_id = Convert.ToInt64(ds.Tables[2].Rows[k]["topic_id"]),
                                                content_id = Convert.ToInt64(ds.Tables[2].Rows[k]["content_id"]),
                                                //question_id = Convert.ToInt64(ds.Tables[2].Rows[k]["question_id"]),
                                                //question = Convert.ToString(ds.Tables[2].Rows[k]["question"]),
                                                //question_description = Convert.ToString(ds.Tables[2].Rows[k]["question_description"]),
                                                question_id = Convert.ToInt64(ds.Tables[3].Rows[l]["question_id"]),
                                                question = Convert.ToString(ds.Tables[3].Rows[l]["question"]),
                                                question_description = Convert.ToString(ds.Tables[3].Rows[l]["question_description"]),
                                                option_list = Ooview
                                            });
                                        }
                                    }
                                    if (Convert.ToInt64(ds.Tables[0].Rows[i]["topic_id"]) == Convert.ToInt64(ds.Tables[1].Rows[j]["topic_id"]) &&
                                        MultiContent==1)
                                    {
                                        Coview.Add(new ContentlistOutput()
                                        {
                                            content_id = Convert.ToInt64(ds.Tables[1].Rows[j]["content_id"]),
                                            content_url= AttachmentUrl+Convert.ToString(ds.Tables[1].Rows[j]["content_url"]),
                                            content_title = Convert.ToString(ds.Tables[1].Rows[j]["content_title"]),
                                            content_description = Convert.ToString(ds.Tables[1].Rows[j]["content_description"]),
                                            content_play_sequence = Convert.ToInt64(ds.Tables[1].Rows[j]["content_play_sequence"]),
                                            isAllowLike = Convert.ToBoolean(ds.Tables[1].Rows[j]["isAllowLike"]),
                                            isAllowComment = Convert.ToBoolean(ds.Tables[1].Rows[j]["isAllowComment"]),
                                            isAllowShare = Convert.ToBoolean(ds.Tables[1].Rows[j]["isAllowShare"]),
                                            question_list = Qoview
                                        });
                                    }
                                    MultiContent = 0;
                                }
                                if (ds.Tables[2].Rows.Count == 0 || ds.Tables[3].Rows.Count == 0)
                                {
                                    Coview = APIHelperMethods.ToModelList<ContentlistOutput>(ds.Tables[1]);
                                    omodel.content_list = Coview;
                                }
                                else
                                {
                                    omodel.content_list = Coview;
                                }
                            }
                        }

                        omodel.status = "200";
                        omodel.message = "Successfully Get List.";
                        omodel.topic_id = model.topic_id;
                        omodel.topic_name = Convert.ToString(ds.Tables[0].Rows[0]["topic_name"]);
                    }
                    var message = Request.CreateResponse(HttpStatusCode.OK, omodel);
                    return message;
                }
            }
            catch (Exception ex)
            {
                omodel.status = "204";
                omodel.message = ex.Message;
                var message = Request.CreateResponse(HttpStatusCode.OK, omodel);
                return message;
            }
        }

        [HttpPost]
        public HttpResponseMessage UserWiseLMSModulesInfo(UserWiseLMSModulesInfoSaveInput model)
        {
            UserWiseLMSModulesInfoSaveOutput omodel = new UserWiseLMSModulesInfoSaveOutput();

            try
            {
                if (!ModelState.IsValid)
                {
                    omodel.status = "213";
                    omodel.message = "Some input parameters are missing.";
                    return Request.CreateResponse(HttpStatusCode.BadRequest, omodel);
                }
                else
                {
                    List<Userlmsinfolists> omodel2 = new List<Userlmsinfolists>();
                    foreach (var s2 in model.user_lms_info_list)
                    {
                        omodel2.Add(new Userlmsinfolists()
                        {
                            module_name = s2.module_name,
                            count_of_use = s2.count_of_use,
                            time_spend = s2.time_spend,
                            last_current_loc_lat = s2.last_current_loc_lat,
                            last_current_loc_long = s2.last_current_loc_long,
                            last_current_loc_address = s2.last_current_loc_address,
                            date_time = s2.date_time,
                            phone_model = s2.phone_model
                        });
                    }

                    string JsonXML = XmlConversion.ConvertToXml(omodel2, 0);

                    DataTable dt = new DataTable();
                    String con = System.Configuration.ConfigurationManager.AppSettings["DBConnectionDefault"];
                    SqlCommand sqlcmd = new SqlCommand();
                    SqlConnection sqlcon = new SqlConnection(con);
                    sqlcon.Open();
                    sqlcmd = new SqlCommand("PRC_FSMLMSINFODETAILS", sqlcon);
                    sqlcmd.Parameters.AddWithValue("@ACTION", "MODULEINFOSAVE");
                    sqlcmd.Parameters.AddWithValue("@USER_ID", model.user_id);
                    sqlcmd.Parameters.AddWithValue("@JsonXML", JsonXML);

                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    SqlDataAdapter da = new SqlDataAdapter(sqlcmd);
                    da.Fill(dt);
                    sqlcon.Close();
                    if (dt.Rows.Count > 0 && Convert.ToInt64(dt.Rows[0][0]) == model.user_id)
                    {
                        omodel.status = "200";
                        omodel.message = "Information added successfully.";
                    }
                    else
                    {
                        omodel.status = "205";
                        omodel.message = "Data not Saved.";
                    }
                    var message = Request.CreateResponse(HttpStatusCode.OK, omodel);
                    return message;
                }
            }
            catch (Exception ex)
            {
                omodel.status = "204";
                omodel.message = ex.Message;
                var message = Request.CreateResponse(HttpStatusCode.OK, omodel);
                return message;
            }
        }
    }
}
