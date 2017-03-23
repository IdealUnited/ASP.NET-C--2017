/*
*
* 文件名称：DBValue.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库类型转换类
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Database
{
    /// <summary>
    /// 数据库类型转换类
    /// </summary>
    public class DBValue
    {
        Object m_Value;

        public Object Value
        {
            get { return m_Value; }
            set { m_Value = value; }
        }

        public DBValue(Object obj)
        {
            Value = obj;
        }

        public override string ToString()
        {
            return Value.ToString();
        }

        public T GetValue<T>()
        {
            if (Value is T)
            {
                return (T)Value;
            }
            else if (!System.Convert.IsDBNull(Value))
            {
                return (T)System.Convert.ChangeType(Value, typeof(T));
            }
            else
            {
                return default(T);
            }
        }

        public T? GetNullable<T>()
            where T : struct
        {
            if (Value is T)
            {
                return (T)Value;
            }
            else if (!System.Convert.IsDBNull(Value))
            {
                return (T)System.Convert.ChangeType(Value, typeof(T));
            }
            else
            {
                return null;
            }
        }

        public static DBValue Convert(Object obj)
        {
            return new DBValue(obj);
        }

        public static implicit operator Boolean(DBValue dbValue)
        {
            return dbValue.GetValue<Boolean>();
        }

        public static implicit operator Byte(DBValue dbValue)
        {
            return dbValue.GetValue<Byte>();
        }

        public static implicit operator Char(DBValue dbValue)
        {
            return dbValue.GetValue<Char>();
        }

        public static implicit operator DateTime(DBValue dbValue)
        {
            return dbValue.GetValue<DateTime>();
        }

        public static implicit operator Decimal(DBValue dbValue)
        {
            return dbValue.GetValue<Decimal>();
        }

        public static implicit operator Double(DBValue dbValue)
        {
            return dbValue.GetValue<Double>();
        }

        public static implicit operator Int16(DBValue dbValue)
        {
            return dbValue.GetValue<Int16>();
        }

        public static implicit operator Int32(DBValue dbValue)
        {
            return dbValue.GetValue<Int32>();
        }

        public static implicit operator Int64(DBValue dbValue)
        {
            return dbValue.GetValue<Int64>();
        }

        public static implicit operator SByte(DBValue dbValue)
        {
            return dbValue.GetValue<SByte>();
        }

        public static implicit operator Single(DBValue dbValue)
        {
            return dbValue.GetValue<Single>();
        }

        public static implicit operator String(DBValue dbValue)
        {
            return dbValue.GetValue<String>();
        }

        public static implicit operator TimeSpan(DBValue dbValue)
        {
            return dbValue.GetValue<TimeSpan>();
        }

        public static implicit operator UInt16(DBValue dbValue)
        {
            return dbValue.GetValue<UInt16>();
        }

        public static implicit operator UInt32(DBValue dbValue)
        {
            return dbValue.GetValue<UInt32>();
        }

        public static implicit operator UInt64(DBValue dbValue)
        {
            return dbValue.GetValue<UInt64>();
        }

        public static implicit operator Byte[](DBValue dbValue)
        {
            return dbValue.GetValue<Byte[]>();
        }

        public static implicit operator Byte?(DBValue dbValue)
        {
            return dbValue.GetNullable<Byte>();
        }

        public static implicit operator Char?(DBValue dbValue)
        {
            return dbValue.GetNullable<Char>();
        }

        public static implicit operator DateTime?(DBValue dbValue)
        {
            return dbValue.GetNullable<DateTime>();
        }

        public static implicit operator Decimal?(DBValue dbValue)
        {
            return dbValue.GetNullable<Decimal>();
        }

        public static implicit operator Double?(DBValue dbValue)
        {
            return dbValue.GetNullable<Double>();
        }

        public static implicit operator Int16?(DBValue dbValue)
        {
            return dbValue.GetNullable<Int16>();
        }

        public static implicit operator Int32?(DBValue dbValue)
        {
            return dbValue.GetNullable<Int32>();
        }

        public static implicit operator Int64?(DBValue dbValue)
        {
            return dbValue.GetNullable<Int64>();
        }

        public static implicit operator SByte?(DBValue dbValue)
        {
            return dbValue.GetNullable<SByte>();
        }

        public static implicit operator Single?(DBValue dbValue)
        {
            return dbValue.GetNullable<Single>();
        }

        public static implicit operator TimeSpan?(DBValue dbValue)
        {
            return dbValue.GetNullable<TimeSpan>();
        }

        public static implicit operator UInt16?(DBValue dbValue)
        {
            return dbValue.GetNullable<UInt16>();
        }

        public static implicit operator UInt32?(DBValue dbValue)
        {
            return dbValue.GetNullable<UInt32>();
        }

        public static implicit operator UInt64?(DBValue dbValue)
        {
            return dbValue.GetNullable<UInt64>();
        }

#if false
        public static implicit operator System.Data.Linq.Binary(DBValue dbValue)
        {
            byte[] bytes = dbValue.GetValue<Byte[]>();
            if (bytes == null)
            {
                bytes = new byte[0];
            }
            return bytes;
        }
#endif

    }
}
