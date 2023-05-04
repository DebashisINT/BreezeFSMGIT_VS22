using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyShop.Models
{
    public class ExpenseReport
    {
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string FromDate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string ToDate { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Is_PageLoad { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string EmployeeCode { get; set; }

        public class GetHQName
        {
            public string HQid { get; set; }

            public string HQname { get; set; }
        }

        public class GetExpenseType
        {
            public string expid { get; set; }

            public string expense_type { get; set; }
        }
    }
}