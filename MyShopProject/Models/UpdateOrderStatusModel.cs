using DataAccessLayer;
using Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class UpdateOrderStatusModel
    {
        public string selectedusrid { get; set; }
        public List<string> StateId { get; set; }
        public List<GetUserName> userlsit { get; set; }
        public List<GetUsersStates> states { get; set; }
        public List<string> empcode { get; set; }
        public string Is_PageLoad { get; set; }
        public List<string> shopId { get; set; }
        public List<Getmasterstock> shoplist { get; set; }
        public string Fromdate { get; set; }
        public string Todate { get; set; }
        public int Uniquecont { get; set; }
        //rev Pratik
        public int IsSchemeDetails { get; set; }
        //End of rev Pratik
        //Rev Debashis
        public int IsPaitentDetails { get; set; }
        //End of Rev Debashis
        //Rev Debashis 0025198
        public List<string> BranchId { get; set; }
        public List<GetBranch> modelbranch = new List<GetBranch>();
        //End of Rev Debashis 0025198
        // Rev 1.0
        public int IsShowMRP { get; set; }
        public int IsShowDiscount { get; set; }
        // End of Rev 1.0
        public string UPDATESTATUS { get; set; }
        public List<string> SectionIds { get; set; }
        public string STATUSId { get; set; }
        public List<STATUSLIST> STATUSLIST { get; set; }

        
        public DataTable GetUpdateStatusOption(string USERID)
        {
            ProcedureExecute proc;
            int rtrnvalue = 0;
            DataTable dt = new DataTable();
            try
            {
                using (proc = new ProcedureExecute("PRC_LMS_POINTSMASTER"))
                {
                    proc.AddVarcharPara("@ACTION", 100, "GetUpdateStatusOption");
                    proc.AddPara("@USERID",Convert.ToInt32(USERID));
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
    }

    public class STATUSLIST
    {
        public string STATUSID { get; set; }
        public string STATUSVALUE { get; set; }
    }

    
}