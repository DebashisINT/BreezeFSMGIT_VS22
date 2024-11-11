#region======================================Revision History=========================================================
//Written By : Debashis Talukder On 11/11/2024
//Purpose: Target Vs Achievement Info Details.Refer: Row: 992 & 993
#endregion===================================End of Revision History==================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShopAPI.Models
{
    public class TargetTypeListInput
    {
        public long user_id { get; set; }
    }
    public class TargetTypeListOutput
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<TAlistOutput> target_type_list { get; set; }
    }
    public class TAlistOutput
    {
        public long id { get; set; }
        public string name { get; set; }
    }

    public class TargetLevelListInput
    {
        public long user_id { get; set; }
    }
    public class TargetLevelListOutput
    {
        public string status { get; set; }
        public string message { get; set; }
        public List<TLlistOutput> target_level_list { get; set; }
    }
    public class TLlistOutput
    {
        public long id { get; set; }
        public string name { get; set; }
    }
}