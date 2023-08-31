/******************************************************************************************************
 * 1.0     31-08-2023       2.0.43      Sanchita     FSM - Dashboard - View Party - Enhancement required. Refer: 26753
 * ******************************************************************************************************/
using BusinessLogicLayer;
using MyShop.Models;
using SalesmanTrack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityLayer;
// Rev 1.0
using Models;
// End of Rev 1.0

namespace MyShop.Areas.MYSHOP.Controllers
{
    public class ViewPartyController : Controller
    {
        //
        // GET: /MYSHOP/ViewParty/
        // Rev 1.0
        UserList lstuser = new UserList();
        EmployeeMasterReport omodel = new EmployeeMasterReport();
        // End of Rev 1.0

        public ActionResult ViewParty()
        {
            // Rev 1.0
            DataTable dtbranch = lstuser.GetHeadBranchList(Convert.ToString(Session["userbranchHierarchy"]), "HO");
            DataTable dtBranchChild = new DataTable();
            if (dtbranch.Rows.Count > 0)
            {
                dtBranchChild = lstuser.GetChildBranch(Convert.ToString(Session["userbranchHierarchy"]));
                if (dtBranchChild.Rows.Count > 0)
                {
                    DataRow dr;
                    dr = dtbranch.NewRow();
                    dr[0] = 0;
                    dr[1] = "All";
                    dtbranch.Rows.Add(dr);
                    dtbranch.DefaultView.Sort = "BRANCH_ID ASC";
                    dtbranch = dtbranch.DefaultView.ToTable();
                }
            }
            omodel.modelbranch = APIHelperMethods.ToModelList<GetBranch>(dtbranch);
            string h_id = omodel.modelbranch.First().BRANCH_ID.ToString();
            ViewBag.HeadBranch = omodel.modelbranch;
            ViewBag.h_id = h_id;

            string IsElectricianRequiredinViewParty = "0";
            DBEngine obj1 = new DBEngine();
            IsElectricianRequiredinViewParty = Convert.ToString(obj1.GetDataTable("select [value] from FTS_APP_CONFIG_SETTINGS WHERE [Key]='IsElectricianRequiredinViewParty'").Rows[0][0]);
            ViewBag.IsElectricianRequiredinViewParty = IsElectricianRequiredinViewParty;
            // End of Rev 1.0

            return View();
        }

        public ActionResult GetType()
        {
            DBEngine obj = new DBEngine();
            DataTable dt = obj.GetDataTable("select id,Name from tbl_shoptypeDetails");
            List<ddl> objddl = new List<ddl>();
            objddl = APIHelperMethods.ToModelList<ddl>(dt);
            return Json(objddl);
        }
        public ActionResult GetTypeOutlet()
        {
            DBEngine obj = new DBEngine();
            DataTable dt = obj.GetDataTable("select shop_typeId,Name  from tbl_shoptype  where IsActive=1");
            List<ddlOutlet> objddl = new List<ddlOutlet>();
            objddl = APIHelperMethods.ToModelList<ddlOutlet>(dt);
            return Json(objddl);
        }
        public ActionResult GetPartyStatus()
        {
            DBEngine obj = new DBEngine();
            DataTable dt = obj.GetDataTable("select ID id ,PARTYSTATUS Name from FSM_PARTYSTATUS");
            List<ddl> objddl = new List<ddl>();
            objddl = APIHelperMethods.ToModelList<ddl>(dt);
            return Json(objddl);
        }


        public class ddl
        {
            public Int64 id { get; set; }
            public string Name { get; set; }

        }

        public class ddlOutlet 
        {
            public Int64 shop_typeId { get; set; }
            public string Name { get; set; }

        }
	}


}