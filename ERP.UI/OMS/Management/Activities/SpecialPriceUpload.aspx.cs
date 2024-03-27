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

namespace ERP.OMS.Management.Activities
{
    public partial class SpecialPriceUpload : System.Web.UI.Page
    {
        BusinessLogicLayer.GenericMethod oGenericMethod;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
        }

        [WebMethod(EnableSession = true)]
        public static object GetCustomer(string SearchKey)
        {
            List<CustomerModel> listCust = new List<CustomerModel>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                SearchKey = SearchKey.Replace("'", "''");
                // BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                DataTable cust = oDBEngine.GetDataTable(" select top 10 cnt_internalid ,uniquename ,Name ,Billing,Type   from v_SaleRateLock_customerDetails where uniquename like '%" + SearchKey + "%' or Name like '%" + SearchKey + "%'   order by Name");

                listCust = (from DataRow dr in cust.Rows
                            select new CustomerModel()
                            {
                                id = dr["cnt_internalid"].ToString(),
                                Na = dr["Name"].ToString(),
                                UId = Convert.ToString(dr["uniquename"]),
                                add = Convert.ToString(dr["Billing"]),
                                TYPE = Convert.ToString(dr["Type"])
                            }).ToList();
            }

            return listCust;
        }

        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public static object GetProduct(string SearchKey)
        {
            List<PosProductModel> listCust = new List<PosProductModel>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                SearchKey = SearchKey.Replace("'", "''");

                // BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                DataTable cust = oDBEngine.GetDataTable("select top 10  SPRODUCTSID,Products_Name ,Products_Description ,sProduct_MinSalePrice  from v_Product_SaleRateLock where Products_Name like '%" + SearchKey + "%'  or Products_Description  like '%" + SearchKey + "%' order by Products_Name,Products_Description");

                listCust = (from DataRow dr in cust.Rows
                            select new PosProductModel()
                            {
                                id = dr["SPRODUCTSID"].ToString(),
                                Na = dr["Products_Name"].ToString(),
                                Des = Convert.ToString(dr["Products_Description"]),
                                MinSalePrice = Convert.ToString(dr["sProduct_MinSalePrice"])
                            }).ToList();
            }

            return listCust;
        }

        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public static string addSaleRateLock(string SaleRateLockID, List<String> CustID, List<String> ProductID, string DiscSalesPrice, string MinSalePrice, string discount, string fromdt, string todate, string action, string FixRate, String SCHEME)
        {
            try
            {
                DateTime fdt = Convert.ToDateTime(fromdt);
                DateTime tdt = Convert.ToDateTime(todate);
                if (fdt > tdt)
                {
                    return "-12";
                }
                else if (fdt == tdt)
                {
                    return "-13";
                }
                else
                {
                    DataTable dtvalue = new DataTable();
                    //dtvalue = null;
                    //if (dtvalue == null)
                    //{
                    dtvalue.Columns.Add("Entity", typeof(String));
                    dtvalue.Columns.Add("Product", typeof(String));
                    //}

                    foreach (String item in CustID)
                    {
                        foreach (String prod in ProductID)
                        {
                            dtvalue.Rows.Add(new Object[] { item, prod.Split('|')[0].ToString() });
                        }
                    }

                    //List<String> ProductList = ProductID.Split(',');
                    //BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine(ConfigurationManager.AppSettings["DBConnectionDefault"]);
                    BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();


                    ProcedureExecute proc = new ProcedureExecute("PRC_SaleRateLock");
                    proc.AddVarcharPara("@SaleRateLockID", 50, SaleRateLockID);
                    //proc.AddVarcharPara("@CustomerID", 50, CustID);
                    // proc.AddIntegerPara("@ProductID", Convert.ToInt32(ProductID));
                    proc.AddDecimalPara("@DiscSalesPrice", 2, 18, Convert.ToDecimal(DiscSalesPrice));
                    proc.AddIntegerPara("@ApprovedBy", Convert.ToInt32(HttpContext.Current.Session["userid"]));
                    proc.AddVarcharPara("@ValidFrom", 50, fdt.ToString("yyyy-MM-dd HH:mm:ss"));
                    proc.AddVarcharPara("@ValidUpto", 50, tdt.ToString("yyyy-MM-dd HH:mm:ss"));
                    proc.AddDecimalPara("@MinSalePrice", 2, 18, Convert.ToDecimal(MinSalePrice));
                    proc.AddDecimalPara("@Disc", 2, 18, Convert.ToDecimal(discount));
                    proc.AddVarcharPara("@Action", 4000, action);
                    proc.AddDecimalPara("@FixedRate", 2, 18, Convert.ToDecimal(FixRate));
                    proc.AddVarcharPara("@SCHEME", 300, SCHEME);
                    proc.AddPara("@UDT_RATELIST", dtvalue);
                    DataTable dtSaleRateLock = proc.GetTable();
                    if (dtSaleRateLock.Rows.Count > 0)
                    {
                        if (dtSaleRateLock.Rows[0]["Insertmsg"].ToString() == "-11")
                        {
                            return "-11";
                        }
                        else
                        {
                            return "1";
                        }
                    }
                    else
                    {
                        return "0";
                    }
                }
            }
            catch (Exception ex)
            {
                return "Error occured";
            }
        }

        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public static string DeleteSpecialPrice(string SPECIALPRICEID)
        {
            try
            {
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                ProcedureExecute proc = new ProcedureExecute("prc_SpecialPriceImportFromExcel");
                proc.AddVarcharPara("@SPECIALPRICEID", 50, SPECIALPRICEID);
                proc.AddVarcharPara("@Action", 4000, "DeleteSpecialPrice");
                proc.AddIntegerPara("@USERID", Convert.ToInt32(HttpContext.Current.Session["userid"]));
                DataTable dtSaleRateLock = proc.GetTable();
                if (dtSaleRateLock.Rows.Count > 0)
                {
                    if (dtSaleRateLock.Rows[0]["Insertmsg"].ToString() == "1")
                    {
                        return "1";
                    }
                    else if (dtSaleRateLock.Rows[0]["Insertmsg"].ToString() == "-998")
                    {
                        return "-998";
                    }
                    else
                    {
                        return "0";
                    }
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                return "Error occured";
            }
        }


        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public static string UpdateSpecialPrice(string SPECIALPRICEID, string SPECIALPRICE)
        {
            try
            {
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                ProcedureExecute proc = new ProcedureExecute("prc_SpecialPriceImportFromExcel");
                proc.AddVarcharPara("@SPECIALPRICEID", 50, SPECIALPRICEID);
                proc.AddDecimalPara("@SPECIALPRICE", 2, 18, Convert.ToDecimal(SPECIALPRICE));
                proc.AddVarcharPara("@Action", 4000, "UpdateSpecialPrice");
                proc.AddIntegerPara("@USERID", Convert.ToInt32(HttpContext.Current.Session["userid"]));
                DataTable dtSaleRateLock = proc.GetTable();
                if (dtSaleRateLock.Rows.Count > 0)
                {
                    if (dtSaleRateLock.Rows[0]["Insertmsg"].ToString() == "-11")
                    {
                        return "-11";
                    }
                    else
                    {
                        return "1";
                    }
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception ex)
            {
                return "Error occured";
            }
        }

        protected void EntityServerModeDataSource_Selecting(object sender, DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs e)
        {
            e.KeyExpression = "SPECIALPRICEID";
            //string connectionString = ConfigurationManager.ConnectionStrings["crmConnectionString"].ConnectionString;
            string connectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
            int User_id = Convert.ToInt32(Session["userid"]);
            ERPDataClassesDataContext dc = new ERPDataClassesDataContext(connectionString);
            var q = from d in dc.PRODUCTSPECIALPRICELISTs
                    where d.USERID == User_id
                    orderby d.SEQ descending
                    select d;
            e.QueryableSource = q;
        }
        protected void CallbackPanel_Callback(object sender, DevExpress.Web.CallbackEventArgsBase e)
        {
            string returnPara = Convert.ToString(e.Parameter);

            string strProduct_hiddenID = (Convert.ToString(txtProduct_hidden.Value) == "") ? "0" : Convert.ToString(txtProduct_hidden.Value);
            Task PopulateStockTrialDataTask = new Task(() => GetSpecialPriceUploaddata(strProduct_hiddenID));
            PopulateStockTrialDataTask.RunSynchronously();
        }
        public void GetSpecialPriceUploaddata(string Products)
        {
            try
            {
                DataSet ds = new DataSet();
                SqlConnection con = new SqlConnection(Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]));
                SqlCommand cmd = new SqlCommand("PRC_PRODUCTSPECIALPRICE_LIST", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@COMPANYID", Convert.ToString(Session["LastCompany"]));
                cmd.Parameters.AddWithValue("@FINYEAR", Convert.ToString(Session["LastFinYear"]));
                cmd.Parameters.AddWithValue("@Products", Products);
                cmd.Parameters.AddWithValue("@USERID", Convert.ToInt32(Session["userid"]));
                cmd.Parameters.AddWithValue("@ACTION", hFilterType.Value);
                cmd.CommandTimeout = 0;
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);

                cmd.Dispose();
                con.Dispose();
            }
            catch (Exception ex)
            {

            }
        }

        protected void GridSaleRate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {

        }

        [WebMethod(EnableSession = true)]
        [System.Web.Script.Services.ScriptMethod(ResponseFormat = System.Web.Script.Services.ResponseFormat.Json)]
        public static List<SPECIALPRICE> GetSpecialPrice(string SPECIALPRICEID)
        {
            List<SPECIALPRICE> listSaleRateLock = new List<SPECIALPRICE>();
            if (HttpContext.Current.Session["userid"] != null)
            {
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();
                ProcedureExecute proc = new ProcedureExecute("prc_SpecialPriceImportFromExcel");
                proc.AddVarcharPara("@Action", 50, "GetSpecialPrice");
                proc.AddVarcharPara("@SPECIALPRICEID", 10, SPECIALPRICEID);
                DataTable dtSPECIALPRICE = proc.GetTable();

                listSaleRateLock = (from DataRow dr in dtSPECIALPRICE.Rows
                                    select new SPECIALPRICE()
                                    {

                                        ProductID = Convert.ToString(dr["PRODUCT_ID"]),
                                        Products_Name = Convert.ToString(dr["sProducts_Name"]),
                                        SPECIAL_PRICE = dr["SPECIAL_PRICE"].ToString(),
                                        PRODUCT_CODE = dr["PRODUCT_CODE"].ToString(),
                                        branch_description = dr["branch_description"].ToString(),
                                    }).ToList();
            }

            return listSaleRateLock;

        }

        

      
        public class SPECIALPRICE
        {
            public int SPECIALPRICEID { get; set; }
            public string PRODUCT_CODE { get; set; }
            public string SPECIAL_PRICE { get; set; }
            public string ProductID { get; set; }
            public string Products_Name { get; set; }

            public string branch_description { get; set; }

        }
        public class SaleRateLock
        {
            public int SaleRateLockID { get; set; }
            public string CustomerID { get; set; }
            public string CustomerName { get; set; }
            public string ProductID { get; set; }
            public string Products_Name { get; set; }
            public string DiscSalesPrice { get; set; }
            public string MinSalePrice { get; set; }
            public string Disc { get; set; }
            public string ValidFrom { get; set; }
            public string ValidUpto { get; set; }
            public string IsInUse { get; set; }
            public string FixRate { get; set; }
            public string Scheme { get; set; }
        }

        public class PosProductModel
        {
            public string id { get; set; }
            public string Na { get; set; }
            public string Des { get; set; }
            public string MinSalePrice { get; set; }
        }

        public class CustomerModel
        {
            public string id { get; set; }
            public string Na { get; set; }
            public string UId { get; set; }
            public string add { get; set; }
            public string TYPE { get; set; }
        }

        protected void btndownload_Click(object sender, EventArgs e)
        {
            string strFileName = "SpecialPriceUpload.xlsx";
            string strPath = (Convert.ToString(System.AppDomain.CurrentDomain.BaseDirectory) + "/CommonFolder/" + strFileName);

            Response.ContentType = "application/xlsx";
            Response.AppendHeader("Content-Disposition", "attachment; filename=SpecialPriceUpload.xlsx");
            Response.TransmitFile(strPath);
            Response.End();

        }

        [WebMethod]
        [HttpGet]
        public static void download(String State)
        {
            DataTable dt = new DataTable();
            try
            {
                BusinessLogicLayer.DBEngine oDBEngine = new BusinessLogicLayer.DBEngine();

                ProcedureExecute proc = new ProcedureExecute("PRC_SaleRateLock");
                proc.AddVarcharPara("@Action", 50, "GetImportData");
                proc.AddVarcharPara("@StateID", 10, State);
                dt = proc.GetTable();

                //HttpContext.Current.Session["DownloadData"] = dt;

                GridViewExtension.ExportToXlsx(GetShopListTemplateByAreaExcel(dt, ""), dt, true, getXlsExportOptions());

            }
            catch { }
            // return "true";
        }

        private static XlsxExportOptionsEx getXlsExportOptions()
        {
            DevExpress.XtraPrinting.XlsxExportOptionsEx obj = new DevExpress.XtraPrinting.XlsxExportOptionsEx(DevExpress.XtraPrinting.TextExportMode.Text);
            obj.ExportMode = DevExpress.XtraPrinting.XlsxExportMode.SingleFile;
            obj.ExportType = DevExpress.Export.ExportType.WYSIWYG;

            return obj;
        }

        private static GridViewSettings GetShopListTemplateByAreaExcel(object datatable, String dates)
        {
            var settings = new GridViewSettings();
            settings.Name = "ProductRate";
            settings.SettingsExport.ExportedRowType = GridViewExportedRowType.All;
            settings.SettingsExport.FileName = "ProductRate";
            settings.Name = "ProductRate";

            DataTable dt = (DataTable)datatable;



            foreach (System.Data.DataColumn datacolumn in dt.Columns)
            {
                if (datacolumn.ColumnName == "STATE" || datacolumn.ColumnName == "Code"
                    || datacolumn.ColumnName == "Description" || datacolumn.ColumnName == "PricetoDistributor" || datacolumn.ColumnName == "PricetoRetailer")
                {
                    settings.Columns.Add(column =>
                    {
                        if (datacolumn.ColumnName == "STATE")
                        {
                            column.Caption = "STATE";
                        }
                        else if (datacolumn.ColumnName == "Code")
                        {
                            column.Caption = "Code";
                        }
                        else if (datacolumn.ColumnName == "Description")
                        {
                            column.Caption = "Description";
                        }
                        else if (datacolumn.ColumnName == "PricetoDistributor")
                        {
                            column.Caption = "Price to Distributor";
                        }
                        else if (datacolumn.ColumnName == "PricetoRetailer")
                        {
                            column.Caption = "Price to Retailer";
                        }


                        column.FieldName = datacolumn.ColumnName;

                        if (datacolumn.DataType.FullName == "System.DateTime")
                        {
                            column.PropertiesEdit.DisplayFormatString = "DD-MM-YYYY";
                        }

                    });
                }

            }


            settings.SettingsExport.PaperKind = System.Drawing.Printing.PaperKind.A4;
            settings.SettingsExport.LeftMargin = 20;
            settings.SettingsExport.RightMargin = 20;
            settings.SettingsExport.TopMargin = 20;
            settings.SettingsExport.BottomMargin = 20;

            return settings;
        }

        protected void ImportExcel(object sender, EventArgs e)
        {
            Boolean HasLog = false;
            if (fileprod.HasFile)
            {
                string FileName = Path.GetFileName(fileprod.PostedFile.FileName);
                string Extension = Path.GetExtension(fileprod.PostedFile.FileName);
                string FolderPath = ConfigurationManager.AppSettings["FolderPath"];
                string FilePath = Server.MapPath("~/Temporary/") + FileName;
                fileprod.SaveAs(FilePath);
                HasLog = Import_To_Grid(FilePath, Extension, "No");

                File.Delete(FilePath);
            }

            if (HasLog == false)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "PageScript", "<script language='javascript'>jAlert('invalid File!'); </script>");

            }
            else
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "PageScript", "<script language='javascript'>jAlert('Import Process Successfully Completed!'); </script>");

            }

        }

        public Boolean Import_To_Grid(string FilePath, string Extension, string isHDR)
        {
            Boolean Success = false;
            Boolean HasLog = false;
            if (fileprod.FileName.Trim() != "")
            {
                string fileName = Path.GetFileName(fileprod.PostedFile.FileName);
                string extention = fileName.Substring(fileName.IndexOf('.'), fileName.Length - fileName.IndexOf('.'));
                extention = extention.TrimStart('.');
                extention = extention.ToUpper();
                int loopcounter = 1;

                if (extention == "XLS" || extention == "XLSX")
                {
                    fileName = fileName.Replace(fileName.Substring(0, fileName.IndexOf('.')), "Productupload");
                    DataTable dt = new DataTable();
                    using (SpreadsheetDocument doc = SpreadsheetDocument.Open(FilePath, false))
                    {
                        Sheet sheet = doc.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
                        Worksheet worksheet = (doc.WorkbookPart.GetPartById(sheet.Id.Value) as WorksheetPart).Worksheet;
                        IEnumerable<Row> rows = worksheet.GetFirstChild<SheetData>().Descendants<Row>().DefaultIfEmpty();

                        foreach (Row row in rows)
                        {
                            if (row.RowIndex.Value == 1)
                            {
                                foreach (Cell cell in row.Descendants<Cell>())
                                {
                                    if (cell.CellValue != null)
                                    {
                                        dt.Columns.Add(GetValue(doc, cell));
                                    }
                                }
                            }
                            else
                            {
                                DataRow tempRow = dt.NewRow();
                                int columnIndex = 0;
                                foreach (Cell cell in row.Descendants<Cell>())
                                {
                                    // Gets the column index of the cell with data

                                    int cellColumnIndex = (int)GetColumnIndexFromName(GetColumnName(cell.CellReference));
                                    cellColumnIndex--; //zero based index
                                    if (columnIndex < cellColumnIndex)
                                    {
                                        do
                                        {
                                            tempRow[columnIndex] = ""; //Insert blank data here;
                                            columnIndex++;
                                        }
                                        while (columnIndex < cellColumnIndex);
                                    }
                                    try
                                    {
                                        tempRow[columnIndex] = GetValue(doc, cell);

                                    }
                                    catch
                                    {
                                        tempRow[columnIndex] = "";
                                    }

                                    columnIndex++;
                                }
                                dt.Rows.Add(tempRow);
                            }
                        }
                    }

                    if (dt != null && dt.Rows.Count > 0)
                    {

                        foreach (DataRow row in dt.Rows)
                        {
                            loopcounter++;
                            try
                            {
                                string BRANCH = Convert.ToString(row["BRANCH"]);
                                string PRODUCTCODE = Convert.ToString(row["PRODUCTCODE"]);
                                string PRODUCTNAME = Convert.ToString(row["PRODUCTNAME"]);
                                string SPECIALPRICE = Convert.ToString(row["SPECIALPRICE"]);

                                DataSet dt2 = InsertSpecialPriceDataFromExcel(BRANCH, PRODUCTCODE, PRODUCTNAME, SPECIALPRICE
                                       );


                                if (dt2 != null && dt2.Tables[0].Rows.Count > 0)
                                {
                                    foreach (DataRow row2 in dt2.Tables[0].Rows)
                                    {
                                        Success = Convert.ToBoolean(row2["Success"]);
                                        HasLog = Convert.ToBoolean(row2["HasLog"]);
                                    }
                                }

                                if (!HasLog)
                                {
                                    string description = Convert.ToString(dt2.Tables[0].Rows[0]["MSG"]);
                                    int loginsert = InsertSpecialPriceImportLOg(PRODUCTCODE, SPECIALPRICE, BRANCH, description, "Failed");
                                }

                                else
                                {
                                    string description = Convert.ToString(dt2.Tables[0].Rows[0]["MSG"]);
                                    int loginsert = InsertSpecialPriceImportLOg(PRODUCTCODE, SPECIALPRICE, BRANCH, description, "Success");
                                }



                            }
                            catch (Exception ex)
                            {
                                Success = false;
                                HasLog = false;

                            }

                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, GetType(), UniqueID, "jAlert('invalid File')", true);
                    }


                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), UniqueID, "jAlert('invalid File')", true);
                }


            }
            return HasLog;
        }
        public int InsertSpecialPriceImportLOg(string PRODUCTCODE, string SPECIALPRICE, string BRANCH, string description, string status)
        {

            int i;
            ProcedureExecute proc = new ProcedureExecute("prc_SpecialPriceImportFromExcel");
            proc.AddVarcharPara("@action", 150, "InsertSpecialPriceImportLOg");
            proc.AddVarcharPara("@BRANCH", 200, BRANCH);
            proc.AddVarcharPara("@PRODUCTCODE", 200, PRODUCTCODE);
            proc.AddPara("@SPECIALPRICE", SPECIALPRICE);
            proc.AddVarcharPara("@decription", 150, description);
            proc.AddVarcharPara("@status", 150, status);
            proc.AddIntegerPara("@UserId", Convert.ToInt32(Session["userid"]));
            i = proc.RunActionQuery();

            return i;
        }
        public DataSet InsertSpecialPriceDataFromExcel(string BRANCH, string PRODUCTCODE, string PRODUCTNAME, string SPECIALPRICE)
        {
            DataSet ds = new DataSet();
            ProcedureExecute proc = new ProcedureExecute("prc_SpecialPriceImportFromExcel");
            proc.AddVarcharPara("@Action", 100, "InsertSpecialPriceDataFromExcel");
            proc.AddIntegerPara("@UserId", Convert.ToInt32(Session["userid"]));
            proc.AddVarcharPara("@BRANCH", 200, BRANCH);
            proc.AddVarcharPara("@PRODUCTCODE", 200, PRODUCTCODE);
            proc.AddVarcharPara("@PRODUCTNAME", 200, PRODUCTNAME);
            proc.AddPara("@SPECIALPRICE", SPECIALPRICE);
            ds = proc.GetDataSet();
            return ds;
        }



        public static int? GetColumnIndexFromName(string columnName)
        {
            //return columnIndex;
            string name = columnName;
            int number = 0;
            int pow = 1;
            for (int i = name.Length - 1; i >= 0; i--)
            {
                number += (name[i] - 'A' + 1) * pow;
                pow *= 26;
            }
            return number;
        }

        public static string GetColumnName(string cellReference)
        {
            // Create a regular expression to match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);
            return match.Value;
        }

        private string GetValue(SpreadsheetDocument doc, Cell cell)
        {
            string value = cell.CellValue.InnerText;
            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return doc.WorkbookPart.SharedStringTablePart.SharedStringTable.ChildElements.GetItem(int.Parse(value)).InnerText;
            }
            return value;
        }

        protected void gridProductRate_CustomCallback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {

        }

        protected void ProductRateServerModeDataSource_Selecting(object sender, DevExpress.Data.Linq.LinqServerModeDataSourceSelectEventArgs e)
        {
            e.KeyExpression = "ID";
            string connectionString = Convert.ToString(System.Web.HttpContext.Current.Session["ErpConnection"]);
            ERPDataClassesDataContext dc = new ERPDataClassesDataContext(connectionString);
            var q = from d in dc.v_ProductRates
                    orderby d.ID descending
                    select d;
            e.QueryableSource = q;
        }

        protected void grid_RateLog_DataBinding(object sender, EventArgs e)
        {
            if (Session["Datlog"] != null)
            {
                grid_RateLog.DataSource = (DataTable)Session["Datlog"];
            }
        }

        protected void ProductRateLog_Callback(object sender, ASPxGridViewCustomCallbackEventArgs e)
        {
            if (Session["Datlog"] != null)
            {
                DataTable ComponentTable = (DataTable)Session["Datlog"];
                grid_RateLog.Selection.CancelSelection();
                grid_RateLog.DataSource = ComponentTable;
                grid_RateLog.DataBind();
            }
            else
            {
                grid_RateLog.Selection.CancelSelection();
                grid_RateLog.DataSource = null;
                grid_RateLog.DataBind();
            }
        }

        protected void cmbExport_SelectedIndexChanged(object sender, EventArgs e)
        {
            Int32 Filter = int.Parse(Convert.ToString(drdExport.SelectedItem.Value));
            string filename = "Product Rate Import Log";
            exporter.FileName = filename;
            exporter.PageHeader.Left = "Product Rate Import Log";
            exporter.PageFooter.Center = "[Page # of Pages #]";
            exporter.PageFooter.Right = "[Date Printed]";
            switch (Filter)
            {
                case 1:
                    using (MemoryStream stream = new MemoryStream())
                    {
                        exporter.WritePdf(stream);
                        WriteToResponse("Area", true, "pdf", stream);
                    }
                    break;
                case 2:
                    exporter.WriteXlsToResponse();
                    break;
                case 3:
                    exporter.WriteRtfToResponse();
                    break;
                case 4:
                    exporter.WriteCsvToResponse();
                    break;
            }
        }

        protected void WriteToResponse(string fileName, bool saveAsFile, string fileFormat, MemoryStream stream)
        {
            if (Page == null || Page.Response == null) return;
            string disposition = saveAsFile ? "attachment" : "inline";
            Page.Response.Clear();
            Page.Response.Buffer = false;
            Page.Response.AppendHeader("Content-Type", string.Format("application/{0}", fileFormat));
            Page.Response.AppendHeader("Content-Transfer-Encoding", "binary");
            Page.Response.AppendHeader("Content-Disposition", string.Format("{0}; filename={1}.{2}", disposition, HttpUtility.UrlEncode(fileName).Replace("+", "%20"), fileFormat));
            if (stream.Length > 0)
                Page.Response.BinaryWrite(stream.ToArray());
        }
    }
}