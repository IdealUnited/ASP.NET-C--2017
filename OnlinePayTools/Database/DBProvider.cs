/*
*
* 文件名称：DBProvider.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库驱动提供程序
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using System.Reflection;

namespace Database
{
    /// <summary>
    /// 数据库驱动提供程序
    /// </summary>
    public class DBProvider
    {
        #region member and attri

        protected Type m_Connection;

        public Type Connection
        {
            get
            {
                return m_Connection;
            }
            set
            {
                m_Connection = value;
            }
        }

        protected Type m_Command;

        public Type Command
        {
            get
            {
                return m_Command;
            }
            set
            {
                m_Command = value;
            }
        }

        protected Type m_Parameter;

        public Type Parameter
        {
            get
            {
                return m_Parameter;
            }
            set
            {
                m_Parameter = value;
            }
        }


        protected Type m_DataAdapter;

        public Type DataAdapter
        {
            get
            {
                return m_DataAdapter;
            }
            set
            {
                m_DataAdapter = value;
            }
        }


        protected Type m_CommandBuilder;

        public Type CommandBuilder
        {
            get
            {
                return m_CommandBuilder;
            }
            set
            {
                m_CommandBuilder = value;
            }
        }

        #endregion


        #region static

        public static List<DBProvider> ProviderList
        {
            get
            {
                return GetProviderList();
            }
        }

        private static List<DBProvider> GetProviderList()
        {
            List<DBProvider> lProvider = new List<DBProvider>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                GetProvider(assembly, lProvider);
            }

            return lProvider;
        }

        private static void GetProvider(Assembly assembly, List<DBProvider> lProvider)
        {
            List<Type> lConnection = new List<Type>();
            List<Type> lCommand = new List<Type>();
            List<Type> lParameter = new List<Type>();
            List<Type> lDataAdapter = new List<Type>();
            List<Type> lCommandBuilder = new List<Type>();

            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                if (!t.IsClass)
                    continue;

                if (!t.IsPublic)
                    continue;

                if (t.BaseType == typeof(DbConnection))
                {
                    lConnection.Add(t);
                }
                else if (t.BaseType == typeof(DbCommand))
                {
                    lCommand.Add(t);
                }
                else if (t.BaseType == typeof(DbParameter))
                {
                    lParameter.Add(t);
                }
                else if (t.BaseType == typeof(DbDataAdapter))
                {
                    lDataAdapter.Add(t);
                }
                else if (t.BaseType == typeof(DbCommandBuilder))
                {
                    lCommandBuilder.Add(t);
                }
            }

            foreach (Type cnn in lConnection)
            {
                String ns = cnn.Namespace;
                DBProvider provider = new DBProvider();
                provider.Connection = cnn;
                Predicate<Type> func = delegate(Type x) { return x.Namespace == ns; };
                provider.Command = lCommand.Find(func);
                provider.Parameter = lParameter.Find(func);
                provider.DataAdapter = lDataAdapter.Find(func);
                provider.CommandBuilder = lCommandBuilder.Find(func);
                if (provider.Command != null
                    && provider.Parameter != null
                    && provider.DataAdapter != null
                    && provider.CommandBuilder != null)
                {
                    lProvider.Add(provider);
                }
            }
        }

        #endregion

    }
}
