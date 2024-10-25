using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TargetVsAchievement.Models;


using BusinessLogicLayer;
using DataAccessLayer;
using DevExpress.Web;
using DevExpress.Web.Mvc;
using EntityLayer.CommonELS;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Web.Services;
using UtilityLayer;

namespace TargetVsAchievement.Areas.TargetVsAchievement.Controllers
{
    public class SalesTargetController : Controller
    {
        SalesTargetModel objdata = null;
        //SalesTargetProduct objdata = null;
        // GET: TargetVsAchievement/SalesTarget
        Int32 DetailsID = 0;
        string SalesTargetNo = string.Empty;
        public SalesTargetController()
        {
            objdata = new SalesTargetModel();
           // objdata = new SalesTargetProduct();
        }
        public ActionResult Index()
        {
            EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/TargetSetUp/Index");
            ViewBag.CanAdd = rights.CanAdd;
            ViewBag.CanView = rights.CanView;
            ViewBag.CanExport = rights.CanExport;
            ViewBag.CanEdit = rights.CanEdit;
            ViewBag.CanDelete = rights.CanDelete;

            TempData["Count"] = 1;
            TempData.Keep();

            return View();
        }
        public ActionResult GetProductEntryList()
        {
            SalesTargetProduct productdataobj = new SalesTargetProduct();
            List<SalesTargetProduct> productdata = new List<SalesTargetProduct>();
            Int64 DetailsID = 0;
            try
            {

                //if (TempData["DetailsID"] != null)
                //{
                //    DetailsID = Convert.ToInt64(TempData["DetailsID"]);
                //    TempData.Keep();
                //}

                //if (DetailsID > 0)
                //{
                //    //rev Pratik
                //    //DataTable objData = objdata.GetBOMProductEntryListByID("GetBOMEntryProductsData", DetailsID);
                //    DataTable objData = objdata.GetBOMProductEntryListByID("GETBOMENTRYPRODUCTSDATA_NEW", DetailsID);

                //    TempData["MultiUom"] = objdata.GetBOMProductEntryListByID("BOMMultiUOMDetails", DetailsID);

                //    //End of rev Pratik

                //    if (objData != null && objData.Rows.Count > 0)
                //    {
                //        DataTable dt = objData;


                //        foreach (DataRow row in dt.Rows)
                //        {
                //            bomproductdataobj = new BOMProduct();
                //            bomproductdataobj.SlNO = Convert.ToString(row["SlNO"]);
                //            bomproductdataobj.ProductName = Convert.ToString(row["sProducts_Code"]);
                //            bomproductdataobj.ProductId = Convert.ToString(row["ProductID"]);
                //            bomproductdataobj.ProductDescription = Convert.ToString(row["sProducts_Name"]);
                //            bomproductdataobj.DesignNo = Convert.ToString(row["DesignNo"]);
                //            bomproductdataobj.ItemRevisionNo = Convert.ToString(row["ItemRevisionNo"]);
                //            bomproductdataobj.ProductQty = Convert.ToString(row["StkQty"]);
                //            bomproductdataobj.ProductUOM = Convert.ToString(row["StkUOM"]);
                //            bomproductdataobj.Warehouse = Convert.ToString(row["WarehouseName"]);
                //            bomproductdataobj.Price = Convert.ToString(row["Price"]);
                //            bomproductdataobj.Amount = Convert.ToString(row["Amount"]);
                //            bomproductdataobj.BOMNo = Convert.ToString(row["BOMNo"]);
                //            bomproductdataobj.RevNo = Convert.ToString(row["RevNo"]);
                //            if (!String.IsNullOrEmpty(Convert.ToString(row["RevDate"])))
                //            {
                //                bomproductdataobj.RevDate = Convert.ToDateTime(row["RevDate"]).ToString("dd-MM-yyyy");
                //            }
                //            else
                //            {
                //                bomproductdataobj.RevDate = " ";
                //            }
                //            bomproductdataobj.Remarks = Convert.ToString(row["Remarks"]);
                //            bomproductdataobj.ProductsWarehouseID = Convert.ToString(row["WarehouseID"]);
                //            bomproductdataobj.Tag_Details_ID = Convert.ToString(row["Tag_Details_ID"]);
                //            bomproductdataobj.Tag_Production_ID = Convert.ToString(row["Tag_Production_ID"]);
                //            bomproductdataobj.RevNo = Convert.ToString(row["RevNo"]);
                //            //Rev Pratik
                //            bomproductdataobj.AltQuantity = Convert.ToString(row["AltQuantity"]);
                //            bomproductdataobj.AltUom = Convert.ToString(row["AltUom"]);
                //            bomproductdataobj.MultiUOMSelectionForManufacturing = cSOrder.GetSystemSettingsResult("MultiUOMSelectionForManufacturing");
                //            //End of rev Pratik

                //            //Rev 1.0
                //            bomproductdataobj.ActualSL = Convert.ToString(row["SlNO"]);
                //            //Rev 1.0 End
                //            bomproductdata.Add(bomproductdataobj);

                //        }
                //        ViewData["BOMEntryProductsTotalAm"] = bomproductdata.Sum(x => Convert.ToDecimal(x.Amount)).ToString();
                //    }
                //}

            }
            catch { }
            return PartialView("~/Areas/TargetVsAchievement/Views/SalesTarget/_PartialSalesTargetEntry.cshtml", productdata);
         
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult BatchEditingUpdateSalesTargetEntry(DevExpress.Web.Mvc.MVCxGridViewBatchUpdateValues<SalesTargetProduct, int> updateValues, SalesTargetModel options)
        {
            TempData["Count"] = (int)TempData["Count"] + 1;
            TempData.Keep();
            
            String Message = "";
            Int64 SaveDataArea = 0;

            List<udtSalesTarget> udt = new List<udtSalesTarget>();

            if ((int)TempData["Count"] != 2)
            {
                Boolean IsProcess = false;                
               
                if (updateValues.Insert.Count > 0 && Convert.ToInt64(options.SALESTARGET_ID) < 1)
                {
                    List<SalesTargetProduct> udtlist = new List<SalesTargetProduct>();                
                    SalesTargetProduct obj = null;
                    
                    foreach (var item in updateValues.Insert)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {                            
                            obj = new SalesTargetProduct();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.NEWVISIT = item.NEWVISIT;
                            obj.REVISIT = item.REVISIT;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj.COLLECTION = item.COLLECTION;
                            obj.ORDERQTY = item.ORDERQTY;
                            obj.SlNO = item.SlNO;
                            udtlist.Add(obj);
                        }
                    }
                    if (udtlist.Count > 0)
                    {
                            SaveDataArea = 1;             
                           
                            foreach (var item in udtlist)
                            {
                                udtSalesTarget obj1 = new udtSalesTarget();
                                obj1.TARGETLEVELID = Convert.ToInt64(item.TARGETLEVELID);
                                obj1.TIMEFRAME = item.TIMEFRAME;
                                obj1.STARTEDATE = item.STARTEDATE;
                                obj1.ENDDATE = item.ENDDATE;
                                obj1.NEWVISIT = item.NEWVISIT;
                                obj1.REVISIT = item.REVISIT;
                                obj1.ORDERAMOUNT = item.ORDERAMOUNT;
                                obj1.COLLECTION = item.COLLECTION;
                                obj1.ORDERQTY = item.ORDERQTY;
                                obj1.SlNO = item.SlNO;
                                udt.Add(obj1);
                            }
                        IsProcess = SalesTargetInsertUpdate(udt, options);


                    }

                }
                if (((updateValues.Update.Count > 0 && Convert.ToInt64(options.SALESTARGET_ID) > 0) || (updateValues.Insert.Count > 0 && Convert.ToInt64(options.SALESTARGET_ID) < 1)) && SaveDataArea == 0)
                {
                    List<SalesTargetProduct> udtlist = new List<SalesTargetProduct>();
                    SalesTargetProduct obj = null;
                    foreach (var item in updateValues.Update)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new SalesTargetProduct();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.NEWVISIT = item.NEWVISIT;
                            obj.REVISIT = item.REVISIT;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj.COLLECTION = item.COLLECTION;
                            obj.ORDERQTY = item.ORDERQTY;
                            obj.ActualSL = item.ActualSL;
                            udtlist.Add(obj);
                        }
                    }

                    foreach (var item in updateValues.Insert)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new SalesTargetProduct();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.NEWVISIT = item.NEWVISIT;
                            obj.REVISIT = item.REVISIT;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj.COLLECTION = item.COLLECTION;
                            obj.ORDERQTY = item.ORDERQTY;
                            obj.ActualSL = "0";
                            udtlist.Add(obj);
                        }
                    }
                    
