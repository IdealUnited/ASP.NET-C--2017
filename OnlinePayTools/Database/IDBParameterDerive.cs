/*
*
* 文件名称：IDBParameterDerive.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：参数推导接口
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;

namespace Database
{
    /// <summary>
    /// 参数推导
    /// OleDb和ODBC是?非命名参数
    /// SQL SERVER是以@开头的命名参数
    /// ORACLE是以:开头的命名参数
    /// 详见MSDN的XXXCommand.Parameters 
    /// </summary>
    public interface IDBParameterDerive
    {
        void Derive(DbCommand cmd, string query, params Object[] parameters);
    }
}
