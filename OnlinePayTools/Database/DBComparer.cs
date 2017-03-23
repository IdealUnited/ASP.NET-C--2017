/*
*
* 文件名称：DBComparer.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：记录主键比较类
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Database
{
    /// <summary>
    /// 记录主键比较类
    /// </summary>
    /// <typeparam name="T">记录类</typeparam>
    public class DBCompare<T> : IEqualityComparer<T>
        where T : IDBOperability
    {
        public bool Equals(T x, T y)
        {
            return x.PrimaryKey.Equals(y.PrimaryKey);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }

    /// <summary>
    /// 记录主键比较类
    /// </summary>
    public class DBCompare : IEqualityComparer<IDBOperability>
    {
        public bool Equals(IDBOperability x, IDBOperability y)
        {
            return x.PrimaryKey.Equals(y.PrimaryKey);
        }

        public int GetHashCode(IDBOperability obj)
        {
            return obj.GetHashCode();
        }
    }
}
