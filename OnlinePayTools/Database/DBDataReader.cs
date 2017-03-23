/*
*
* 文件名称：DBDataReader.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：DataReader的封装类，代理对应的Item[Int32]和Item[String]接口
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Database
{
    /// <summary>
    /// DataReader的封装类，代理对应的Item[Int32]和Item[String]接口
    /// </summary>
    public class DBDataReader : IDBRecord
    {
        private IDataReader m_DataReader;

        private IDataReader DataReader
        {
            get { return m_DataReader; }
            set { m_DataReader = value; }
        }

        public DBDataReader(System.Data.IDataReader reader)
        {
            DataReader = reader;
        }

        /// <summary>
        /// 当前接口封装的数据
        /// </summary>
        public object Value
        {
            get { return DataReader; }
        }

        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据
        /// </summary>
        /// <param name="columnIndex">列的从零开始的索引</param>
        /// <returns>包含的数据</returns>
        public object this[string columnName]
        {
            get
            {
                return DataReader[columnName];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <returns>包含的数据</returns>
        public object this[int columnIndex]
        {
            get
            {
                return DataReader[columnIndex]; ;
            }
            set
            {
                throw new NotSupportedException();
            }
        }
    }
}
