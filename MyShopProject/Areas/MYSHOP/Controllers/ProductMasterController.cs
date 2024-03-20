/***************************************************************************************************************
Written by Sanchita for  v2.0.46  on 13/03/2024     New Product Module shall be implemented for ITC Refer: 27289
***********************************************************************************************************/
using BusinessLogicLayer.SalesmanTrack;
using MyShop.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UtilityLayer;
using DataAccessLayer;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using DevExpress.Web.Mvc;
using DevExpress.Web;

namespace MyShop.Areas.MYSHOP.Controllers
{
    public class ProductMasterController : Controller
    {
        // GET: MYSHOP/ProductMaster
        public ActionResult Index()
        {
            ProductMasterModel Dtls = new ProductMasterModel();

            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("PRC_FSMPRODUCTMASTER");
            proc.AddPara("@ACTION", "GETLISTDATA");
            ds = proc.GetDataSet();

            List<ProductClassList> ProductClassLst = new List<ProductClassList>();
            ProductClassLst = APIHelperMethods.ToModelList<ProductClassList>(ds.Tables[0]);
            Dtls.ProductClassList = ProductClassLst;
            Dtls.ProductClass = 0;

            List<ProductStrengthList> ProductStrengthLst = new List<ProductStrengthList>();
            ProductStrengthLst = APIHelperMethods.ToModelList<ProductStrengthList>(ds.Tables[1]);
            Dtls.ProductStrengthList = ProductStrengthLst;
            Dtls.ProductStrength = 0;

            List<ProductUnitList> ProductUnitLst = new List<ProductUnitList>();
            ProductUnitLst = APIHelperMethods.ToModelList<ProductUnitList>(ds.Tables[2]);
            Dtls.ProductUnitList = ProductUnitLst;

            List<ProductBrandList> ProductBrandLst = new List<ProductBrandList>();
            ProductBrandLst = APIHelperMethods.ToModelList<ProductBrandList>(ds.Tables[3]);
            Dtls.ProductBrandList = ProductBrandLst;
            Dtls.ProductBrand = 0;


            List<ProductStatusList> ProductStatusLst = new List<ProductStatusList>();
            ProductStatusLst = APIHelperMethods.ToModelList<ProductStatusList>(ds.Tables[4]);
            Dtls.ProductStatusList = ProductStatusLst;
            Dtls.ProductStatus = 1;


            return View(Dtls);
        }

        public PartialViewResult _PartialProductGrid(string id, ProductMasterModel model)
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("PRC_FSMPRODUCTMASTER");
            proc.AddPara("@ACTION", "GETPRODUCTMASTERLISTDATA");
            proc.AddPara("@USER_ID", Convert.ToString(Session["userid"]));
            proc.AddPara("@IS_PAGELOAD", model.Ispageload);
            ds = proc.GetDataSet();

            return PartialView(GetProductList());
        }

        public IEnumerable GetProductList()
        {
            string connectionString = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings["DBConnectionDefault"]);
            ReportsDataContext dc = new ReportsDataContext(connectionString);
            
            int userid = Convert.ToInt32(Session["userid"]);

            var q = from d in dc.FTS_ProductMasterLists
                    where d.USERID == userid
                    orderby d.sProducts_ID descending
                    select d;
            
