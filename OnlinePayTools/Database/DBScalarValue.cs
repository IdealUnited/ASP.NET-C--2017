/*
*
* 文件名称：DBScalarValue.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库单值获取类，用于获取一个值或者一列值
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Database
{
    /// <summary>
    /// 数据库单值获取类，用于获取一个值或者一列值
    /// </summary>
    public class DBScalarValue<T> : IDBOperability
    {
        T m_Value;

        public T Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        #region IDBOperability 成员

        public void SetRecordData(IDBRecord dbRecord, object oFlag)
        {
            IDBRecord x = dbRecord;
            if (oFlag != null)
            {
                if (oFlag is String)
                {
                    x[oFlag as String] = Value;
                }
                else if (oFlag is Int32)
                {
                    x[(Int32)oFlag] = Value;
                }
                else
                {
                    x[0] = Value;
                }         
            }
            else
            {
                x[0] = Value;
            }
        }

        public void GetRecordData(IDBRecord dbRecord, object oFlag)
        {
            IDBRecord x = dbRecord;
            Object oValue = null;
            if (oFlag != null)
            {
                if (oFlag is String)
                {
                    oValue = x[oFlag as String];
                }
                else if (oFlag is Int32)
                {
                    oValue = x[(Int32)oFlag];
                }
                else
                {
                    oValue = x[0];
                }
            }
            else
            {
                oValue = x[0];
            }

            DBValue dbValue = DBValue.Convert(oValue);
            Value = dbValue.GetValue<T>();           
        }

        public object PrimaryKey
        {
            get
            {
                return Value;
            }
            set
            {
                Value = (T)value;
            }
        }

        #endregion
    }
}
