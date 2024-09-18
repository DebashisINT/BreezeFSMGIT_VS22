using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using BusinessLogicLayer;
using BusinessLogicLayer.SalesmanTrack;
using BusinessLogicLayer.SalesTrackerReports;
using DataAccessLayer;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using DocumentFormat.OpenXml.Drawing.Spreadsheet;
using Models;
using MyShop.Models;
using SalesmanTrack;
using UtilityLayer;
using static MyShop.Models.UpdateOrderStatusModel;

namespace MyShop.Areas.MYSHOP.Controllers
{
    public class UpdateOrderStatusController : Controller
    {
        // GET: MYSHOP/UpdateOrderStatus       

        CommonBL objSystemSettings = new CommonBL();
        UserList lstuser = new UserList();
        OrderList objshop = new OrderList();
        DataTable dtuser = new DataTable();
        DataTable dtstate = new DataTable();
        DataTable dtshop = new DataTable();
        List<OrderDetailsSummaryProducts> oproduct = new List<OrderDetailsSummaryProducts>();
        OrderDetailsSummaryProducts mproductwindow = new OrderDetailsSummaryProducts();
        List<OrderUpdateDetailsSummary> omodel = new List<OrderUpdateDetailsSummary>();
        ProductDetails _productDetails = new ProductDetails();
        DataTable dtquery = new DataTable();
        public ActionResult Index()
        {
            try
            {
                UpdateOrderStatusModel omodel = new UpdateOrderStatusModel();
                string userid = Session["userid"].ToString();
                omodel.selectedusrid = userid;
                omodel.Fromdate = DateTime.Now.ToString("dd-MM-yyyy");
                omodel.Todate = DateTime.Now.ToString("dd-MM-yyyy");

                DataTable dt = new SalesSummary_Report().GetStateMandatory(Session["userid"].ToString());
                if (dt != null && dt.Rows.Count > 0)
                {
                    ViewBag.StateMandatory = dt.Rows[0]["IsStateMandatoryinReport"].ToString();
                }

                EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/UpdateOrderStatus/Index");
                
                ViewBag.CanView = rights.CanView;
                ViewBag.CanExport = rights.CanExport;         
                ViewBag.CanPrint = rights.CanPrint;
                ViewBag.CanInvoice= rights.CanInvoice;
                ViewBag.CanReadyToDispatch= rights.CanReadyToDispatch;
                ViewBag.CanDispatch= rights.CanDispatch;
                ViewBag.CanDeliver= rights.CanDeliver;              

               

                STATUSLIST dataobj = new STATUSLIST();
                List<STATUSLIST> _STATUSLIST = new List<STATUSLIST>();


                
                dataobj.STATUSID = "Select";
                dataobj.STATUSVALUE = "Select";
                _STATUSLIST.Add(dataobj);


                if (rights.CanInvoice==true)
                {
                    dataobj = new STATUSLIST();
                    dataobj.STATUSID = "Invoiced";
                    dataobj.STATUSVALUE = "Invoiced";
                    _STATUSLIST.Add(dataobj);
                }
                else if (rights.CanReadyToDispatch == true)
                {
                    dataobj = new STATUSLIST();
                    dataobj.STATUSID = "Ready To Dispatch";
                    dataobj.STATUSVALUE = "Ready To Dispatch";
                    _STATUSLIST.Add(dataobj);
                }
                else if (rights.CanDispatch == true)
                {
                    dataobj = new STATUSLIST();
                    dataobj.STATUSID = "Dispatched";
                    dataobj.STATUSVALUE = "Dispatched";
                    _STATUSLIST.Add(dataobj);
                }
                else if (rights.CanDeliver == true)
                {
                    dataobj = new STATUSLIST();
                    dataobj.STATUSID = "Delivered";
                    dataobj.STATUSVALUE = "Delivered";
                    _STATUSLIST.Add(dataobj);
                }

                omodel.STATUSLIST = _STATUSLIST;
                


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
                ViewBag.h_id = h_id;
               
                return View(omodel);

            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }
        public ActionResult PartialOrderSummary(UpdateOrderStatusModel model)
        {
            try
            {
                string Is_PageLoad = string.Empty;
                String weburl = System.Configuration.ConfigurationSettings.AppSettings["SiteURL"];
                List<GpsStatusClasstOutput> omel = new List<GpsStatusClasstOutput>();

                DataTable dt = new DataTable();

                if (model.Fromdate == null)
                {
                    model.Fromdate = DateTime.Now.ToString("dd-MM-yyyy");
                }

                if (model.Todate == null)
                {
                    model.Todate = DateTime.Now.ToString("dd-MM-yyyy");
                }
                if (model.Is_PageLoad == "0") Is_PageLoad = "Ispageload";
                ViewData["ModelData"] = model;

                string datfrmat = model.Fromdate.Split('-')[2] + '-' + model.Fromdate.Split('-')[1] + '-' + model.Fromdate.Split('-')[0];
                string dattoat = model.Todate.Split('-')[2] + '-' + model.Todate.Split('-')[1] + '-' + model.Todate.Split('-')[0];
                string Userid = Convert.ToString(Session["userid"]);
                string state = "";
                int i = 1;
                if (model.StateId != null && model.StateId.Count > 0)
                {
                    foreach (string item in model.StateId)
                    {
                        if (i > 1)
                            state = state + "," + item;
                        else
                            state = item;
                        i++;
                    }
                }
                string shop = "";
                int j = 1;
                if (model.shopId != null && model.shopId.Count > 0)
                {
                    foreach (string item in model.shopId)
                    {
                        if (j > 1)
                            shop = shop + "," + item;
                        else
                            shop = item;
                        j++;
                    }
                }
                string empcode = "";
                int k = 1;
                if (model.empcode != null && model.empcode.Count > 0)
                {
                    foreach (string item in model.empcode)
                    {
                        if (k > 1)
                            empcode = empcode + "," + item;
                        else
                            empcode = item;
                        k++;
                    }
                }               
                //if (model.IsPaitentDetails != null)
                //{
                //    TempData["IsPaitentDetails"] = model.IsPaitentDetails;
                //    TempData.Keep();
                //}
               
                string Branch_Id = "";
                int l = 1;
                if (model.BranchId != null && model.BranchId.Count > 0)
                {
                    foreach (string item in model.BranchId)
                    {
                        if (l > 1)
                            Branch_Id = Branch_Id + "," + item;
                        else
                            Branch_Id = item;
                        l++;
                    }
                }
               

                if (model.Is_PageLoad != "0")
                {
                    double days = (Convert.ToDateTime(dattoat) - Convert.ToDateTime(datfrmat)).TotalDays;
                    if (days <= 30)
                    {
                      
                        dt = GetallorderListSummary(state, shop, datfrmat, dattoat, empcode, Branch_Id, Userid, model.UPDATESTATUS);
                       
                    }
                    omodel = APIHelperMethods.ToModelList<OrderUpdateDetailsSummary>(dt);
                }

                
                DataTable dtColmn = objshop.GetPageRetention(Session["userid"].ToString(), "UPDATE ORDER STATUS");
                if (dtColmn != null && dtColmn.Rows.Count > 0)
                {
                    ViewBag.RetentionColumn = dtColmn;
                }
                
                EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/UpdateOrderStatus/Index");
                ViewBag.CanPrint = rights.CanPrint;                
                TempData["UpdateOrderStatusList"] = omodel;
                return PartialView("_PartialUpdateOrderStatus", omodel);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }
        public DataTable GetallorderListSummary(string stateid, string shopid, string fromdate, string todate, string EmployeeId, string Branch_Id, String Userid = "0", string UPDATESTATUS="")
        
        {
            DataTable ds = new DataTable();
            ProcedureExecute proc = new ProcedureExecute("PRC_UPDATE_ORDER_STATUS");

            proc.AddPara("@start_date", fromdate);
            proc.AddPara("@end_date", todate);
            proc.AddPara("@stateID", stateid);
            proc.AddPara("@shop_id", shopid);
            proc.AddPara("@Employee_id", EmployeeId);
            proc.AddPara("@LOGIN_ID", Userid);            
            proc.AddPara("@BRANCHID", Branch_Id);
            proc.AddPara("@UPDATESTATUS", UPDATESTATUS);
            ds = proc.GetTable();

            return ds;
        }

        public ActionResult PartialOrderProductDetails()
        {
            string IsDiscountInOrder = objSystemSettings.GetSystemSettingsResult("IsDiscountInOrder");//REV 1.0
            string IsViewMRPInOrder = objSystemSettings.GetSystemSettingsResult("IsViewMRPInOrder");//REV 1.0
            List<OrderDetailsSummaryProducts> oproduct = new List<OrderDetailsSummaryProducts>();
            try
            {
                string Is_PageLoad = string.Empty;
                String weburl = System.Configuration.ConfigurationSettings.AppSettings["SiteURL"];
                List<GpsStatusClasstOutput> omel = new List<GpsStatusClasstOutput>();

                DataTable dt = new DataTable();
                DataTable dtproduct = new DataTable();
                dtproduct = objshop.GetProducts();
                List<Productlist_Order> oproductlist = new List<Productlist_Order>();

                oproductlist = APIHelperMethods.ToModelList<Productlist_Order>(dtproduct);

                mproductwindow.products = oproductlist;

               
                ViewBag.IsDiscountInOrder = IsDiscountInOrder;
                ViewBag.IsViewMRPInOrder = IsViewMRPInOrder;
                
                return PartialView("_PartialOrderProductDetails", mproductwindow);

            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });

            }
        }
        public ActionResult PartialOrderSummaryAllProducts(string OrderId = null)
        {

            try
            {
                string IsDiscountInOrder = objSystemSettings.GetSystemSettingsResult("IsDiscountInOrder");//REV 1.0
                string IsViewMRPInOrder = objSystemSettings.GetSystemSettingsResult("IsViewMRPInOrder");//REV 1.0
                string Is_PageLoad = string.Empty;
                String weburl = System.Configuration.ConfigurationSettings.AppSettings["SiteURL"];
                List<GpsStatusClasstOutput> omel = new List<GpsStatusClasstOutput>();

                DataTable dt = new DataTable();
                if (!string.IsNullOrEmpty(OrderId))
                {
                    dt = objshop.GetallorderDetails(Int32.Parse(OrderId));
                    mproductwindow.productdetails = APIHelperMethods.ToModelList<OrderDetailsSummaryProductslist>(dt);
                }
                ViewBag.IsDiscountInOrder = IsDiscountInOrder;
                ViewBag.IsViewMRPInOrder = IsViewMRPInOrder;
                
                return PartialView("_PartialOrderSummaryAllProducts", mproductwindow.productdetails);

            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });

            }
        }



        //public ActionResult DeleteOrder(string OrderId)
        //{
        //    int output = objshop.OrderDelete(OrderId);
        //    if (output > 0)
        //    {
        //        return Json("Success");
        //    }
        //    else
        //    {
        //        return Json("failure");
        //    }
        //}
       
        public JsonResult PrintSalesOrder(string OrderId)
        {

            string[] filePaths = new string[] { };
            string DesignPath = "";
            if (ConfigurationManager.AppSettings["IsDevelopedZone"] != null)
            {
                DesignPath = @"Reports\Reports\RepxReportDesign\OrderSummary\DocDesign\Designes";
            }
            else
            {
                DesignPath = @"Reports\RepxReportDesign\OrderSummary\DocDesign\Designes";
            }
            //DesignPath = @"Reports\Reports\RepxReportDesign\OrderSummary\DocDesign\Designes";
            string fullpath = Server.MapPath("~");
            fullpath = fullpath.Replace("ERP.UI\\", "");
            //fullpath = "D:\\FTS-GIT\\";
            string DesignFullPath = fullpath + DesignPath;
            filePaths = System.IO.Directory.GetFiles(DesignFullPath, "*.repx");
            List<DesignList> Listobj = new List<DesignList>();
            DesignList desig = new DesignList();
            foreach (string filename in filePaths)
            {
                //Rev 2.0
                desig = new DesignList();
                //Rev 2.0 End
                string reportname = Path.GetFileNameWithoutExtension(filename);
                string name = "";
                if (reportname.Split('~').Length > 1)
                {
                    name = reportname.Split('~')[0];
                }
                else
                {
                    name = reportname;
                }
                string reportValue = reportname;

                desig.name = name;
                desig.reportValue = reportname;
                Listobj.Add(desig);

            }
            //CmbDesignName.SelectedIndex = 0;
            return Json(Listobj, JsonRequestBehavior.AllowGet);
        }
        public class DesignList
        {
            public string name { get; set; }
            public string reportValue { get; set; }

        }
      
        public ActionResult DeleteProduct(int OrderId, int ProdID)
        {
            int output = objshop.OrderProductModifyDelete(ProdID, OrderId, 0, 0, 0, 0, "Delete");
            if (output > 0)
            {
                return Json("Success");
            }
            else
            {
                return Json("failure");
            }
        }

        public ActionResult EditOrderProducts(string OrderId, string ProdID)
        {

            List<OrderDetailsSummaryProducts> oproduct = new List<OrderDetailsSummaryProducts>();
            try
            {
                string Is_PageLoad = string.Empty;
                String weburl = System.Configuration.ConfigurationSettings.AppSettings["SiteURL"];
                List<GpsStatusClasstOutput> omel = new List<GpsStatusClasstOutput>();

                DataTable dt = new DataTable();
                DataTable dtproduct = new DataTable();
                dtproduct = objshop.GetProducts();
                List<Productlist_Order> oproductlist = new List<Productlist_Order>();
                oproductlist = APIHelperMethods.ToModelList<Productlist_Order>(dtproduct);

                dtquery = objshop.OrderProductFetch(ProdID, OrderId, "Edit");
                mproductwindow = APIHelperMethods.ToModel<OrderDetailsSummaryProducts>(dtquery);
                // Mantis Issue 25478
                //mproductwindow.products = oproductlist;
                // End of Mantis Issue 25478
                return Json(mproductwindow);

            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });

            }
        }



        public ActionResult UpdateOrderProduct(OrderDetailsSummaryProducts model)
        {
            if (model.Order_ProdId != 0)
            {
                //REV 1.0
                //int output = objshop.OrderProductModifyDelete(model.Order_ProdId, model.Order_ID, model.Product_Qty, model.Product_Rate, "Update");
                int output = objshop.OrderProductModifyDelete(model.Order_ProdId, model.Order_ID, model.Product_Qty, model.Product_Rate, model.Product_MRP, model.Product_Discount, "Update");
                //REV 1.0 END
                if (output > 0)
                {
                    return Json("Success");
                }
                else
                {
                    return Json("failure");
                }
            }
            else
            {
                return Json("failure");
            }
        }



        public ActionResult ExporOrdrSummaryList(int type)
        {
            ViewData["UpdateOrderStatusList"] = TempData["UpdateOrderStatusList"];
            switch (type)
            {
                case 1:
                    return GridViewExtension.ExportToPdf(GetOrderRegisterList(), ViewData["UpdateOrderStatusList"]);
                //break;
                case 2:
                    return GridViewExtension.ExportToXlsx(GetOrderRegisterList(), ViewData["UpdateOrderStatusList"]);
                //break;
                case 3:
                    return GridViewExtension.ExportToXls(GetOrderRegisterList(), ViewData["UpdateOrderStatusList"]);
                case 4:
                    return GridViewExtension.ExportToRtf(GetOrderRegisterList(), ViewData["UpdateOrderStatusList"]);
                case 5:
                    return GridViewExtension.ExportToCsv(GetOrderRegisterList(), ViewData["UpdateOrderStatusList"]);
                //break;

                default:
                    break;
            }
            TempData["UpdateOrderStatusList"] = ViewData["UpdateOrderStatusList"];
            return null;
        }
        private GridViewSettings GetOrderRegisterList()
        {
            
            DataTable dtColmn = objshop.GetPageRetention(Session["userid"].ToString(), "UPDATE ORDER STATUS");
            if (dtColmn != null && dtColmn.Rows.Count > 0)
            {
                ViewBag.RetentionColumn = dtColmn;
            }
           

            var settings = new GridViewSettings();
            settings.Name = "gridsummarylist";
           
            settings.SettingsExport.ExportedRowType = GridViewExportedRowType.All;
            settings.SettingsExport.FileName = "Update Order Status";

               
                settings.Columns.Add(x =>
                {
                    x.FieldName = "EmployeeName";
                    x.Caption = "Employee Name";
                    x.VisibleIndex = 1;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);

                  
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='EmployeeName'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                    
                });

                settings.Columns.Add(x =>
                {
                    x.FieldName = "BRANCHDESC";
                    x.Caption = "Branch";
                    x.VisibleIndex = 2;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);

                   
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='BRANCHDESC'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                   
                });


                settings.Columns.Add(x =>
                {
                    x.FieldName = "shop_name";
                    x.Caption = "Shop Name";
                    x.VisibleIndex = 3;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);

                   
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='shop_name'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                    
                });

                settings.Columns.Add(x =>
                {
                    x.FieldName = "ENTITYCODE";
                    x.Caption = "Code";
                    x.VisibleIndex = 4;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);

                  
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='ENTITYCODE'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                   
                });

                settings.Columns.Add(x =>
                {
                    x.FieldName = "address";
                    x.Caption = "Address";
                    x.VisibleIndex = 5;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(20);

                  
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='address'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                   

                });
                settings.Columns.Add(x =>
                {
                    x.FieldName = "owner_contact_no";
                    x.Caption = "Contact";
                    x.VisibleIndex = 6;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);

                    
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='owner_contact_no'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                    
                });

                settings.Columns.Add(x =>
                {
                    x.FieldName = "Shoptype";
                    x.Caption = "Shop type";
                    x.VisibleIndex = 7;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);

                   
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='Shoptype'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                    
                });


                settings.Columns.Add(x =>
                {
                    x.FieldName = "date";
                    x.Caption = "Order Date";
                    x.VisibleIndex = 8;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);

                  
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='date'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                    
                });


                settings.Columns.Add(x =>
                {
                    x.FieldName = "OrderCode";
                    x.Caption = "Order Number";
                    x.VisibleIndex = 9;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(15);

                   
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='OrderCode'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                    
                });

                settings.Columns.Add(x =>
                {
                    x.FieldName = "order_amount";
                    x.Caption = "Order Value";
                    x.VisibleIndex = 10;
                    x.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);
                    x.PropertiesEdit.DisplayFormatString = "0.00";

                  
                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='order_amount'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }
                    
                });

                settings.Columns.Add(x =>
                {
                    x.FieldName = "ORDERSTATUS";
                    x.Caption = "Order Status";
                    x.VisibleIndex = 11;
                    x.HeaderStyle.HorizontalAlign = System.Web.UI.WebControls.HorizontalAlign.Right;
                    x.Width = System.Web.UI.WebControls.Unit.Percentage(10);
                    //x.PropertiesEdit.DisplayFormatString = "0.00";


                    if (ViewBag.RetentionColumn != null)
                    {
                        System.Data.DataRow[] row = ViewBag.RetentionColumn.Select("ColumnName='ORDERSTATUS'");
                        if (row != null && row.Length > 0)
                        {
                            x.Visible = false;
                        }
                        else
                        {
                            x.Visible = true;
                        }
                    }
                    else
                    {
                        x.Visible = true;
                    }

                });
            settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            settings.SettingsExport.LeftMargin = 20;
            settings.SettingsExport.RightMargin = 20;
            settings.SettingsExport.TopMargin = 20;
            settings.SettingsExport.BottomMargin = 20;

            return settings;
        }

        public ActionResult getMrpDiscount(OrderDetailsSummaryProducts model)
        {
            if (model.Order_ProdId != 0)
            {
                dtquery = objshop.getMrpDiscount(model.Product_Id, "ProductIdWiseMrpDiscount");
                _productDetails = APIHelperMethods.ToModel<ProductDetails>(dtquery);
                return Json(_productDetails);
            }
            else
            {
                return Json("failure");
            }
        }

       
        public ActionResult PageRetention(List<String> Columns)
        {
            try
            {
                String Col = "";
                int i = 1;
                if (Columns != null && Columns.Count > 0)
                {
                    Col = string.Join(",", Columns);
                }
                int k = objshop.InsertPageRetention(Col, Session["userid"].ToString(), "ORDER SUMMARY");

                return Json(k, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return RedirectToAction("Logout", "Login", new { Area = "" });
            }
        }
        
    }
}