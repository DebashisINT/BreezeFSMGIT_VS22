using ClosedXML.Excel;
using DataAccessLayer;
using DevExpress.Web.Mvc;
using DevExpress.Web;
using DevExpress.XtraPrinting;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ERP.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Threading.Tasks;
using iTextSharp.text.log;
using System.Globalization;
using DocumentFormat.OpenXml.Office.Word;
using BusinessLogicLayer;
using ClsDropDownlistNameSpace;
using SalesmanTrack;
using static ERP.OMS.Management.Activities.SpecialPriceUpload;
using static ERP.OMS.Management.Activities.CustSaleRateLock;


namespace ERP.OMS.Management.Activities
{
    public partial class ProductsBranchMap : System.Web.UI.Page
    {
        UserList lstuser = new UserList();
        BusinessLogicLayer.GenericMethod oGenericMethod;
        BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationSettings.AppSettings["DBConnectionDefault"]);
        clsDropDownList oclsDropDownList = new clsDropDownList();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["SI_ComponentData_Branch"] = null;
            }
        }

        #region Branch Populate
        protected void Componentbranch_Callback(object source, DevExpress.Web.CallbackEventArgsBase e)
        {
            string FinYear = Convert.ToString(Session["LastFinYear"]);
            if (e.Parameter.Split('~')[0] == "BindComponentGrid")
            {
                DataTable ComponentTable = new DataTable();
                string Hoid = e.Parameter.Split('~')[1];
                if (Hoid != "All")
                {
                    ComponentTable = GetBranch(Convert.ToString(HttpContext.Current.Session["userbranchHierarchy"]), Hoid);
                    if (ComponentTable.Rows.Count > 0)
                    {
                        Session["SI_ComponentData_Branch"] = ComponentTable;
                        lookup_branch.DataSource = ComponentTable;
                        lookup_branch.DataBind();
                    }
                    else
                    {
                        Session["SI_ComponentData_Branch"] = ComponentTable;
                        lookup_branch.DataSource = null;
                        lookup_branch.DataBind();
                    }
                }
                else
                {

                    ProcedureExecute proc = new ProcedureExecute("PRC_FSMBRANCHWISEPRODUCTMAPPING");
                    proc.AddVarcharPara("@Action", 50, "FETCHBRANCHS");
                    ComponentTable = proc.GetTable();
                    if (ComponentTable.Rows.Count > 0)
                    {
                        Session["SI_ComponentData_Branch"] = ComponentTable;
                        lookup_branch.DataSource = ComponentTable;
                        lookup_branch.DataBind();
                    }
                }
            }

            //if (e.Parameter.Split('~')[0] == "BindComponentGrid")
            //{
            //    DataTable ComponentTable = new DataTable();

            //    BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
            //    ProcedureExecute proc = new ProcedureExecute("PRC_FSMBRANCHWISEPRODUCTMAPPING");
            //    proc.AddVarcharPara("@Action", 50, "FETCHBRANCHS");                
            //    ComponentTable = proc.GetTable();

            //    if (ComponentTable.Rows.Count > 0)
            //    {
            //        Session["SI_ComponentData_Branch"] = ComponentTable;
            //        lookup_branch.DataSource = ComponentTable;
            //        lookup_branch.DataBind();
            //    }
            //    else
            //    {
            //        Session["SI_ComponentData_Branch"] = ComponentTable;
            //        lookup_branch.DataSource = null;
            //        lookup_branch.DataBind();
            //    }
            //}
        }
        public DataTable GetBranch(string BRANCH_ID, string Ho)
        {
            DataTable dt = new DataTable();
            //SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["DBConnectionDefault"]);
            SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
            SqlCommand cmd = new SqlCommand("GetFinancerBranchfetchhowise", con);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Branch", BRANCH_ID);
            cmd.Parameters.AddWithValue("@Hoid", Ho);
            cmd.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            da.Fill(dt);
            cmd.Dispose();
            con.Dispose();

            return dt;
        }
        protected void lookup_branch_DataBinding(object sender, EventArgs e)
        {
            if (Session["SI_ComponentData_Branch"] != null)
            {
                lookup_branch.DataSource = (DataTable)Session["SI_ComponentData_Branch"];
            }
        }
        #endregion      

        protected void ProductComponentPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter.Split('~')[0] == "BindProductGrid")
            {
                DataTable ComponentTable = new DataTable();

                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                ProcedureExecute proc = new ProcedureExecute("PRC_FSMBRANCHWISEPRODUCTMAPPING");
                proc.AddVarcharPara("@Action", 50, "FETCHPRODUCTS");
                ComponentTable = proc.GetTable();

                if (ComponentTable.Rows.Count > 0)
                {
                    Session["ComponentData_Product"] = ComponentTable;
                    gridProductLookup.DataSource = ComponentTable;
                    gridProductLookup.DataBind();
                }
                else
                {
                    Session["ComponentData_Product"] = ComponentTable;
                    gridProductLookup.DataSource = null;
                    gridProductLookup.DataBind();
                }
            }
        }

        protected void gridProductLookup_DataBinding(object sender, EventArgs e)
        {
            if (Session["ComponentData_Product"] != null)
            {
                gridProductLookup.DataSource = (DataTable)Session["ComponentData_Product"];
            }
        }

        protected void ParentEmpComponentPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter.Split('~')[0] == "BindParentEmpgrid")
            {
                string BranchComponent = "";
                string BRANCH_ID = "";
                List<object> BranchList = lookup_branch.GridView.GetSelectedFieldValues("ID");
                foreach (object Brnch in BranchList)
                {
                    BranchComponent += "," + Brnch;
                }
                BRANCH_ID = BranchComponent.TrimStart(',');


                DataTable ComponentTable = new DataTable();

                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                ProcedureExecute proc = new ProcedureExecute("PRC_FSMBRANCHWISEPRODUCTMAPPING");
                proc.AddVarcharPara("@Action", 50, "FETCHPARENTEMPLOYEE");
                proc.AddVarcharPara("@BRANCHID", 4000, BRANCH_ID);
                ComponentTable = proc.GetTable();

                if (ComponentTable.Rows.Count > 0)
                {
                    Session["ComponentData_ParentEmp"] = ComponentTable;
                    gridParentEmpLookup.DataSource = ComponentTable;
                    gridParentEmpLookup.DataBind();
                }
                else
                {
                    Session["ComponentData_ParentEmp"] = ComponentTable;
                    gridParentEmpLookup.DataSource = null;
                    gridParentEmpLookup.DataBind();
                }
            }
        }
        protected void ParentEmpLookup_DataBinding(object sender, EventArgs e)
        {
            if (Session["ComponentData_ParentEmp"] != null)
            {
                gridParentEmpLookup.DataSource = (DataTable)Session["ComponentData_ParentEmp"];
            }
        }
        protected void CallbackPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            string returnPara = Convert.ToString(e.Parameter);

            string BranchComponent = "";
            string BRANCH_ID = "";
            List<object> BranchList = lookup_branch.GridView.GetSelectedFieldValues("ID");
            foreach (object Brnch in BranchList)
            {
                BranchComponent += "," + Brnch;
            }
            BRANCH_ID = BranchComponent.TrimStart(',');

            string PRODUCT_ID = "";
            string PRODUCTS = "";
            List<object> PRODUCTSList = gridProductLookup.GridView.GetSelectedFieldValues("SPRODUCTS_ID");
            foreach (object PRODUCT in PRODUCTSList)
            {
                PRODUCTS += "," + PRODUCT;
            }
            PRODUCT_ID = PRODUCTS.TrimStart(',');


            string ParentEMP_ID = "";
            string ParentEMP = "";
            List<object> ParentEMPIDList = gridParentEmpLookup.GridView.GetSelectedFieldValues("cnt_internalId");
            foreach (object Parent in ParentEMPIDList)
            {                
                ParentEMP += "," + Parent;
            }
            ParentEMP_ID = ParentEMP.TrimStart(',');


            string ChildEMP_ID = "";
            string ChildEMP = "";
            List<object> ChildEMPIDList = ChildParentEmpLookup.GridView.GetSelectedFieldValues("cnt_internalId");
            foreach (object child in ChildEMPIDList)
            {
                ChildEMP += "," + child;
            }
            ChildEMP_ID = ChildEMP.TrimStart(',');


            try
            {
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                ProcedureExecute proc = new ProcedureExecute("PRC_FSMBRANCHWISEPRODUCTMAPPING");
                proc.AddVarcharPara("@Action", 100, "INSERT");
                proc.AddVarcharPara("@PRODUCTID", 4000, PRODUCT_ID);
                proc.AddVarcharPara("@BRANCHID", 4000, BRANCH_ID);
                proc.AddVarcharPara("@ParentEMPID", 4000, ParentEMP_ID);
                proc.AddVarcharPara("@ChildEMPID", 4000, ChildEMP_ID);
                proc.AddIntegerPara("@USERID", Convert.ToInt32(HttpContext.Current.Session["userid"]));
                DataTable dt = proc.GetTable();
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["Success"].ToString() == "True"
)
                    {
                        CallbackPanel.JSProperties["cpSaveSuccessOrFail"] = "1";
                    }
                    else if (dt.Rows[0]["Success"].ToString() == "-10")
                    {
                        CallbackPanel.JSProperties["cpSaveSuccessOrFail"] = "-10";
                    }
                   
                }
                else
                {
                    CallbackPanel.JSProperties["cpSaveSuccessOrFail"] = "-10";
                }

            }
            catch
            {

            }
        }

        protected void ChildEmpComponentPanel_Callback(object sender, CallbackEventArgsBase e)
        {
            if (e.Parameter.Split('~')[0] == "BindChildEmpGrid")
            {
                string BranchComponent = "";
                string BRANCH_ID = "";
                List<object> BranchList = lookup_branch.GridView.GetSelectedFieldValues("ID");
                foreach (object Brnch in BranchList)
                {
                    BranchComponent += "," + Brnch;
                }
                BRANCH_ID = BranchComponent.TrimStart(',');

                string ParentEMP_ID = "";
                string ParentEMP = "";
                List<object> ParentEMPIDList = gridParentEmpLookup.GridView.GetSelectedFieldValues("cnt_internalId");
                foreach (object Parent in ParentEMPIDList)
                {
                    ParentEMP += "," + Parent;
                }
                ParentEMP_ID = ParentEMP.TrimStart(',');



                if(ParentEMP_ID=="")
                {
                    DataTable ComponentTable = new DataTable();

                    BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                    ProcedureExecute proc = new ProcedureExecute("PRC_FSMBRANCHWISEPRODUCTMAPPING");
                    proc.AddVarcharPara("@Action", 50, "FETCHCHILDEMPLOYEE");
                    proc.AddVarcharPara("@BRANCHID", 4000, BRANCH_ID);
                    proc.AddVarcharPara("@ParentEMPID", 4000, ParentEMP_ID);
                    ComponentTable = proc.GetTable();

                    if (ComponentTable.Rows.Count > 0)
                    {
                        Session["ComponentData_ChildEmp"] = ComponentTable;
                        ChildParentEmpLookup.DataSource = ComponentTable;
                        ChildParentEmpLookup.DataBind();
                    }
                    else
                    {
                        Session["ComponentData_ChildEmp"] = ComponentTable;
                        ChildParentEmpLookup.DataSource = null;
                        ChildParentEmpLookup.DataBind();
                    }

                }
                else
                {
                    DataTable ComponentTable = new DataTable();

                    BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                    ProcedureExecute proc = new ProcedureExecute("PRC_FSMBRANCHWISEPRODUCTMAPPING");
                    proc.AddVarcharPara("@Action", 50, "FETCHCHILDEMPLOYEE");
                    proc.AddVarcharPara("@BRANCHID", 4000, BRANCH_ID);
                    proc.AddVarcharPara("@ParentEMPID", 4000, ParentEMP_ID);
                    ComponentTable = proc.GetTable();

                    if (ComponentTable.Rows.Count > 0)
                    {
                        Session["ComponentData_ChildEmp"] = ComponentTable;
                        ChildParentEmpLookup.DataSource = ComponentTable;
                        ChildParentEmpLookup.DataBind();
                    }
                    else
                    {
                        Session["ComponentData_ChildEmp"] = ComponentTable;
                        ChildParentEmpLookup.DataSource = null;
                        ChildParentEmpLookup.DataBind();
                    }
                }

            }
        }

        protected void ChildParentEmpLookup_DataBinding(object sender, EventArgs e)
        {
            if (Session["ComponentData_ChildEmp"] != null)
            {
                ChildParentEmpLookup.DataSource = (DataTable)Session["ComponentData_ChildEmp"];
            }
        }

       
    }
}