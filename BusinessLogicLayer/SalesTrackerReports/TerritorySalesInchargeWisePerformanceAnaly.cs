/*****************************************************************************************************************
 * 1.0		V2.0.38		Sanchita	13/04/2023		A tick box required as "Show Inactive Users" in parameter of 
												TERRITORY SALES INCHARGE WISE PERFORMANCE ANALYTICS report. Refer: 25811
********************************************************************************************************************/
using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.SalesTrackerReports
{
    public class TerritorySalesInchargeWisePerformanceAnaly
    {
        // Rev 1.0 [ ShowInactiveUsers added ]
        public DataTable GetTerritorySalesInchargeWisePerformanceAnalyReport(string fromdate,string todate, string stateID, string empid, string userid, int ShowInactiveUsers)
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_FTSTERRITORYSALESINCHARGEPERFORMANCEANALY_REPORT");

            proc.AddPara("@FROMDATE", fromdate);
            proc.AddPara("@TODATE", todate);
            proc.AddPara("@STATEID", stateID);
            proc.AddPara("@EMPID", empid);
            proc.AddPara("@USERID", userid);
            // Rev 1.0
            proc.AddPara("@ShowInactiveUsers", ShowInactiveUsers);
            // End of Rev 1.0
            ds = proc.GetTable();
            return ds;
        }

        public int InsertPageRetention(string Col, String USER_ID, String ReportName)
        {
            ProcedureExecute proc = new ProcedureExecute("PRC_FTS_PageRetention");
            proc.AddPara("@Col", Col);
            proc.AddPara("@ReportName", ReportName);
            proc.AddPara("@USER_ID", USER_ID);
            proc.AddPara("@ACTION", "INSERT");
            int i = proc.RunActionQuery();
            return i;
        }

        public DataTable GetPageRetention(String USER_ID, String ReportName)
        {
            DataTable dt = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_FTS_PageRetention");
            proc.AddPara("@ReportName", ReportName);
            proc.AddPara("@USER_ID", USER_ID);
            proc.AddPara("@ACTION", "DETAILS");
            dt = proc.GetTable();
            return dt;
        }
    }
}