                    foreach (var item in updateValues.DeleteKeys)
                    {
                        Int32 delId = Convert.ToInt32(item);

                        foreach (var item1 in udtlist.ToList())
                        {
                            Int32 delId1 = Convert.ToInt32(item1.ActualSL);

                            if (delId1 == delId)
                            {
                                udtlist.Remove(item1);
                            }
                        }
                    }
                   

                    if (udtlist.Count > 0)
                    {
                        SaveDataArea = 1;
                       
                        foreach (var item in udtlist)
                        {
                            udtSalesTarget obj1 = new udtSalesTarget();
                            obj1.TARGETLEVELID = Convert.ToInt64(item.TARGETLEVELID);
                            obj1.TIMEFRAME = item.TIMEFRAME;
                            obj1.STARTEDATE = item.STARTEDATE;
                            obj1.ENDDATE = item.ENDDATE;
                            obj1.NEWVISIT = item.NEWVISIT;
                            obj1.REVISIT = item.REVISIT;
                            obj1.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj1.COLLECTION = item.COLLECTION;
                            obj1.ORDERQTY = item.ORDERQTY;
                            obj1.SlNO = item.SlNO;
                            udt.Add(obj1);
                        }

                        IsProcess = SalesTargetInsertUpdate(udt, options);
                    }
                }

                TempData["Count"] = 1;
                TempData.Keep();
                
                ViewData["DetailsID"] = DetailsID;               
                ViewData["SalesTargetNo"] = options.SalesTargetNo;
                ViewData["Success"] = IsProcess;
                ViewData["Message"] = Message;
            }
           
            return PartialView("~/Areas/TargetVsAchievement/Views/SalesTarget/_PartialSalesTargetEntry.cshtml", updateValues.Update);
        }

        public Boolean SalesTargetInsertUpdate(List<udtSalesTarget> obj, SalesTargetModel obj2)
        {
            Boolean Success = false;
            try
            {
                DataTable dtSalesTarget = new DataTable();
                dtSalesTarget = ToDataTable(obj);
                DataColumnCollection dtC = dtSalesTarget.Columns;
                if (dtC.Contains("UpdateEdit"))
                {
                    dtSalesTarget.Columns.Remove("UpdateEdit");
                }
                if (dtC.Contains("SALESTARGETDETAILS_ID"))
                {
                    dtSalesTarget.Columns.Remove("SALESTARGETDETAILS_ID");
                }


                DataSet dt = new DataSet();
               
                if (Convert.ToInt64(obj2.SALESTARGET_ID) > 0 && Convert.ToInt16(TempData["IsView"]) == 0)
                {
                    dt = objdata.SalesTargetEntryInsertUpdate("UPDATEMAINPRODUCT",Convert.ToDateTime(obj2.SalesTargetDate), Convert.ToInt64(obj2.SALESTARGET_ID), obj2.SalesTargetLevel, obj2.SalesTargetNo
                           , dtSalesTarget, Convert.ToInt64(Session["userid"]));                    
                }   
                else
                { 
                    dt = objdata.SalesTargetEntryInsertUpdate("INSERTMAINPRODUCT", Convert.ToDateTime(obj2.SalesTargetDate), Convert.ToInt64(obj2.SALESTARGET_ID), obj2.SalesTargetLevel, obj2.SalesTargetNo
                           , dtSalesTarget, Convert.ToInt64(Session["userid"]));
                   
                }
                if (dt != null && dt.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Tables[0].Rows)
                    {
                        Success = Convert.ToBoolean(row["Success"]);                      
                        DetailsID = Convert.ToInt32(row["DetailsID"]);
                        SalesTargetNo = Convert.ToString(obj2.SalesTargetNo);
                    }
                }
            }
            catch { }
            return Success;
        }



        public DataTable ToDataTable<T>(List<T> items)
        {

            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {

                //Setting column names as Property names

                dataTable.Columns.Add(prop.Name);

            }

            foreach (T item in items)
            {

                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)
                {

                    //inserting property values to datatable rows

                    values[i] = Props[i].GetValue(item, null);

                }

                dataTable.Rows.Add(values);

            }

            //put a breakpoint here and check datatable

            return dataTable;

        }
    }
}