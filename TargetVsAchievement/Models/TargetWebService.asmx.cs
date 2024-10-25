using DataAccessLayer;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;

namespace TargetVsAchievement.Models
{
    /// <summary>
    /// Summary description for TargetWebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
     [System.Web.Script.Services.ScriptService]
    public class TargetWebService : System.Web.Services.WebService
    {

        string ConnectionString = String.Empty;
        public TargetWebService()
        {
            ConnectionString = Convert.ToString(System.Web.HttpContext.Current.Session["DBConnectionDefault"]);
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public object GetTargetLevelDetailsList(string SearchKey, string Type)
        {
            
            List<SalesTargetLevelDetails> list = new List<SalesTargetLevelDetails>();
           
            if (HttpContext.Current.Session["userid"] != null)
            {
                SearchKey = SearchKey.Replace("'", "''");
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConnectionString);
                string Query = "";
                Query = @"   EXEC PRC_FSMSALESTARGET '" + Type + "','" + SearchKey + "' ";


                DataTable dt = oDBEngine.GetDataTable(Query);

                if (!String.IsNullOrEmpty(Type))
                {

                    list = (from DataRow dr in dt.Rows
                            select new SalesTargetLevelDetails()
                            {
                                Level_ID = Convert.ToString(dr["ID"]),
                                Level_Code = Convert.ToString(dr["CODE"]),
                                Level_Name = Convert.ToString(dr["NAME"]),

                            }).ToList();
                }
                else
                {
                    list = (from DataRow dr in dt.Rows
                            select new SalesTargetLevelDetails()
                            {
                                Level_ID = Convert.ToString(dr["ID"]),
                                Level_Code = Convert.ToString(dr["CODE"]),
                                Level_Name = Convert.ToString(dr["NAME"]),
                               
                            }).ToList();
                }

            }

            return list;
        }
    }

    public class SalesTargetLevelDetails
    {
        public string Level_ID { get; set; }
        public string Level_Code { get; set; }
        public string Level_Name { get; set;}
    }
}
