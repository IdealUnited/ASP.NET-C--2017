using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace ComLibrary
{
   public class OfficeUtils
    {
        LogUtil log = new LogUtil();
        static bool _isVerbose = false;
        static bool _isAllian = false;

        // 获得字段的实际最大长度
        public int GetMaxLength(DataTable dt, string captionName)
        {
            DataColumn maxLengthColumn = new DataColumn();
            maxLengthColumn.ColumnName = "MaxLength";
            maxLengthColumn.Expression = String.Format("len(convert({0},'System.String'))", captionName);
            dt.Columns.Add(maxLengthColumn);
            object maxLength = dt.Compute("max(MaxLength)", "true");
            if (maxLength == DBNull.Value)
            {
                return 0;
            }
            dt.Columns.Remove(maxLengthColumn);

            return Convert.ToInt32(maxLength);
        }

        public static DataTable readExcelToDataTable(string inputFile)
        {
            var conn = new OleDbConnection();
            try
            {
                if (Path.GetExtension(inputFile) != ".xls" && Path.GetExtension(inputFile) != ".xlsx")
                {
                    throw new Exception("文件格式不正确！请选择正确的Excel文件。");
                }
                conn.ConnectionString = String.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;" +
                    @"Data Source={0}" +
                    ";Extended Properties=\"Excel 12.0 Xml;HDR=No;IMEX=1\"", inputFile);
                conn.Open();
                DataTable sheetTb = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                //默认取第一个sheet
                if (sheetTb == null || sheetTb.Rows.Count == 0) {
                    throw new Exception("数据为空！请检查文件。");
                }
                string tableName = sheetTb.Rows[0]["TABLE_NAME"].ToString();

                string sql = String.Format("select * from [{0}]", tableName);
                OleDbDataAdapter da = new OleDbDataAdapter(sql, conn);

                var ds = new DataSet();
                da.Fill(ds);
                var tb1 = ds.Tables[0];

                if (tb1.Rows.Count == 0)
                {
                    throw new Exception("数据为空！请检查文件。");
                }
                if (tb1.Rows.Count == 1 && tb1.Columns.Count == 1)
                {
                    if (tb1.Rows[0][0] == DBNull.Value)
                    {
                        throw new Exception("数据为空！请检查文件。");
                    }
                }
                if (tb1.Rows[0][0] !="订单号")
                {
                    throw new Exception("数据模板不对，请选择正确的数据模板。");
                }
                conn.Close();
                return tb1;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally {
                if (conn != null&&conn.State==ConnectionState.Open) {
                    conn.Close();
                }
            }     
        }
    }
}
