/*
*
* 文件名称：DBHelperExt.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库的一些扩展方法（需要NET3.0），部分方法需要NET4.0
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
using System.Linq;

namespace Database
{
    /// <summary>
    /// 数据库数据转换类
    /// </summary>
    public static partial class DBHelper
    {
        /// <summary>
        /// IDataReader没有多个记录集的话可以用DataSet.Load
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static DataTable ToTable(this IDataReader reader)
        {
            return Reader2Table(reader);
        }


        public static void ToCSV(this DataTable table, String strFile)
        {
            String separator = ",";
            _Export(table, strFile, separator);
        }

        public static void ToExcel(this DataTable table, String strFile)
        {
            String separator = "\t";
            _Export(table, strFile, separator);
        }


        #region Table2InsertSQL

        
        public static List<String> ToInsertSQL(this DataTable table, String strTableName, int nIndex, String QuotePrefix, String QuoteSuffix)
        {
            return Table2InsertSQL(table, strTableName, nIndex, QuotePrefix, QuoteSuffix);
        }
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
        public static String ToPrimaryKeys<T>(this IEnumerable<T> source, String QuotePrefix, String QuoteSuffix)
            where T : IDBOperability
        {
            var result = from x in source
                         select String.Format("{0}{1}{2}", QuotePrefix, x.PrimaryKey, QuoteSuffix);
            return String.Join(",", result);
        }


        /// <summary>
        /// 将内存中图片数据转为图片
        /// </summary>
        /// <param name="bytes">内存中的图片数据</param>
        /// <returns>相应的图片对象</returns>
        public static Bitmap ToImage(this byte[] bytes)
        {
            return Bytes2Image(bytes);
        }

        /// <summary>
        /// 将内存中图片数据转为图片
        /// </summary>
        /// <param name="bytes">内存中的图片数据</param>
        /// <returns>相应的图片对象</returns>
        public static byte[] ToBytes(this Image img)
        {
            return Image2Bytes(img);
        }

        /// <summary>
        /// 根据主键计算需要插入的记录（新的有而旧的没有）
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="newSource">新的记录</param>
        /// <param name="oldSource">旧的记录</param>
        /// <returns>应插入的记录</returns>
        public static IEnumerable<T> Insert<T>(this IEnumerable<T> newSource, IEnumerable<T> oldSource)
            where T : IDBOperability
        {
            DBCompare<T> compare = new DBCompare<T>();
            var result = from x in newSource
                         where !oldSource.Contains(x, compare)
                         select x;
            return result;
        }


        /// <summary>
        /// 根据主键计算需要更新的记录（新的和旧的都有）
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="newSource">新的记录</param>
        /// <param name="oldSource">旧的记录</param>
        /// <returns>应更新的记录</returns>
        public static IEnumerable<T> Update<T>(this IEnumerable<T> newSource, IEnumerable<T> oldSource)
            where T : IDBOperability
        {
            DBCompare<T> compare = new DBCompare<T>();
            var result = from x in newSource
                         where oldSource.Contains(x, compare)
                         select x;
            return result;
        }

        /// <summary>
        /// 根据主键计算需要删除的记录（新的没有而旧的有）
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="newSource">新的记录</param>
        /// <param name="oldSource">旧的记录</param>
        /// <returns>应删除的记录</returns>
        public static IEnumerable<T> Delete<T>(this IEnumerable<T> newSource, IEnumerable<T> oldSource)
            where T : IDBOperability
        {
            DBCompare<T> compare = new DBCompare<T>();
            var result = from x in oldSource
                         where !newSource.Contains(x, compare)
                         select x;
            return result;
        }

        public static void Reserve<T>(this List<T> tValue, int nSize)
        {
            if (tValue.Capacity < nSize)
            {
                tValue.Capacity = nSize;
            }
        }

        #endregion

    }

}
