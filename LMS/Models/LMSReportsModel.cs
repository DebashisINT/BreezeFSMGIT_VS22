using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Web;

namespace LMS.Models
{
    public class LMSReportsModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Fromdate { get; set; }



        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Todate { get; set; }

        public string is_pageload { get; set; }
        public class GetUser
        {
            public Int64 user_id { get; set; }
            public string user_name { get; set; }
        }

        public class GetContent
        {
            public Int64 CONTENTID { get; set; }
            public string CONTENTTITLE { get; set; }
            public string CONTENTDESC { get; set; }
        }


        public DataTable GETDROPDOWNVALUE(string Action)
        {
            ProcedureExecute proc;
            int rtrnvalue = 0;
            DataTable dt = new DataTable();
            try
            {
                using (proc = new ProcedureExecute("PRC_LMS_REPORTS"))
                {
                    proc.AddVarcharPara("@ACTION", 100, Action);
                   // proc.AddIntegerPara("@ID", Convert.ToInt32(ID));
                    dt = proc.GetTable();
                    return dt;
                }
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


        public void CreateTable(string UserIds, string Topic_Id, string Content_Id, DateTime fromdate, DateTime todate,string userid)
        {
            ProcedureExecute proc = new ProcedureExecute("PRC_LMS_REPORTS");
            proc.AddPara("@ACTION", "GETLISTINGDETAILS");
            proc.AddPara("@fromdate", fromdate);
            proc.AddPara("@todate", todate);
            proc.AddPara("@USER_IDS", UserIds);
            proc.AddPara("@TOPIC_IDS", Topic_Id);
            proc.AddPara("@CONTENT_IDS", Content_Id);
            proc.AddPara("@USER_ID", userid);
            proc.GetScalar();

        }
    }
}