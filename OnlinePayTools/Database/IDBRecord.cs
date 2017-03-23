/*
*
* 文件名称：IDBRecord.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库记录与记录类的交互接口
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Database
{
    /// <summary>
    /// 数据库记录与记录类的交互接口
    /// </summary>
    public interface IDBRecord
    {
        /// <summary>
        /// 当前接口封装的数据
        /// </summary>
        Object Value{ get;}

        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据
        /// </summary>
        /// <param name="columnName">列的名称</param>
        /// <returns>包含的数据</returns>
        Object this[string columnName] { get; set; }

        /// <summary>
        /// 获取或设置存储在由名称指定的列中的数据
        /// </summary>
        /// <param name="columnIndex">列的从零开始的索引</param>
        /// <returns>包含的数据</returns>
        Object this[int columnIndex] { get; set; }
    }
}