            return q;

        }

        public JsonResult SaveProduct(string id, string ProductCode, string ProductName, string ProductMRP, string ProductPrice,
            string ProductClass, string ProductStrength, string ProductUnit, string ProductBrand, string ProductStatus)
        {
            int output = 0;
            string Userid = Convert.ToString(Session["userid"]);

            ProcedureExecute proc;
            int rtrnvalue = 0;
            try
            {
                using (proc = new ProcedureExecute("PRC_FSMPRODUCTMASTER"))
                {
                    proc.AddVarcharPara("@PRODID", 100, id);
                    if (id == "0")
                        proc.AddVarcharPara("@ACTION", 100, "ADDPRODUCT");
                    else
                        proc.AddVarcharPara("@ACTION", 100, "UPDATEPRODUCT");
                    proc.AddVarcharPara("@ProductCode", 100, ProductCode);
                    proc.AddVarcharPara("@ProductName", 100, ProductName);
                    proc.AddVarcharPara("@ProductMRP", 100, ProductMRP);
                    proc.AddVarcharPara("@ProductPrice", 100, ProductPrice);
                    proc.AddVarcharPara("@ProductClass", 100, ProductClass);
                    proc.AddVarcharPara("@ProductStrength", 100, ProductStrength);
                    proc.AddVarcharPara("@ProductUnit", 100, ProductUnit);
                    proc.AddVarcharPara("@ProductBrand", 100, ProductBrand);
                    proc.AddVarcharPara("@ProductStatus", 100, ProductStatus);

                    proc.AddVarcharPara("@USER_ID", 100, Convert.ToString(Session["userid"]));
                    proc.AddVarcharPara("@RETURN_VALUE", 50, "", QueryParameterDirection.Output);
                    int i = proc.RunActionQuery();
                    rtrnvalue = Convert.ToInt32(proc.GetParaValue("@RETURN_VALUE"));
                    output =  rtrnvalue;


                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                proc = null;
            }



            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EditProduct(string id)
        {
            DataTable output = new DataTable();
            
            ProcedureExecute proc;
            
            DataTable dt = new DataTable();
            try
            {
                using (proc = new ProcedureExecute("PRC_FSMPRODUCTMASTER"))
                {
                    proc.AddVarcharPara("@PRODID", 100, id);
                    proc.AddVarcharPara("@ACTION", 100, "EDITPRODUCT");
                    output = proc.GetTable();
                    
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                proc = null;
            }

            if (output.Rows.Count > 0)
            {
                return Json(new { 
                    ProductCode = Convert.ToString(output.Rows[0]["sProducts_Code"]),
                    ProductMRP = Convert.ToString(output.Rows[0]["sProduct_MRP"]),
                    ProductName = Convert.ToString(output.Rows[0]["sProducts_Name"]),
                    ProductPrice = Convert.ToString(output.Rows[0]["sProduct_Price"]),
                    ProductClass = Convert.ToString(output.Rows[0]["ProductClass_Code"]),
                    ProductStrength = Convert.ToString(output.Rows[0]["sProducts_Size"]),
                    ProductUnit = Convert.ToString(output.Rows[0]["sProducts_TradingLotUnit"]),
                    ProductBrand = Convert.ToString(output.Rows[0]["sProducts_Brand"]),
                    ProductStatus = Convert.ToString(output.Rows[0]["sProduct_Status"])
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { code = "", name = "" }, JsonRequestBehavior.AllowGet);
            }

        }

        public JsonResult DeleteProduct(string ID)
        {
            int output = 0;
            
            ProcedureExecute proc;
            int rtrnvalue = 0;
            DataTable dt = new DataTable();
            try
            {
                using (proc = new ProcedureExecute("PRC_FSMPRODUCTMASTER"))
                {
                    proc.AddVarcharPara("@PRODID", 100, ID);
                    proc.AddVarcharPara("@ACTION", 100, "DELETEPRODUCT");
                    proc.AddVarcharPara("@RETURN_VALUE", 50, "", QueryParameterDirection.Output);
                    int i = proc.RunActionQuery();
                    rtrnvalue = Convert.ToInt32(proc.GetParaValue("@RETURN_VALUE"));
                    output= rtrnvalue;
                }
            }

            catch (Exception ex)
            {
                throw ex;
            }

            finally
            {
                proc = null;
            }

            return Json(output, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExporProductMasterList(int type)
        {
            switch (type)
            {
                case 1:
                    return GridViewExtension.ExportToPdf(GetDoctorBatchGridViewSettings(), GetProductList());
                case 2:
                    return GridViewExtension.ExportToXlsx(GetDoctorBatchGridViewSettings(), GetProductList());
                case 3:
                    return GridViewExtension.ExportToXls(GetDoctorBatchGridViewSettings(), GetProductList());
                case 4:
                    return GridViewExtension.ExportToRtf(GetDoctorBatchGridViewSettings(), GetProductList());
                case 5:
                    return GridViewExtension.ExportToCsv(GetDoctorBatchGridViewSettings(), GetProductList());
                default:
                    break;
            }
            return null;
        }

        private GridViewSettings GetDoctorBatchGridViewSettings()
        {
            var settings = new GridViewSettings();
            settings.Name = "gridPartyDetails";
            settings.SettingsExport.ExportedRowType = GridViewExportedRowType.All;
            settings.SettingsExport.FileName = "Product Master Details";

            settings.Columns.Add(x =>
            {
                x.FieldName = "sProducts_Code";
                x.Caption = "Item Code";
                x.VisibleIndex = 1;
                x.ExportWidth = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "sProducts_Name";
                x.Caption = "Item Name";
                x.VisibleIndex = 2;
                x.ExportWidth = 250;
            });


            settings.Columns.Add(x =>
            {
                x.FieldName = "ProductClass_Name";
                x.Caption = "Item Class/Category";
                x.VisibleIndex = 3;
                x.ExportWidth = 250;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Brand_Name";
                x.Caption = "Item Brand";
                x.VisibleIndex = 4;
                x.ExportWidth = 200;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "Size_Name";
                x.Caption = "Item Strength";
                x.VisibleIndex = 5;
                x.ExportWidth = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "sProduct_Price";
                x.Caption = "Item Price";
                x.VisibleIndex = 6;
                x.ExportWidth = 100;
                x.PropertiesEdit.DisplayFormatString = "0.00";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "sProduct_MRP";
                x.Caption = "Item MRP";
                x.VisibleIndex = 7;
                x.ExportWidth = 100;
                x.PropertiesEdit.DisplayFormatString = "0.00";
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "sProduct_Status";
                x.Caption = "Item Status";
                x.VisibleIndex = 8;
                x.ExportWidth = 100;
            });

            settings.Columns.Add(x =>
            {
                x.FieldName = "UOM_Name";
                x.Caption = "Item Unit";
                x.VisibleIndex = 9;
                x.ExportWidth = 100;
            });


            settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            settings.SettingsExport.LeftMargin = 20;
            settings.SettingsExport.RightMargin = 20;
            settings.SettingsExport.TopMargin = 20;
            settings.SettingsExport.BottomMargin = 20;

            return settings;
        }
    }
    
}