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

            if (TempData["DetailsID"] != null)
            {
                objdata.SALESTARGET_ID = Convert.ToInt64(TempData["DetailsID"]);
                TempData.Keep();

                if (Convert.ToInt64(objdata.SALESTARGET_ID) > 0)
                {
                    DataTable objData = objdata.GETSALESTARGETASSIGNDETAILSBYID("GETHEADERSALESTARGET", objdata.SALESTARGET_ID);
                    if (objData != null && objData.Rows.Count > 0)
                    {
                        DataTable dt = objData;
                        foreach (DataRow row in dt.Rows)
                        {
                            objdata.SALESTARGET_ID = Convert.ToInt64(row["SALESTARGET_ID"]);
                            objdata.SalesTargetLevel = Convert.ToString(row["SalesTargetLevel"]);
                            objdata.SalesTargetNo = Convert.ToString(row["SalesTargetNo"]);
                            objdata.SalesTargetDate = Convert.ToDateTime(row["SalesTargetDate"]); 
                        }
                    }
                }
            }
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
                if (TempData["DetailsID"] != null)
                {
                    DetailsID = Convert.ToInt64(TempData["DetailsID"]);
                    TempData.Keep();
                }
                if (DetailsID > 0)
                {
                    DataTable objData = objdata.GETSALESTARGETASSIGNDETAILSBYID("GETDETAILSSALESTARGET", DetailsID);
                    if (objData != null && objData.Rows.Count > 0)
                    {
                        DataTable dt = objData;


                        foreach (DataRow row in dt.Rows)
                        {
                            productdataobj = new SalesTargetProduct();
                            productdataobj.SlNO = Convert.ToString(row["SlNO"]);
                            productdataobj.ActualSL = Convert.ToString(row["SALESTARGETDETAILS_ID"]);
                            productdataobj.TARGETDOCNUMBER = Convert.ToString(row["TARGETDOCNUMBER"]);
                            productdataobj.TARGETLEVELID = Convert.ToString(row["TARGETLEVELID"]);
                            productdataobj.TARGETLEVEL = Convert.ToString(row["TARGETLEVEL"]);
                            productdataobj.INTERNALID = Convert.ToString(row["INTERNALID"]);

                            productdataobj.TIMEFRAME = Convert.ToString(row["TIMEFRAME"]);
                            productdataobj.STARTEDATE = Convert.ToDateTime(row["TARGETLEVELID"]);
                            productdataobj.ENDDATE = Convert.ToDateTime(row["TARGETDOCNUMBER"]);
                            productdataobj.NEWVISIT = Convert.ToInt64(row["TARGETLEVELID"]);

                            productdataobj.REVISIT = Convert.ToInt64(row["TIMEFRAME"]);
                            productdataobj.ORDERAMOUNT = Convert.ToDecimal(row["TARGETLEVELID"]);
                            productdataobj.COLLECTION = Convert.ToDecimal(row["TARGETDOCNUMBER"]);
                            productdataobj.ORDERQTY = Convert.ToDecimal(row["TARGETLEVELID"]);

                            productdata.Add(productdataobj);

                        }
                        //ViewData["BOMEntryProductsTotalAm"] = bomproductdata.Sum(x => Convert.ToDecimal(x.Amount)).ToString();
                    }
                }

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
                            obj.TARGETLEVEL = item.TARGETLEVEL; 
                            obj.INTERNALID = item.INTERNALID;
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
                                obj1.TARGETLEVEL = item.TARGETLEVEL;
                                obj1.INTERNALID = item.INTERNALID;
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
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.INTERNALID = item.INTERNALID;
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
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.INTERNALID = item.INTERNALID;
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
                            obj1.TARGETLEVEL = item.TARGETLEVEL;
                            obj1.INTERNALID = item.INTERNALID;
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

        public JsonResult SetDataByID(Int64 detailsid = 0, Int16 IsView = 0)
        {
            Boolean Success = false;
            try
            {
                TempData["DetailsID"] = detailsid;
                TempData["IsView"] = IsView;
                TempData.Keep();
                Success = true;
            }
            catch { }
            return Json(Success);
        }
    }
}