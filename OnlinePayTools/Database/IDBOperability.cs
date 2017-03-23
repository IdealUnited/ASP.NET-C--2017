/*
*
* 文件名称：IDBOperability.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：记录类接口
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Database
{
    /// <summary>
    /// 记录类接口
    /// </summary>
    public interface IDBOperability
    {
        void SetRecordData(IDBRecord dbRecord, Object oFlag);
        void GetRecordData(IDBRecord dbRecord, Object oFlag);
        Object PrimaryKey { get; set; }
    }
}
