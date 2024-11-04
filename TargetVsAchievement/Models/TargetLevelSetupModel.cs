using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace TargetVsAchievement.Models
{
    public class TargetLevelSetupModel
    {
        public int DesignationId { get; set; }
        public List<DesignationList> DesignationList { get; set; }
        public int LevelBasedOn { get; set; }

        public string Action {  get; set; }
        public string SalesmanLevel { get; set; }
        public string BasedOn { get; set; }
        public string selectedEmployeeBasedOnMapList { get; set; }
        public string RETURN_VALUE { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Fromdate { get; set; }
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Todate { get; set; }

        public string TargetType { get; set; }
    }

    public class DesignationList
    {
        public String deg_id { get; set; }
        public String deg_designation { get; set; }
    }

    public class EmployeeMapList
    {
        public string BASEDON { get; set; }

        public string ID { get; set; }
        public string NAME { get; set; }
        public bool selected { get; set; }
        //public string tabNameText { get; set; }
        //public string rightTabNameText { get; set; }
        //public string searchPlaceholderText { get; set; }
        //public bool TOPICSTATUS { get; set; }
        //public string TOPIC_COMP_DAY { get; set; }
        //public bool TOPIC_ISDEFAULT { get; set; }
        //public string TOPIC_SEQ { get; set; }


    }
}