/*
*
* 文件名称：DBTableAttribute.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库属性类, 标识了数据库类对应的表，主键，查询的字段
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Database
{
    /// <summary>
    /// 数据库属性类
    /// 标识了数据库类对应的表，主键，查询的字段
    /// </summary>
    public class DBTableAttribute : System.Attribute
    {
        /// <summary>
        /// 构造TableAttribute
        /// </summary>
        public DBTableAttribute()
        {

        }

        /// <summary>
        /// 构造TableAttribute
        /// </summary>
        /// <param name="strTableName">表名称</param>
        public DBTableAttribute(String strTableName)
        {
            TableName = strTableName;
        }

        /// <summary>
        /// 构造TableAttribute
        /// </summary>
        /// <param name="strTableName">表名称</param>
        /// <param name="strPrimeKey">主键名称</param>
        public DBTableAttribute(String strTableName,
            String strPrimeKey)
        {
            TableName = strTableName;
            PrimaryKey = strPrimeKey;
        }

        /// <summary>
        /// 构造TableAttribute
        /// </summary>
        /// <param name="strTableName">表名称</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="strFieldList">数据库表字段</param>
        public DBTableAttribute(String strTableName,
            String strPrimeKey, String strFieldList)
        {
            TableName = strTableName;
            PrimaryKey = strPrimeKey;
            FieldList = strFieldList;
        }

        #region member and property

        /// <summary>
        /// 数据库表名称
        /// </summary>
        private String m_TableName;

        /// <summary>
        /// 数据库表名称
        /// </summary>
        public String TableName
        {
            get
            {
                return m_TableName;
            }
            set
            {
                m_TableName = value;
            }
        }

        /// <summary>
        /// 数据库表主键
        /// </summary>
        private String m_PrimeKey = "Guid";

        /// <summary>
        /// 数据库表主键
        /// </summary>
        public String PrimaryKey
        {
            get
            {
                return m_PrimeKey;
            }
            set
            {
                m_PrimeKey = value;
            }
        }

        /// <summary>
        /// 数据库表字段
        /// </summary>
        private String m_FieldList = "*";

        /// <summary>
        /// 数据库表字段
        /// </summary>
        public String FieldList
        {
            get
            {
                return m_FieldList;
            }
            set
            {
                m_FieldList = value;
            }
        }

        /// <summary>
        /// 在拼接SQL中ParamterName名称
        /// OleDb和ODBC是?非命名参数
        /// SQL SERVER是以@开头的命名参数
        /// ORACLE是以:开头的命名参数
        /// 详见MSDN的XXXCommand.Parameters 
        /// </summary>
        private String m_SQLParamterName = "?";

        /// <summary>
        /// 在拼接SQL中ParamterName名称
        /// OleDb和ODBC是?非命名参数
        /// SQL SERVER是以@开头的命名参数
        /// ORACLE是以:开头的命名参数
        /// 详见MSDN的XXXCommand.Parameters 
        /// </summary>
        public String SQLParamterName
        {
            get
            {
                return m_SQLParamterName;
            }
            set
            {
                m_SQLParamterName = value;
            }
        }

        /// <summary>
        /// 是否是命名参数
        /// </summary>
        public Boolean IsNamedParamter
        {
            get
            {
                return SQLParamterName.Length > 1;
            }
        }

        /// <summary>
        /// DbParameter中使用的ParamterName
        /// OleDb和ODBC是?非命名参数
        /// SQL SERVER是以@开头的命名参数
        /// ORACLE是以:开头的命名参数，但拼接的时候会把:去掉
        /// 详见MSDN的XXXCommand.Parameters  
        /// </summary>
        private String m_ParamParamterName = "?";


        /// <summary>
        /// DbParameter中使用的ParamterName
        /// OleDb和ODBC是?非命名参数
        /// SQL SERVER是以@开头的命名参数
        /// ORACLE是以:开头的命名参数，但拼接的时候会把:去掉
        /// 详见MSDN的XXXCommand.Parameters 
        /// </summary>
        public String ParamParamterName
        {
            get
            {
                return m_ParamParamterName;
            }
            set
            {
                m_ParamParamterName = value;
            }
        }

        #endregion

        #region static
        /// <summary>
        /// 获取类的TableAttribute
        /// </summary>
        /// <typeparam name="T">需要获取的类</typeparam>
        /// <returns>类的TableAttribute</returns>
        public static DBTableAttribute GetAttribute<T>()
        {
            Type type = typeof(T);
            DBTableAttribute tblAttr = Array.Find(type.GetCustomAttributes(false),
                delegate (Object x) {return x is DBTableAttribute;}) as DBTableAttribute;
            if (tblAttr == null)
            {
                tblAttr = new DBTableAttribute(type.Name);
            }
            return tblAttr;
        }
        #endregion

    }
}
