using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using TargetVsAchievement.Models;

namespace TargetVsAchievement.Areas.TargetVsAchievement.Controllers
{
    public class ProductTargetAddEditController : Controller
    {
        ProductTargetAddModel objdata = null;

        // GET: TargetVsAchievement/ProductTargetAddEdit
        Int32 DetailsID = 0;
        string ProductTargetNo = string.Empty;
        public ProductTargetAddEditController()
        {
            objdata = new ProductTargetAddModel();
            // objdata = new ProductTargetProduct();
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

            TempData["DetailsID"] = null;
            TempData.Keep();

            return View(objdata);
            
        }
        public ActionResult EDITINDEX()
        {
            EntityLayer.CommonELS.UserRightsForPage rights = BusinessLogicLayer.CommonBLS.CommonBL.GetUserRightSession("/TargetSetUp/Index");

            if (TempData["DetailsID"] != null)
            {
                objdata.PRODUCTTARGET_ID = Convert.ToInt64(TempData["DetailsID"]);
                TempData.Keep();

                if (Convert.ToInt64(objdata.PRODUCTTARGET_ID) > 0)
                {
                    DataTable objData = objdata.GETTARGETASSIGNDETAILSBYID("GETHEADERPRODUCTTARGET", objdata.PRODUCTTARGET_ID);
                    if (objData != null && objData.Rows.Count > 0)
                    {
                        DataTable dt = objData;
                        foreach (DataRow row in dt.Rows)
                        {
                            objdata.PRODUCTTARGET_ID = Convert.ToInt64(row["PRODUCTTARGET_ID"]);
                            objdata.ProductTargetLevel = Convert.ToString(row["TARGETLEVEL"]);
                            objdata.ProductTargetNo = Convert.ToString(row["TARGETDOCNUMBER"]);
                            objdata.ProductTargetDate = Convert.ToDateTime(row["TARGETDATE"]);
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
            return PartialView("~/Areas/TargetVsAchievement/Views/ProductTargetAddEdit/Index.cshtml", objdata);

        }
        public ActionResult GetProductTargetEntryList()
        {
            ProductTargetDetails productdataobj = new ProductTargetDetails();
            List<ProductTargetDetails> productdata = new List<ProductTargetDetails>();
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
                    DataTable objData = objdata.GETTARGETASSIGNDETAILSBYID("GETDETAILSPRODUCTTARGET", DetailsID);
                    if (objData != null && objData.Rows.Count > 0)
                    {
                        DataTable dt = objData;


                        foreach (DataRow row in dt.Rows)
                        {
                            productdataobj = new ProductTargetDetails();
                            productdataobj.SlNO = Convert.ToString(row["SlNO"]);
                            productdataobj.ActualSL = Convert.ToString(row["PRODUCTTARGETDETAILS_ID"]);
                            productdataobj.TARGETDOCNUMBER = Convert.ToString(row["TARGETDOCNUMBER"]);
                            productdataobj.TARGETLEVELID = Convert.ToString(row["TARGETLEVELID"]);
                            productdataobj.TARGETLEVEL = Convert.ToString(row["TARGETLEVEL"]);
                            productdataobj.INTERNALID = Convert.ToString(row["INTERNALID"]);

                            productdataobj.TIMEFRAME = Convert.ToString(row["TIMEFRAME"]);
                            productdataobj.STARTEDATE = Convert.ToDateTime(row["STARTEDATE"]);
                            productdataobj.ENDDATE = Convert.ToDateTime(row["ENDDATE"]);
                            productdataobj.TARGETPRODUCTCODE = Convert.ToString(row["TARGETPRODUCTCODE"]);

                            productdataobj.TARGETPRODUCTNAME = Convert.ToString(row["TARGETPRODUCTNAME"]);
                            productdataobj.ORDERAMOUNT = Convert.ToDecimal(row["ORDERAMOUNT"]);
                            productdataobj.PRODUCTID = Convert.ToString(row["PRODUCTID"]);
                            productdataobj.ORDERQTY = Convert.ToDecimal(row["ORDERQTY"]);

                            productdata.Add(productdataobj);

                        }

                    }
                }

            }
            catch { }
            return PartialView("~/Areas/TargetVsAchievement/Views/ProductTargetAddEdit/PartialProductTargetEntryGrid.cshtml", productdata);

        }

        [HttpPost, ValidateInput(false)]
        public ActionResult BatchEditingUpdateProductTargetEntry(DevExpress.Web.Mvc.MVCxGridViewBatchUpdateValues<ProductTargetDetails, int> updateValues, ProductTargetAddModel options)
        {
            TempData["Count"] = (int)TempData["Count"] + 1;
            TempData.Keep();

            String Message = "";
            Int64 SaveDataArea = 0;

            List<udtProductAddTarget> udt = new List<udtProductAddTarget>();

            if ((int)TempData["Count"] != 2)
            {
                Boolean IsProcess = false;

                if (updateValues.Insert.Count > 0 && Convert.ToInt64(options.PRODUCTTARGET_ID) < 1)
                {
                    List<ProductTargetDetails> udtlist = new List<ProductTargetDetails>();
                    ProductTargetDetails obj = null;

                    foreach (var item in updateValues.Insert)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new ProductTargetDetails();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.INTERNALID = item.INTERNALID;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.PRODUCTID = item.PRODUCTID;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
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
                            udtProductAddTarget obj1 = new udtProductAddTarget();
                            obj1.TARGETLEVELID = Convert.ToInt64(item.TARGETLEVELID);
                            obj1.TARGETLEVEL = item.TARGETLEVEL;
                            obj1.INTERNALID = item.INTERNALID;
                            obj1.TIMEFRAME = item.TIMEFRAME;
                            obj1.STARTEDATE = item.STARTEDATE;
                            obj1.ENDDATE = item.ENDDATE;
                            obj1.PRODUCTID = item.PRODUCTID;
                            obj1.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj1.ORDERQTY = item.ORDERQTY;
                            obj1.SlNO = item.SlNO;
                            udt.Add(obj1);
                        }
                        IsProcess = ProductTargetInsertUpdate(udt, options);


                    }

                }
                if (((updateValues.Update.Count > 0 && Convert.ToInt64(options.PRODUCTTARGET_ID) > 0) || (updateValues.Insert.Count > 0 && Convert.ToInt64(options.PRODUCTTARGET_ID) < 1)) && SaveDataArea == 0)
                {
                    List<ProductTargetDetails> udtlist = new List<ProductTargetDetails>();
                    ProductTargetDetails obj = null;
                    foreach (var item in updateValues.Update)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new ProductTargetDetails();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.INTERNALID = item.INTERNALID;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj.PRODUCTID = item.PRODUCTID;
                            obj.ORDERQTY = item.ORDERQTY;
                            obj.ActualSL = item.ActualSL;
                            udtlist.Add(obj);
                        }
                    }

                    foreach (var item in updateValues.Insert)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new ProductTargetDetails();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.INTERNALID = item.INTERNALID;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj.PRODUCTID = item.PRODUCTID;
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
                            udtProductAddTarget obj1 = new udtProductAddTarget();
                            obj1.TARGETLEVELID = Convert.ToInt64(item.TARGETLEVELID);
                            obj1.TARGETLEVEL = item.TARGETLEVEL;
                            obj1.TIMEFRAME = item.TIMEFRAME;
                            obj1.INTERNALID = item.INTERNALID;
                            obj1.STARTEDATE = item.STARTEDATE;
                            obj1.ENDDATE = item.ENDDATE;
                            obj1.ORDERAMOUNT = item.ORDERAMOUNT;
                            obj1.PRODUCTID = item.PRODUCTID;
                            obj1.ORDERQTY = item.ORDERQTY;
                            obj1.SlNO = item.SlNO;
                            udt.Add(obj1);
                        }

