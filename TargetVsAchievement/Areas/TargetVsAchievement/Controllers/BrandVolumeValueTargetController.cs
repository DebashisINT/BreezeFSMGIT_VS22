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
    public class BrandVolumeValueTargetController : Controller
    {
        BrandVolumeValueTargetModel objdata = null;
        Int32 DetailsID = 0;
        string SalesTargetNo = string.Empty;
        public BrandVolumeValueTargetController()
        {
            objdata = new BrandVolumeValueTargetModel();
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
                objdata.TARGET_ID = Convert.ToInt64(TempData["DetailsID"]);
                TempData.Keep();

                if (Convert.ToInt64(objdata.TARGET_ID) > 0)
                {
                    DataTable objData = objdata.GETTARGETASSIGNDETAILSBYID("GETHEADERSALESTARGET", objdata.TARGET_ID);
                    if (objData != null && objData.Rows.Count > 0)
                    {
                        DataTable dt = objData;
                        foreach (DataRow row in dt.Rows)
                        {
                            objdata.TARGET_ID = Convert.ToInt64(row["BRANDTARGET_ID"]);
                            objdata.TargetType = Convert.ToString(row["TARGETLEVEL"]);
                            objdata.TargetNo = Convert.ToString(row["TARGETDOCNUMBER"]);
                            objdata.TargetDate = Convert.ToDateTime(row["TARGETDATE"]);
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
            return PartialView("~/Areas/TargetVsAchievement/Views/BrandVolumeValueTarget/Index.cshtml", objdata);

        }
        public ActionResult GetBrandVolumeValueTargetEntryList()
        {
            BRANDVOLUMEVALUETARGETGRIDLIST productdataobj = new BRANDVOLUMEVALUETARGETGRIDLIST();
            List<BRANDVOLUMEVALUETARGETGRIDLIST> productdata = new List<BRANDVOLUMEVALUETARGETGRIDLIST>();
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
                    DataTable objData = objdata.GETTARGETASSIGNDETAILSBYID("GETDETAILSSALESTARGET", DetailsID);
                    if (objData != null && objData.Rows.Count > 0)
                    {
                        DataTable dt = objData;


                        foreach (DataRow row in dt.Rows)
                        {
                            productdataobj = new BRANDVOLUMEVALUETARGETGRIDLIST();
                            productdataobj.SlNO = Convert.ToString(row["SlNO"]);
                            productdataobj.ActualSL = Convert.ToString(row["BRANDTARGETDETAILS_ID"]);
                            productdataobj.TARGETDOCNUMBER = Convert.ToString(row["TARGETDOCNUMBER"]);
                            productdataobj.TARGETLEVELID = Convert.ToString(row["TARGETLEVELID"]);
                            productdataobj.TARGETLEVEL = Convert.ToString(row["TARGETLEVEL"]);
                            productdataobj.INTERNALID = Convert.ToString(row["INTERNALID"]);

                            productdataobj.TIMEFRAME = Convert.ToString(row["TIMEFRAME"]);
                            productdataobj.STARTEDATE = Convert.ToDateTime(row["STARTEDATE"]);
                            productdataobj.ENDDATE = Convert.ToDateTime(row["ENDDATE"]);
                            productdataobj.BRANDID = Convert.ToInt64(row["BRANDID"]);

                            productdataobj.BrandName = Convert.ToString(row["BrandName"]); 
                            productdataobj.ORDERAMOUNT = Convert.ToDecimal(row["ORDERAMOUNT"]);
                            
                            productdataobj.ORDERQTY = Convert.ToDecimal(row["ORDERQTY"]);

                            productdata.Add(productdataobj);

                        }

                    }
                }

            }
            catch { }
            return PartialView("~/Areas/TargetVsAchievement/Views/BrandVolumeValueTarget/_PartialBrandVolumeValueTargetEntry.cshtml", productdata);

        }


        [HttpPost, ValidateInput(false)]
        public ActionResult BatchEditingUpdateTargetEntry(DevExpress.Web.Mvc.MVCxGridViewBatchUpdateValues<BRANDVOLUMEVALUETARGETGRIDLIST, int> updateValues, BrandVolumeValueTargetModel options)
        {
            TempData["Count"] = (int)TempData["Count"] + 1;
            TempData.Keep();

            String Message = "";
            Int64 SaveDataArea = 0;

            List<UDTBRANDVOLUMEVALUETARGET> udt = new List<UDTBRANDVOLUMEVALUETARGET>();

            if ((int)TempData["Count"] != 2)
            {
                Boolean IsProcess = false;

                if (updateValues.Insert.Count > 0 && Convert.ToInt64(options.TARGET_ID) < 1)
                {
                    List<BRANDVOLUMEVALUETARGETGRIDLIST> udtlist = new List<BRANDVOLUMEVALUETARGETGRIDLIST>();
                    BRANDVOLUMEVALUETARGETGRIDLIST obj = null;

                    foreach (var item in updateValues.Insert)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new BRANDVOLUMEVALUETARGETGRIDLIST();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.INTERNALID = item.INTERNALID;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.BRANDID = item.BRANDID;
                            obj.BrandName = item.BrandName;
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
                            UDTBRANDVOLUMEVALUETARGET obj1 = new UDTBRANDVOLUMEVALUETARGET();
                            obj1.TARGETLEVELID = Convert.ToInt64(item.TARGETLEVELID);
                            obj1.TARGETLEVEL = item.TARGETLEVEL;
                            obj1.INTERNALID = item.INTERNALID;
                            obj1.TIMEFRAME = item.TIMEFRAME;
                            obj1.STARTEDATE = item.STARTEDATE;
                            obj1.ENDDATE = item.ENDDATE;
                            obj1.BRANDID = item.BRANDID;
                            obj1.BrandName = item.BrandName;
                            obj1.ORDERAMOUNT = item.ORDERAMOUNT;
                            
                            obj1.ORDERQTY = item.ORDERQTY;
                            obj1.SlNO = item.SlNO;
                            udt.Add(obj1);
                        }
                        IsProcess = TargetInsertUpdate(udt, options);


                    }

                }
                if (((updateValues.Update.Count > 0 && Convert.ToInt64(options.TARGET_ID) > 0) || (updateValues.Insert.Count > 0 && Convert.ToInt64(options.TARGET_ID) < 1)) && SaveDataArea == 0)
                {
                    List<BRANDVOLUMEVALUETARGETGRIDLIST> udtlist = new List<BRANDVOLUMEVALUETARGETGRIDLIST>();
                    BRANDVOLUMEVALUETARGETGRIDLIST obj = null;
                    foreach (var item in updateValues.Update)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new BRANDVOLUMEVALUETARGETGRIDLIST();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.INTERNALID = item.INTERNALID;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.BRANDID = item.BRANDID;
                            obj.BrandName = item.BrandName;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
                            
                            obj.ORDERQTY = item.ORDERQTY;
                            obj.SlNO = item.ActualSL;
                            udtlist.Add(obj);
                        }
                    }

                    foreach (var item in updateValues.Insert)
                    {
                        if (Convert.ToInt64(item.TARGETLEVELID) > 0)
                        {
                            obj = new BRANDVOLUMEVALUETARGETGRIDLIST();
                            obj.TARGETLEVELID = item.TARGETLEVELID;
                            obj.TARGETLEVEL = item.TARGETLEVEL;
                            obj.INTERNALID = item.INTERNALID;
                            obj.TIMEFRAME = item.TIMEFRAME;
                            obj.STARTEDATE = item.STARTEDATE;
                            obj.ENDDATE = item.ENDDATE;
                            obj.BRANDID = item.BRANDID;
                            obj.BrandName = item.BrandName;
                            obj.ORDERAMOUNT = item.ORDERAMOUNT;
                           
                            obj.ORDERQTY = item.ORDERQTY;
                            obj.SlNO = "0";
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
                            UDTBRANDVOLUMEVALUETARGET obj1 = new UDTBRANDVOLUMEVALUETARGET();
                            obj1.TARGETLEVELID = Convert.ToInt64(item.TARGETLEVELID);
                            obj1.TARGETLEVEL = item.TARGETLEVEL;
                            obj1.INTERNALID = item.INTERNALID;
                            obj1.TIMEFRAME = item.TIMEFRAME;
                            obj1.STARTEDATE = item.STARTEDATE;
                            obj1.ENDDATE = item.ENDDATE;
                            obj1.BRANDID = item.BRANDID;
                            obj1.BrandName = item.BrandName;
                            obj1.ORDERAMOUNT = item.ORDERAMOUNT;
                            
                            obj1.ORDERQTY = item.ORDERQTY;
                            obj1.SlNO = item.SlNO;
                            udt.Add(obj1);
                        }

                        IsProcess = TargetInsertUpdate(udt, options);
                    }
                }

                TempData["Count"] = 1;
                TempData.Keep();
                TempData["DetailsID"] = null;
                TempData.Keep();
                ViewData["DetailsID"] = DetailsID;
                ViewData["SalesTargetNo"] = options.TargetNo;
                ViewData["Success"] = IsProcess;
                ViewData["Message"] = Message;
            }

            return PartialView("~/Areas/TargetVsAchievement/Views/BrandVolumeValueTarget/_PartialBrandVolumeValueTargetEntry.cshtml", updateValues.Update);
        }

        public Boolean TargetInsertUpdate(List<UDTBRANDVOLUMEVALUETARGET> obj, BrandVolumeValueTargetModel obj2)
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
                if (dtC.Contains("TARGETDETAILS_ID"))
                {
                    dtSalesTarget.Columns.Remove("TARGETDETAILS_ID");
                }


                DataSet dt = new DataSet();

                if (Convert.ToInt64(obj2.TARGET_ID) > 0 && Convert.ToInt16(TempData["IsView"]) == 0)
                {
                    dt = objdata.TargetEntryInsertUpdate("UPDATEBRANDTARGET", Convert.ToDateTime(obj2.TargetDate), Convert.ToInt64(obj2.TARGET_ID), obj2.TargetType, obj2.TargetNo
                           , dtSalesTarget, Convert.ToInt64(Session["userid"]));
                }
                else
                {
                    dt = objdata.TargetEntryInsertUpdate("INSERTBRANDTARGET", Convert.ToDateTime(obj2.TargetDate), Convert.ToInt64(obj2.TARGET_ID), obj2.TargetType, obj2.TargetNo
                           , dtSalesTarget, Convert.ToInt64(Session["userid"]));

                }
                if (dt != null && dt.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Tables[0].Rows)
                    {
                        Success = Convert.ToBoolean(row["Success"]);
                        DetailsID = Convert.ToInt32(row["DetailsID"]);
                        SalesTargetNo = Convert.ToString(obj2.TargetNo);
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
                using (proc = new ProcedureExecute("PRC_BRANDVOLUMEVALUETARGETASSIGN"))
                {
                    proc.AddVarcharPara("@action", 100, "CHECKUNIQUETARGETDOCNUMBER");
                    proc.AddIntegerPara("@ReturnValue", 0, QueryParameterDirection.Output);
                    proc.AddVarcharPara("@TargetNo", 100, SalesTargetNo);
                    proc.AddVarcharPara("@TARGET_ID", 100, TargetID);
                    int i = proc.RunActionQuery();
                    retData = Convert.ToInt32(proc.GetParaValue("@ReturnValue"));

                }
            }
            catch { }
            return Json(retData);
        }
    }
}