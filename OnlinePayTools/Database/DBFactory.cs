/*
*
* 文件名称：DBFactory.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：创建数据库封装类
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

using SqlDatabase = Database.DBDatabase
    <
    System.Data.SqlClient.SqlConnection,
    System.Data.SqlClient.SqlCommand,
    System.Data.SqlClient.SqlParameter,
    System.Data.SqlClient.SqlDataAdapter,
    System.Data.SqlClient.SqlCommandBuilder
    >;


using OleDbDatabase = Database.DBDatabase
    <
    System.Data.OleDb.OleDbConnection,
    System.Data.OleDb.OleDbCommand,
    System.Data.OleDb.OleDbParameter,
    System.Data.OleDb.OleDbDataAdapter,
    System.Data.OleDb.OleDbCommandBuilder
    >;


using OdbcDatabase = Database.DBDatabase
    <
    System.Data.Odbc.OdbcConnection,
    System.Data.Odbc.OdbcCommand,
    System.Data.Odbc.OdbcParameter,
    System.Data.Odbc.OdbcDataAdapter,
    System.Data.Odbc.OdbcCommandBuilder
    >;
using System.Data.Common;



#if USE_ORACLEDATABASE
using OracleDatabase = Database.DBDatabase
    <
    System.Data.OracleClient.OracleConnection,
    System.Data.OracleClient.OracleCommand,
    System.Data.OracleClient.OracleParameter,
    System.Data.OracleClient.OracleDataAdapter,
    System.Data.OracleClient.OracleCommandBuilder
    >;
#endif


namespace Database
{
    /// <summary>
    /// 创建数据库封装类
    /// </summary>
    public static partial class DBFactory
    {
        static DBFactory()
        {
            LoadExecutingReferencedAssemblies();
        }

        private static void LoadExecutingReferencedAssemblies()
        {
            try
            {
	            AssemblyName[] assemblyNames = Assembly.GetExecutingAssembly().GetReferencedAssemblies();
	            foreach (AssemblyName assemblyName in assemblyNames)
	            {
	                if (assemblyName.Name.StartsWith("Interop."))
	                {
	                    continue;
	                }
	                try
	                {
	                    Assembly.Load(assemblyName);
	                }
	                catch
	                {
	
	                }
	            }
            }
            catch
            {
            	
            }
        }

        private static readonly Type m_dbDatabase = typeof(DBDatabase<,,,,>);

        public static Type DbDatabase
        {
            get
            {
                return DBFactory.m_dbDatabase;
            }
        }

        /// <summary>
        /// 创建IDBDatabase
        /// </summary>
        /// <param name="strConnection">Connection的全称</param>
        /// <returns>创建的IDBDatabase</returns>
        public static IDBDatabase CreateDatabase(String strConnection)
        {
            List<DBProvider>  list = Database.DBProvider.ProviderList;
            DBProvider dbProvider = list.Find(delegate(DBProvider x) { return x.Connection.FullName == strConnection; });
            if (dbProvider == null)
            {
                return null;
            }
            IDBDatabase db = CreateDatabase(dbProvider);
            
            return db;
        }

        /// <summary>
        /// 创建IDBDatabase
        /// </summary>
        /// <param name="providerFactory">表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例</param>
        /// <returns>创建的IDBDatabase</returns>
        public static IDBDatabase CreateDatabase(DbProviderFactory providerFactory)
        {
            DBProvider dbProvider = new DBProvider();

            using (DbConnection cnn = providerFactory.CreateConnection())
            using (DbCommand cmd = providerFactory.CreateCommand())
            using (DbCommandBuilder cmdBuilder = providerFactory.CreateCommandBuilder())
            using (DbDataAdapter adapter = providerFactory.CreateDataAdapter())
            {
                DbParameter param = providerFactory.CreateParameter();
                dbProvider.Connection = cnn.GetType();
                dbProvider.Command = cmd.GetType();
                dbProvider.Parameter = param.GetType();
                dbProvider.CommandBuilder = cmdBuilder.GetType();
                dbProvider.DataAdapter = adapter.GetType();
            }
            
            IDBDatabase db = CreateDatabase(dbProvider);

            return db;
        }

        /// <summary>
        /// 创建IDBDatabase
        /// </summary>
        /// <param name="dbProvider">数据库驱动提供者</param>
        /// <returns>创建的IDBDatabase</returns>
        public static IDBDatabase CreateDatabase(DBProvider dbProvider)
        {
            Type type = DbDatabase.MakeGenericType(dbProvider.Connection, dbProvider.Command,
                dbProvider.Parameter, dbProvider.DataAdapter, dbProvider.CommandBuilder);

            IDBDatabase db = Activator.CreateInstance(type) as IDBDatabase;
            String strCnnName = dbProvider.Connection.FullName.ToUpper();
            if (strCnnName.Contains(".OLEDB.")
                || strCnnName.Contains(".ODBC."))
            {
                db.ParameterDerive = DBUnnamedParameterDerive.Instance;
            }
            else if (strCnnName.Contains("ORACLE"))
            {
                db.ParameterDerive = DBOracleParameterDerive.Instance;
            }
            else
            {
                db.ParameterDerive = DBNamedParameterDerive.Instance;
            }

            return db;
        }

        public static SqlDatabase CreateSqlDatabase()
        {
            SqlDatabase db = new SqlDatabase();
            db.ParameterDerive = DBNamedParameterDerive.Instance;
            return db;
        }

        public static OleDbDatabase CreateOleDbDatabase()
        {
            OleDbDatabase db = new OleDbDatabase();
            db.ParameterDerive = DBUnnamedParameterDerive.Instance;
            return db;
        }

        public static OdbcDatabase CreateOdbcDatabase()
        {
            OdbcDatabase db = new OdbcDatabase();
            db.ParameterDerive = DBUnnamedParameterDerive.Instance;
            return db;
        }

#if USE_ORACLEDATABASE
        public static OracleDatabase CreateOracleDatabase()
        {
            OracleDatabase db = new OracleDatabase();
            db.ParameterDerive = DBOracleParameterDerive.Instance;
            return db;
        }
#endif

    }
}
