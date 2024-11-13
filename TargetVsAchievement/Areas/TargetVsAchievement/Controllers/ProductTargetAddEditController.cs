using DataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Services;
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

            TempData["LevelDetails"] = null;
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
                DataTable dt = new DataTable();

                if (TempData["DetailsID"] != null)
                {
                    DetailsID = Convert.ToInt64(TempData["DetailsID"]);
                    TempData.Keep();
                }
                if (DetailsID > 0 && TempData["LevelDetails"] == null)
                {
                    DataTable objData = objdata.GETTARGETASSIGNDETAILSBYID("GETDETAILSPRODUCTTARGET", DetailsID);
                    if (objData != null && objData.Rows.Count > 0)
                    {
                        dt = objData;

                        DataTable dtable = new DataTable();

                        dtable.Clear();
                        dtable.Columns.Add("HIddenID", typeof(System.Guid));
                        dtable.Columns.Add("SlNO", typeof(System.String));
                        dtable.Columns.Add("TARGETLEVEL", typeof(System.String));
                        dtable.Columns.Add("TIMEFRAME", typeof(System.String));
                        dtable.Columns.Add("STARTEDATE", typeof(System.String));
                        dtable.Columns.Add("ENDDATE", typeof(System.String));
                        dtable.Columns.Add("TARGETLEVELID", typeof(System.String));
                        dtable.Columns.Add("INTERNALID", typeof(System.String));
                        dtable.Columns.Add("BRANDID", typeof(System.String));
                        dtable.Columns.Add("BRANDNAME", typeof(System.String));
                        dtable.Columns.Add("ORDERAMOUNT", typeof(System.String));
                        dtable.Columns.Add("ORDERQTY", typeof(System.String));

                        String Gid = "";

                        foreach (DataRow row in dt.Rows)
                        {
                            Gid = Guid.NewGuid().ToString();
                            productdataobj = new ProductTargetDetails();
                            productdataobj.SlNO = Convert.ToString(row["SlNO"]);
                            //productdataobj.ActualSL = Convert.ToString(row["PRODUCTTARGETDETAILS_ID"]);
                            productdataobj.TARGETDOCNUMBER = Convert.ToString(row["TARGETDOCNUMBER"]);
                            productdataobj.TARGETLEVELID = Convert.ToString(row["TARGETLEVELID"]);
                            productdataobj.TARGETLEVEL = Convert.ToString(row["TARGETLEVEL"]);
                            productdataobj.INTERNALID = Convert.ToString(row["INTERNALID"]);

                            productdataobj.TIMEFRAME = Convert.ToString(row["TIMEFRAME"]);
                            productdataobj.STARTEDATE = Convert.ToString(row["STARTEDATE"]);
                            productdataobj.ENDDATE = Convert.ToString(row["ENDDATE"]);

                            productdataobj.PRODUCTID = Convert.ToString(row["PRODUCTID"]);
                            productdataobj.PRODUCTCODE = Convert.ToString(row["PRODUCTCODE"]);
                            productdataobj.PRODUCTNAME = Convert.ToString(row["PRODUCTNAME"]);
                            productdataobj.ORDERAMOUNT = Convert.ToString(row["ORDERAMOUNT"]);
                            productdataobj.ORDERQTY = Convert.ToString(row["ORDERQTY"]);

                            productdataobj.Guids = Gid;

                            productdata.Add(productdataobj);

                            object[] trow = { Gid, row["SlNO"] , Convert.ToString(row["TARGETLEVEL"]), Convert.ToString(row["TIMEFRAME"]),
                                    Convert.ToString(row["STARTEDATE"]), Convert.ToString(row["ENDDATE"]),
                                    Convert.ToString(row["TARGETLEVELID"]), Convert.ToString(row["INTERNALID"]),
                                    Convert.ToString(row["PRODUCTID"]), Convert.ToString(row["PRODUCTCODE"]), Convert.ToString(row["PRODUCTNAME"]),  
                                    Convert.ToString(row["ORDERAMOUNT"]),Convert.ToString(row["ORDERQTY"]) };

                            dtable.Rows.Add(trow);

                        }

                        dt = dtable;
                    }
                }
                else
                {
                    dt = (DataTable)TempData["LevelDetails"];

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            productdataobj = new ProductTargetDetails();
                            productdataobj.SlNO = Convert.ToString(row["SlNO"]);
                            //productdataobj.ActualSL = Convert.ToString(row["WODTARGETDETAILS_ID"]);
                            //productdataobj.TARGETDOCNUMBER = Convert.ToString(row["TARGETDOCNUMBER"]);
                            productdataobj.TARGETLEVELID = Convert.ToString(row["TARGETLEVELID"]);
                            productdataobj.TARGETLEVEL = Convert.ToString(row["TARGETLEVEL"]);
                            productdataobj.INTERNALID = Convert.ToString(row["INTERNALID"]);

                            productdataobj.TIMEFRAME = Convert.ToString(row["TIMEFRAME"]);
                            productdataobj.STARTEDATE = Convert.ToString(row["STARTEDATE"]);
                            productdataobj.ENDDATE = Convert.ToString(row["ENDDATE"]);

                            productdataobj.PRODUCTID = Convert.ToString(row["PRODUCTID"]);
                            productdataobj.PRODUCTCODE = Convert.ToString(row["PRODUCTCODE"]);
                            productdataobj.PRODUCTNAME = Convert.ToString(row["PRODUCTNAME"]);
                            productdataobj.ORDERAMOUNT = Convert.ToString(row["ORDERAMOUNT"]);
                            productdataobj.ORDERQTY = Convert.ToString(row["ORDERQTY"]);

                            productdataobj.Guids = Convert.ToString(row["HIddenID"]);
                            productdata.Add(productdataobj);

                        }
                    }


                }
                TempData["LevelDetails"] = dt;
                TempData.Keep();

            }
            catch { }
            return PartialView("~/Areas/TargetVsAchievement/Views/ProductTargetAddEdit/PartialProductTargetEntryGrid.cshtml", productdata);

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
                TempData["LevelDetails"] = null;
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

        [WebMethod]
        public JsonResult AddLevelDetails(ProductTargetDetails prod)
        {
            DataTable dt = (DataTable)TempData["LevelDetails"];
            DataTable dt2 = new DataTable();

            if (dt == null)
            {
                DataTable dtable = new DataTable();

                dtable.Clear();
                dtable.Columns.Add("HIddenID", typeof(System.Guid));
                dtable.Columns.Add("SlNO", typeof(System.String));
                dtable.Columns.Add("TARGETLEVEL", typeof(System.String));
                dtable.Columns.Add("TIMEFRAME", typeof(System.String));
                dtable.Columns.Add("STARTEDATE", typeof(System.String));
                dtable.Columns.Add("ENDDATE", typeof(System.String));
                dtable.Columns.Add("TARGETLEVELID", typeof(System.String));
                dtable.Columns.Add("INTERNALID", typeof(System.String));
                dtable.Columns.Add("PRODUCTID", typeof(System.String));
                dtable.Columns.Add("PRODUCTNAME", typeof(System.String));
                dtable.Columns.Add("PRODUCTCODE", typeof(System.String));
                dtable.Columns.Add("ORDERAMOUNT", typeof(System.String));
                dtable.Columns.Add("ORDERQTY", typeof(System.String));


                object[] trow = { Guid.NewGuid(), 1, prod.TARGETLEVEL, prod.TIMEFRAME,
                        DateTime.ParseExact(prod.STARTEDATE, "yyyy-MM-dd", null).ToString("dd-MM-yyyy"),
                        DateTime.ParseExact(prod.ENDDATE, "yyyy-MM-dd", null).ToString("dd-MM-yyyy"),
                        prod.TARGETLEVELID, prod.INTERNALID,
                        prod.PRODUCTID, prod.PRODUCTNAME, prod.PRODUCTCODE, prod.ORDERAMOUNT, prod.ORDERQTY };
                dtable.Rows.Add(trow);
                TempData["LevelDetails"] = dtable;
                TempData.Keep();
            }
            else
            {
                if (string.IsNullOrEmpty(prod.Guids))
                {
                    object[] trow = { Guid.NewGuid(), Convert.ToInt32(dt.Rows.Count) + 1, prod.TARGETLEVEL, prod.TIMEFRAME,
                        DateTime.ParseExact(prod.STARTEDATE, "yyyy-MM-dd", null).ToString("dd-MM-yyyy"),
                        DateTime.ParseExact(prod.ENDDATE, "yyyy-MM-dd", null).ToString("dd-MM-yyyy"),
                        prod.TARGETLEVELID, prod.INTERNALID,
                        prod.PRODUCTID, prod.PRODUCTNAME, prod.PRODUCTCODE, prod.ORDERAMOUNT, prod.ORDERQTY};


                    dt.Rows.Add(trow);
                    TempData["LevelDetails"] = dt;
                    TempData.Keep();
                }
                else
                {
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow item in dt.Rows)
                        {
                            if (prod.Guids.ToString() == item["HIddenID"].ToString())
                            {
                                // item["SlNO"] = prod.SlNO;
                                item["TARGETLEVEL"] = prod.TARGETLEVEL;
                                item["TIMEFRAME"] = prod.TIMEFRAME;
                                item["STARTEDATE"] = DateTime.ParseExact(prod.STARTEDATE, "yyyy-MM-dd", null).ToString("dd-MM-yyyy");
                                item["ENDDATE"] = DateTime.ParseExact(prod.ENDDATE, "yyyy-MM-dd", null).ToString("dd-MM-yyyy");
                                item["TARGETLEVELID"] = prod.TARGETLEVELID;
                                item["INTERNALID"] = prod.INTERNALID;
                                item["PRODUCTID"] = prod.PRODUCTID;
                                item["PRODUCTNAME"] = prod.PRODUCTNAME;
                                item["PRODUCTCODE"] = prod.PRODUCTCODE;
                                item["ORDERAMOUNT"] = prod.ORDERAMOUNT;
                                item["ORDERQTY"] = prod.ORDERQTY;
                            }
                        }
                    }
                }
                TempData["LevelDetails"] = dt;
                TempData.Keep();
            }
            return Json("");
        }
    }
}