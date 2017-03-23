/*
*
* 文件名称：DBParameterDerive.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：参数推导基类
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Data;
using System.Runtime.InteropServices;

namespace Database
{
    /// <summary>
    /// 参数推导基类
    /// OleDb和ODBC是?非命名参数
    /// SQL SERVER是以@开头的命名参数
    /// ORACLE是以:开头的命名参数
    /// 详见MSDN的XXXCommand.Parameters 
    /// </summary>
    public abstract class DBParameterDerive : IDBParameterDerive
    {
        private static readonly Regex m_Regex = new Regex(@"\{\d+\}",
            RegexOptions.Compiled | RegexOptions.Multiline);

        protected static Regex Regex
        {
            get { return DBParameterDerive.m_Regex; }
        }

        protected static int GetMatchIndex(Match m)
        {
            string strValue = m.Value.Substring(1, m.Value.Length - 2);
            return Convert.ToInt32(strValue);
        }

        public abstract void Derive(System.Data.Common.DbCommand cmd, string query, params Object[] parameters);


        #region static
       

        public static List<DBParameterDerive> ParameterDeriveList
        {
            get
            {
                return GetParameterDeriveList();
            }
        }

        private static List<DBParameterDerive> GetParameterDeriveList()
        {
            List<DBParameterDerive> lParameterDerive = new List<DBParameterDerive>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                GetParameterDerive(assembly, lParameterDerive);
            }

            return lParameterDerive;
        }

        private static void GetParameterDerive(Assembly assembly, List<DBParameterDerive> lParameterDerive)
        {
            DBParameterDerive parameterDerive = null;
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                parameterDerive = null;
                if (!t.IsClass)
                    continue;

                if (!t.IsPublic)
                    continue;

                if (t.BaseType == typeof(DBParameterDerive))
                {
                    try
                    {
                        PropertyInfo pi = t.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
                        if (pi != null)
                        {
                            parameterDerive = pi.GetValue(null, null) as DBParameterDerive;
                        }
                        if (parameterDerive == null)
                        {
                            parameterDerive = Activator.CreateInstance(t) as DBParameterDerive;                            
                        }
                        lParameterDerive.Add(parameterDerive);
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion
    }
}
