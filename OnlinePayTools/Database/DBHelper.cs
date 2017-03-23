/*
*
* 文件名称：DBHelper.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：一些数据库操作常用辅助函数
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Data;

namespace Database
{
    /// <summary>
    /// 一些数据库操作常用辅助函数
    /// </summary>
    public static partial class DBHelper
    {
        /// <summary>
        /// IDataReader没有多个记录集的话可以用DataSet.Load
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable Reader2Table(IDataReader reader)
        {
            DataTable table = new DataTable();
            int nCount = reader.FieldCount;
            for (int i = 0; i < nCount; i++)
            {
                table.Columns.Add(new DataColumn(reader.GetName(i), reader.GetFieldType(i)));
            }

            while (reader.Read())
            {
                DataRow row = table.NewRow();
                for (int i = 0; i < nCount; i++)
                {
                    row[i] = reader[i];
                }
                table.Rows.Add(row);
            }
            return table;
        }

        public static String DbValueToString(Object oValue, DataColumn col)
        {
            String strValue = "NULL";
            if (!Convert.IsDBNull(oValue))
            {
                switch (col.DataType.Name)
                {
                    case "Byte[]":
                        strValue = BitConverter.ToString(oValue as Byte[], 0).Replace("-", "");
                        break;
                    default:
                        strValue = String.Format("{0}", oValue);
                        break;
                }
            }
            return strValue;
        }

        public static void ExportCSV(DataTable table, String strFile)
        {
            String separator = ",";
            _Export(table, strFile, separator);
        }

        public static void ExportExcel(DataTable table, String strFile)
        {
            String separator = "\t";
            _Export(table, strFile, separator);
        }

        private static void _Export(DataTable table, String strFile, String separator)
        {

            using (TextWriter tw = new StreamWriter(strFile, false, Encoding.UTF8))
            {
                DataColumn[] cols = new DataColumn[table.Columns.Count];
                table.Columns.CopyTo(cols, 0);
                tw.WriteLine(String.Join(separator, Array.ConvertAll<DataColumn, String>(cols, delegate (DataColumn x){ return String.Format("\"{0}\"", x.ColumnName); })));
                StringBuilder strTemp = null;
                int nCount = table.Columns.Count;
                foreach (DataRow row in table.Rows)
                {
                    strTemp = new StringBuilder();
                    for (int i = 0; i < nCount; i++)
                    {
                        if (strTemp.Length != 0)
                        {
                            strTemp.Append(separator);
                        }
                        Object curValue = row[i];
                        DataColumn curCol = cols[i];

                        strTemp.AppendFormat("\"{0}\"", DbValueToString(curValue, curCol));
                    }
                    tw.WriteLine(strTemp.ToString());
                }
            }
        }

        #region Table2InsertSQL

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strValue"></param>
        /// <param name="oValue"></param>
        /// <param name="col"></param>
        /// <param name="nIndex"></param>
        /// <returns>是否要添加到SQL语句中</returns>
        private static bool DbValueToSQLString(out String strValue, Object oValue, DataColumn col, int nIndex)
        {
            bool bRet = false;
            strValue = String.Empty;
            if (Convert.IsDBNull(oValue))
            {
                switch (nIndex)
                {
                    case 0:
                        strValue = "NULL";
                        bRet = true;
                        break;
                    case 1:
                        strValue = "NULL";
                        bRet = false;
                        break;
                    case 2:
                        strValue = "DEFAULT";
                        bRet = true;
                        break;

                    default:
                        bRet = false;
                        break;
                }

            }
            else
            {
                strValue = DbValueToSQLString(oValue, col);
                bRet = true;
            }
            return bRet;
        }

        private static String DbValueToSQLString(Object oValue, DataColumn col)
        {
            String strValue;
            switch (col.DataType.Name)
            {
                case "String":
                case "DateTime":
                case "TimeSpan":
                    strValue = String.Format("'{0}'", oValue);
                    break;
                case "Byte[]":
                    String strTmp = BitConverter.ToString(oValue as Byte[], 0).Replace("-", "");
                    strValue = String.Format("0x{0}", strTmp);
                    break;
                default:
                    strValue = String.Format("{0}", oValue);
                    break;
            }
            return strValue;
        }

        public static List<String> Table2InsertSQL(DataTable table, String strTableName, int nIndex, String QuotePrefix, String QuoteSuffix)
        {
            List<String> lValue = new List<String>();
            List<String> strFieldList = new List<String>();
            List<String> strValueList = new List<String>();
            DataColumnCollection cols = table.Columns;
            int nCount = table.Columns.Count;
            String strTmp;

            if (lValue.Capacity < table.Rows.Count)
            {
                lValue.Capacity = table.Rows.Count;
            }
            foreach (DataRow row in table.Rows)
            {
                strFieldList.Clear();
                strValueList.Clear();
                for (int i = 0; i < nCount; i++)
                {
                    Object curValue = row[i];
                    DataColumn curCol = cols[i];
                    if (DbValueToSQLString(out strTmp, curValue, curCol, nIndex))
                    {
                        strFieldList.Add(String.Format("{0}{1}{2}",
                            QuotePrefix, curCol.ColumnName, QuoteSuffix));
                        strValueList.Add(strTmp);
                    }
                }
                if (strFieldList.Count != 0)
                {
                    strTmp = String.Format("INSERT INTO {0}{1}{2} ({3}) VALUES ({4});",
                        QuotePrefix, strTableName, QuoteSuffix,
                        String.Join(",", strFieldList.ToArray()),
                        String.Join(",", strValueList.ToArray()));
                    lValue.Add(strTmp);
                }
            }

            return lValue;
        }
        #endregion

        #region DataTable Update

        #endregion

        #region 

        /// <summary>
        /// 拼SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="QuotePrefix"></param>
        /// <param name="QuoteSuffix"></param>
        /// <returns></returns>
        public static String PrimaryKeys<T>(List<T> source, String QuotePrefix, String QuoteSuffix)
            where T : IDBOperability
        {
            List<String> lPrimeKey = source.ConvertAll<String>(delegate(T x) { return String.Format("{0}{1}{2}", QuotePrefix, x.PrimaryKey, QuoteSuffix); });
            return String.Join(",", lPrimeKey.ToArray());
        }

        /// <summary>
        /// 拼接SQL的占位符
        /// </summary>
        /// <param name="nIndex">开始序号</param>
        /// <param name="nCount">个数</param>
        /// <returns></returns>
        public static String PrimaryKeyPlaceHolder(int nIndex, int nCount)
        {
            String[] lString = new String[nCount];
            for (int i=0;i<nCount; i++)
            {
                lString[i] = "{" + nIndex++.ToString() + "}";
            }
            return String.Join(",", lString);
        }

        /// <summary>
        /// 获取主键数组
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">记录</param>
        /// <returns>主键数组</returns>
        public static object[] GetPrimaryKeyArray<T>(List<T> tValue)
            where T : class, IDBOperability
        {
            return tValue.ConvertAll<Object>(delegate(T x) { return x.PrimaryKey; }).ToArray();
        }

        #region 辅助函数

        /// <summary>
        /// 将多个相同参数拼接成数组
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tArray">数组参数列表</param>
        /// <returns>拼接的数组</returns>
        public static T[] ToArray<T>(params T[] tArray)
        {
            return tArray;
        }

        #endregion

        /// <summary>
        /// 将内存中图片数据转为图片
        /// </summary>
        /// <param name="bytes">内存中的图片数据</param>
        /// <returns>相应的图片对象</returns>
        public static Bitmap Bytes2Image(byte[] bytes)
        {
            Bitmap bitmap = null;
            if (bytes != null
                && bytes.Length > 0)
            {
                using (MemoryStream mem = new MemoryStream(bytes))
                {
                    bitmap = new Bitmap(mem);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// 将内存中图片数据转为图片
        /// </summary>
        /// <param name="bytes">内存中的图片数据</param>
        /// <returns>相应的图片对象</returns>
        public static byte[] Image2Bytes(Image img)
        {
            byte[] bytes = null;
            if (img != null)
            {
                MemoryStream mem = new MemoryStream();

                img.Save(mem, ImageFormat.Jpeg);
                bytes = mem.GetBuffer();

            }

            return bytes;
        }

        /// <summary>
        /// 根据主键计算需要插入的记录（新的有而旧的没有）
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="newSource">新的记录</param>
        /// <param name="oldSource">旧的记录</param>
        /// <returns>应插入的记录</returns>
        public static List<T> Insert<T>(List<T> newSource, List<T> oldSource)
            where T : IDBOperability
        {
            return newSource.FindAll(delegate(T x) { return !oldSource.Exists(delegate(T y) { return x.PrimaryKey.Equals(y.PrimaryKey); }); });
        }


        /// <summary>
        /// 根据主键计算需要更新的记录（新的和旧的都有）
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="newSource">新的记录</param>
        /// <param name="oldSource">旧的记录</param>
        /// <returns>应更新的记录</returns>
        public static List<T> Update<T>(List<T> newSource, List<T> oldSource)
            where T : IDBOperability
        {
            return newSource.FindAll(delegate(T x) {return oldSource.Exists(delegate(T y) {return x.PrimaryKey.Equals(y.PrimaryKey);});});
        }


        /// <summary>
        /// 根据主键计算需要删除的记录（新的没有而旧的有）
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="newSource">新的记录</param>
        /// <param name="oldSource">旧的记录</param>
        /// <returns>应删除的记录</returns>
        public static List<T> Delete<T>(List<T> newSource, List<T> oldSource)
            where T : IDBOperability
        {
            return oldSource.FindAll(delegate(T x) { return !newSource.Exists(delegate(T y) { return x.PrimaryKey.Equals(y.PrimaryKey); }); });
        }


        /// <summary>
        /// 解析IDataReader
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="dataReader">需要解析的IDataReader</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        public static void GetRecordData<T>(List<T> tValue, IDataReader dataReader, Object oFlag)
            where T : class, IDBOperability, new()
        {
            tValue.Clear();

            T tTemp = default(T);
            IDBRecord dbRecord = new DBDataReader(dataReader);
            while (dataReader.Read())
            {
                tTemp = new T();
                tTemp.GetRecordData(dbRecord, oFlag);
                tValue.Add(tTemp);
            }
        }


        /// <summary>
        /// 解析DataTable
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="dataReader">需要解析的DataTable</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        public static void GetRecordData<T>(List<T> tValue, DataTable table, Object oFlag)
            where T : class, IDBOperability, new()
        {
            if (tValue.Capacity < table.Rows.Count)
            {
                tValue.Capacity = table.Rows.Count;
            }
            using (DataTableReader reader = table.CreateDataReader())
            {
                GetRecordData<T>(tValue, reader, oFlag);
            }
        }

        /// <summary>
        /// 将记录数据存放到DataTable中
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="table">需要存放的数据表</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        public static void SetRecordData<T>(List<T> tValue, DataTable table, String strPrimeKey, Object oFlag)
            where T : class, IDBOperability, new()
        {
            DataColumn dataColumn = table.Columns[strPrimeKey];
            DataColumn[] prePrimaryKey = table.PrimaryKey;
            if (table.PrimaryKey == null
                || table.PrimaryKey.Length != 1
                || table.PrimaryKey[0] != dataColumn)
            {
                table.PrimaryKey = new DataColumn[] { dataColumn };
            }
            DataRowCollection rows = table.Rows;
            DataRow dataRow = null;
            foreach (IDBOperability item in tValue)
            {
                dataRow = rows.Find(item.PrimaryKey);
                if (dataRow == null)
                {
                    dataRow = table.NewRow();
                }
                IDBRecord dbRecord = new DBDataRow(dataRow);
                item.SetRecordData(dbRecord, oFlag);
                if (dataRow.RowState == DataRowState.Detached)
                {
                    rows.Add(dataRow);
                }
            }

            table.PrimaryKey = prePrimaryKey;
        }
        #endregion

    }

}
