/*
*
* 文件名称：DBDelegate.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：委托文件
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;



namespace Database
{

    /// <summary>
    /// 设置CommandBuilder的参数
    /// 1.比如OLEDB驱动插入Access数据的时报错“insert into语法错误”，可能是因为字段名称占用了Access关键字，需要设置QuotePrefix为"[",QuoteSuffix设置为"]"
    /// 2.要做批量更新的时候设置cmdBuilder.DataAdapter.UpdateBatchSize为批更新数量，不过似乎好多驱动都不支持
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="cmdBuilder">需要设置的CommandBuilder</param>
    public delegate void DBSetCommandBuilder(Object sender, DbCommandBuilder cmdBuilder);

    /// <summary>
    /// 参数推导之前
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="cmd">需要推导的Command</param>
    /// <param name="query">推导的查询</param>
    /// <param name="parameters">参数列表</param>
    public delegate void DBBeforeParameterDerive(Object sender, DbCommand cmd, string query, params Object[] parameters);

    /// <summary>
    /// 参数推导之后
    /// </summary>
    /// <param name="sender">事件源</param>
    /// <param name="cmd">需要推导的Command</param>
    /// <param name="query">推导的查询</param>
    /// <param name="parameters">参数列表</param>
    public delegate void DBAfterParameterDerive(Object sender, DbCommand cmd, string query, params Object[] parameters);

}
