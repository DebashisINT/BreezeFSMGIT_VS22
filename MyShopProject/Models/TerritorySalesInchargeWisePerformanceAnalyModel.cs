/*****************************************************************************************************************
 * 1.0		V2.0.38		Sanchita	13/04/2023		A tick box required as "Show Inactive Users" in parameter of 
												TERRITORY SALES INCHARGE WISE PERFORMANCE ANALYTICS report. Refer: 25811
********************************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Models;
using System.Web.Mvc;

namespace Models
{
    public class TerritorySalesInchargeWisePerformanceAnalyModel
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Fromdate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Todate { get; set; }
        public List<string> StateId { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<string> desgid { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<string> empcode { get; set; }

        public List<GetUsersStates> states { get; set; }

        public List<GetDesignation> designation { get; set; }

        public List<GetAllEmployee> employee { get; set; }

        public string is_pageload { get; set; }
        public String EmployeeCode { get; set; }
        public Int32 is_procfirst { get; set; }

        // Rev 1.0
        public int IsShowInactiveUsers { get; set; }
        // End of Rev 1.0

    }
}