                        IsProcess = ProductTargetInsertUpdate(udt, options);
                    }
                }

                TempData["Count"] = 1;
                TempData.Keep();

                ViewData["DetailsID"] = DetailsID;
                ViewData["ProductTargetNo"] = options.ProductTargetNo;
                ViewData["Success"] = IsProcess;
                ViewData["Message"] = Message;
            }

            return PartialView("~/Areas/TargetVsAchievement/Views/ProductTargetAddEdit/PartialProductTargetEntryGrid.cshtml", updateValues.Update);
        }

        public Boolean ProductTargetInsertUpdate(List<udtProductAddTarget> obj, ProductTargetAddModel obj2)
        {
            Boolean Success = false;
            try
            {
                DataTable dtProductTarget = new DataTable();
                dtProductTarget = ToDataTable(obj);
                DataColumnCollection dtC = dtProductTarget.Columns;
                if (dtC.Contains("UpdateEdit"))
                {
                    dtProductTarget.Columns.Remove("UpdateEdit");
                }
                if (dtC.Contains("PRODUCTADDTARGETDETAILS_ID"))
                {
                    dtProductTarget.Columns.Remove("PRODUCTADDTARGETDETAILS_ID");
                }


                DataSet dt = new DataSet();

                if (Convert.ToInt64(obj2.PRODUCTTARGET_ID) > 0 && Convert.ToInt16(TempData["IsView"]) == 0)
                {
                    dt = objdata.ProductTargetEntryInsertUpdate("UPDATEMAINPRODUCT", Convert.ToDateTime(obj2.ProductTargetDate), Convert.ToInt64(obj2.PRODUCTTARGET_ID), obj2.ProductTargetLevel, obj2.ProductTargetNo
                           , dtProductTarget, Convert.ToInt64(Session["userid"]));
                }
                else
                {
                    dt = objdata.ProductTargetEntryInsertUpdate("INSERTMAINPRODUCT", Convert.ToDateTime(obj2.ProductTargetDate), Convert.ToInt64(obj2.PRODUCTTARGET_ID), obj2.ProductTargetLevel, obj2.ProductTargetNo
                           , dtProductTarget, Convert.ToInt64(Session["userid"]));

                }
                if (dt != null && dt.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Tables[0].Rows)
                    {
                        Success = Convert.ToBoolean(row["Success"]);
                        DetailsID = Convert.ToInt32(row["DetailsID"]);
                        ProductTargetNo = Convert.ToString(obj2.ProductTargetNo);
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

        public JsonResult CHECKUNIQUETARGETDOCNUMBER(string SalesTargetNo, string TargetID)
        {

            var retData = 0;
            try
            {
                ProcedureExecute proc;
                using (proc = new ProcedureExecute("PRC_PRODUCTTARGETASSIGN"))
                {
                    proc.AddVarcharPara("@action", 100, "CHECKUNIQUETARGETDOCNUMBER");
                    proc.AddIntegerPara("@ReturnValue", 0, QueryParameterDirection.Output);
                    proc.AddVarcharPara("@ProductTargetNo", 100, SalesTargetNo);
                    proc.AddVarcharPara("@PRODUCTTARGET_ID", 100, TargetID);
                    int i = proc.RunActionQuery();
                    retData = Convert.ToInt32(proc.GetParaValue("@ReturnValue"));

                }
            }
            catch { }
            return Json(retData);
        }
    }
}