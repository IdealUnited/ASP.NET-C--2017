/*
*
* 文件名称：DBDatabase.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库封装类，封装类数据常用操作增删改查，计数和保存操作
*
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

#pragma warning disable 0618


namespace Database
{
    /// <summary>
    /// 数据库封装类，封装类数据常用操作增删改查，计数和保存操作
    /// </summary>
    /// <typeparam name="TConnection">表示到数据库的连接</typeparam>
    /// <typeparam name="TCommand">表示要对数据源执行的 SQL 语句或存储过程</typeparam>
    /// <typeparam name="TParameter">表示 DbCommand 的参数，还可表示该参数到一个 DataSet 列的映射</typeparam>
    /// <typeparam name="TDataAdapter">表示用于填充 DataSet 和更新数据库的一组数据命令和到数据库的连接</typeparam>
    /// <typeparam name="TCommandBuilder">自动生成用于协调 DataSet 的更改与关联数据库的单表命令</typeparam>
    public class DBDatabase<TConnection, TCommand, TParameter, TDataAdapter, TCommandBuilder>
        : IDBDatabase<TConnection, TCommand, TParameter, TDataAdapter, TCommandBuilder>,
        IDBDatabase
        where TConnection : System.Data.Common.DbConnection, new()
        where TCommand : System.Data.Common.DbCommand, new()
        where TParameter : System.Data.Common.DbParameter, new()
        where TDataAdapter : System.Data.Common.DbDataAdapter, new()
        where TCommandBuilder : System.Data.Common.DbCommandBuilder, new()
    {

        #region 成员函数，属性及事件
        /// <summary>
        /// 数据库连接
        /// </summary>
        private TConnection m_Connection = new TConnection();

        /// <summary>
        /// 数据库连接
        /// </summary>
        public TConnection Connection
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        /// <summary>
        /// 当前事务
        /// </summary>
        private DbTransaction m_Transaction = null;

        /// <summary>
        /// 当前事务
        /// </summary>
        public DbTransaction Transaction
        {
            get { return m_Transaction; }
            set { m_Transaction = value; }
        }

        /// <summary>
        /// 参数推导
        /// </summary>
        private IDBParameterDerive m_ParameterDerive = DBUnnamedParameterDerive.Instance;

        /// <summary>
        /// 参数推导
        /// </summary>
        public IDBParameterDerive ParameterDerive
        {
            get { return m_ParameterDerive; }
            set { m_ParameterDerive = value; }
        }

        /// <summary>
        /// 设置CommandBuilder的参数
        /// 1.比如OLEDB驱动插入Access数据的时报错“insert into语法错误”，可能是因为字段名称占用了Access关键字，需要设置QuotePrefix为"[",QuoteSuffix设置为"]"
        /// 2.要做批量更新的时候设置cmdBuilder.DataAdapter.UpdateBatchSize为批更新数量
        /// </summary>
        public event DBSetCommandBuilder DBSetCommandBuilder;

        /// <summary>
        /// 参数推导之前,可以做SQL日志记录
        /// </summary>
        public event DBBeforeParameterDerive DBBeforeParameterDerive;

        /// <summary>
        /// 参数推导之后,可以做SQL日志记录
        /// </summary>
        public event DBAfterParameterDerive DBAfterParameterDerive;

        #endregion

        #region Base Interface

        #region ParamDerive
        private void ParamDerive(DbCommand cmd, string query, params Object[] parameters)
        {
            if (DBBeforeParameterDerive != null)
            {
                DBBeforeParameterDerive(this, cmd, query, parameters);
            }
            Debug.WriteLine(DateTime.Now.ToString() + " Before Derive:" + query);
            ParameterDerive.Derive(cmd, query, parameters);
            Debug.WriteLine(DateTime.Now.ToString() + " After Derive:" + cmd.CommandText);
            if (DBAfterParameterDerive != null)
            {
                DBAfterParameterDerive(this, cmd, query, parameters);
            }
        }
        #endregion

        #region DBCreater

        /// <summary>
        /// 创建Connection
        /// </summary>
        /// <returns>创建的Connection</returns>
        public TConnection CreateTConnection()
        {
            TConnection cnn = new TConnection();
            return cnn;
        }

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <param name="cmdType">Command的CommandType</param>
        /// <returns>创建的Command</returns>
        public TCommand CreateTCommand(String strCommandText, CommandType cmdType)
        {
            TCommand cmd = Connection.CreateCommand() as TCommand;
            cmd.Transaction = Transaction;
            cmd.Connection = Connection;
            cmd.CommandText = strCommandText;
            cmd.CommandType = cmdType;
            return cmd;
        }

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <returns>创建的Command</returns>
        public TCommand CreateTCommand()
        {
            TCommand cmd = CreateTCommand("", CommandType.Text);
            return cmd;
        }

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <returns>创建的Command</returns>
        public TCommand CreateTCommand(String strCommandText)
        {
            TCommand cmd = CreateTCommand(strCommandText, CommandType.Text);
            return cmd;
        }

        /// <summary>
        /// 创建存储过程的Command，自动调用TCommandBuilder.DeriveParameters填充Command的Parameters
        /// </summary>
        /// <param name="strCommandText">Command的CommandText，存储过程名称</param>
        /// <returns>创建的Command</returns>
        public TCommand CreateProcedureTCommand(String strCommandText)
        {
            TCommand cmd = CreateTCommand(strCommandText, CommandType.StoredProcedure);

            Type type = typeof(TCommandBuilder);
            System.Reflection.MethodInfo mi = type.GetMethod("DeriveParameters");
            if (mi != null)
            {
                mi.Invoke(null, new Object[] { cmd });
                foreach (TParameter item in cmd.Parameters)
                {
                    if (item.Value == null)
                    {
                        item.Value = DBNull.Value;
                    }
                }
            }
            return cmd;
        }

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <returns>创建的Parameter</returns>
        public TParameter CreateTParameter()
        {
            TParameter dbParam = new TParameter();
            return dbParam;
        }

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <param name="strName">Parameter的ParameterName</param>
        /// <param name="oValue">Parameter的Value</param>
        /// <returns>创建的Parameter</returns>
        public TParameter CreateTParameter(String strName, Object oValue)
        {
            TParameter dbParam = CreateTParameter();
            dbParam.ParameterName = strName;
            dbParam.Value = oValue;
            return dbParam;
        }

        /// <summary>
        /// 创建DataAdapter
        /// </summary>
        /// <returns>创建的DataAdapter</returns>
        public TDataAdapter CreateTDataAdapter()
        {
            TDataAdapter dataAdapter = new TDataAdapter();
            return dataAdapter;
        }

        /// <summary>
        /// 创建CommandBuilder
        /// </summary>
        /// <returns>创建的CommandBuilder</returns>
        public TCommandBuilder CreateTCommandBuilder()
        {
            TCommandBuilder cmdBuilder = new TCommandBuilder();
            return cmdBuilder;
        }

        #endregion

        #region Execute
        /// <summary>
        /// 执行SQL语句，并返回DbDataReader
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DbDataReader</returns>
        public DbDataReader ExecuteDataReader(string strSQL, params Object[] parameters)
        {
            using (TCommand cmd = CreateTCommand(strSQL))
            {
                ParamDerive(cmd, strSQL, parameters);

                DbDataReader dataReader = cmd.ExecuteReader();
                return dataReader;
            }
        }

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="cmdBuilder">生成的CommandBuilder</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        public DataSet ExecuteDataSet(string strSQL,
            out TCommand cmd, out TDataAdapter dataAdapter,
            out TCommandBuilder cmdBuilder,
            params Object[] parameters)
        {
            cmd = null;
            dataAdapter = null;
            cmdBuilder = null;
            try
            {
                cmd = CreateTCommand(strSQL);
                ParamDerive(cmd, strSQL, parameters);

                dataAdapter = CreateTDataAdapter();
                dataAdapter.SelectCommand = cmd;

                cmdBuilder = CreateTCommandBuilder();
                cmdBuilder.DataAdapter = dataAdapter;
                if (DBSetCommandBuilder != null)
                {
                    DBSetCommandBuilder(this, cmdBuilder);
                }

                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                return dataSet;
            }
            catch
            {
                using (cmd)
                using (dataAdapter)
                using (cmdBuilder)
                {
                }
                cmd = null;
                dataAdapter = null;
                cmdBuilder = null;

                throw;
            }

        }

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        public DataSet ExecuteDataSet(string strSQL,
            out TCommand cmd, out TDataAdapter dataAdapter,
            params Object[] parameters)
        {
            cmd = null;
            dataAdapter = null;
            try
            {
                cmd = CreateTCommand(strSQL);
                ParamDerive(cmd, strSQL, parameters);

                dataAdapter = CreateTDataAdapter();
                dataAdapter.SelectCommand = cmd;

                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet);

                return dataSet;
            }
            catch
            {
                using (cmd)
                using (dataAdapter)
                {
                }
                cmd = null;
                dataAdapter = null;

                throw;
            }
        }

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        public DataSet ExecuteDataSet(string strSQL, params Object[] parameters)
        {
            using (TCommand cmd = CreateTCommand(strSQL))
            {
                ParamDerive(cmd, strSQL, parameters);

                using (TDataAdapter dataAdapter = CreateTDataAdapter())
                {
                    dataAdapter.SelectCommand = cmd;

                    DataSet dataSet = new DataSet();
                    dataAdapter.Fill(dataSet);

                    return dataSet;
                }
            }

        }

        /// <summary>
        /// 执行SQL语句，并返回影响的行数
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>影响的行数</returns>
        public int ExcuteNonQuery(string strSQL, params Object[] parameters)
        {
            using (TCommand cmd = CreateTCommand(strSQL))
            {
                ParamDerive(cmd, strSQL, parameters);

                int nRet = cmd.ExecuteNonQuery();
                return nRet;
            }
        }


        /// <summary>
        /// 执行SQL语句，并返回第一行第一列的值
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>第一行第一列的值</returns>
        public DBValue ExecuteScalar(string strSQL, params Object[] parameters)
        {
            using (TCommand cmd = CreateTCommand(strSQL))
            {
                ParamDerive(cmd, strSQL, parameters);

                Object obj = cmd.ExecuteScalar();
                return DBValue.Convert(obj);
            }
        }

        /// <summary>
        /// 以指定的隔离级别启动数据库事务
        /// </summary>
        /// <param name="il">指定事务的隔离级别</param>
        public void BeginTrans(IsolationLevel il)
        {
            Transaction = Connection.BeginTransaction(il);
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        public void BeginTrans()
        {
            Transaction = Connection.BeginTransaction();
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void Commit()
        {
            if (!HasTransaction)
            {
                throw new InvalidOperationException();
            }

            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
        }

        /// <summary>
        /// 回滚事务
        /// </summary>
        public void Rollback()
        {
            if (!HasTransaction)
            {
                throw new InvalidOperationException();
            }

            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;
        }

        /// <summary>
        /// 事务是否未提交或者回滚
        /// </summary>
        public Boolean HasTransaction
        {
            get
            {
                return Transaction != null;
            }
        }

        #endregion

        #endregion

        #region Other
        /// <summary>
        /// 销毁类
        /// </summary>
        public void Dispose()
        {
            if (Connection != null)
            {
                if (Connection.State == ConnectionState.Open)
                {
                    Close();
                }
                Connection.Dispose();
                Connection = null;
            }
        }


        /// <summary>
        /// 隐式转换Connection
        /// </summary>
        /// <param name="dbDatabase">需要转换的DBDatabase</param>
        /// <returns>DBDatabase封装的Connection</returns>
        public static implicit operator TConnection(DBDatabase<TConnection, TCommand, TParameter, TDataAdapter, TCommandBuilder> dbDatabase)
        {
            return dbDatabase.Connection;
        }
        #endregion

        #region 业务数据接口

        #region Select
        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Select<T>(List<T> tValue, string strSQL, Object oFlag, params Object[] parameters)
            where T : class, IDBOperability, new()
        {
            tValue.Clear();

            using (DbDataReader dataReader = ExecuteDataReader(strSQL, parameters))
            {
                T tTemp = default(T);
                IDBRecord dbRecord = new DBDataReader(dataReader);
                while (dataReader.Read())
                {
                    tTemp = new T();
                    tTemp.GetRecordData(dbRecord, oFlag);
                    tValue.Add(tTemp);
                }
            }
        }

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strCondition">SQL语句的WHERE子句,ORDER子句或者LIMIT子句任意组合，为空则查询整个表</param>
        /// <param name="strTop">SQL语句的TOP子句<</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Select<T>(List<T> tValue, DBTableAttribute tblAttr,
            String strCondition, String strTop, Object oFlag, params Object[] parameters)
            where T : class, IDBOperability, new()
        {
            String strSQL = String.Format("SELECT {0} {1} FROM {2} {3}",
                strTop, tblAttr.FieldList, tblAttr.TableName, strCondition);
            Select<T>(tValue, strSQL, oFlag, parameters);
        }

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strCondition">SQL语句的WHERE子句,ORDER子句或者LIMIT子句任意组合，为空则查询整个表</param>
        public void Select<T>(List<T> tValue, DBTableAttribute tblAttr, String strCondition)
            where T : class, IDBOperability, new()
        {
            Select<T>(tValue, tblAttr, strCondition, "", null);
        }

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        public void Select<T>(List<T> tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            Select<T>(tValue, tblAttr, "", "", null);
        }

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strCondition">SQL语句的WHERE子句,ORDER子句或者LIMIT子句任意组合，为空则查询整个表</param>
        public void Select<T>(List<T> tValue, String strCondition)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Select<T>(tValue, tblAttr, strCondition, "", null);
        }

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        public void Select<T>(List<T> tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Select<T>(tValue, tblAttr, "", "", null);
        }

        #endregion

        #region Save
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Save<T>(T tValue, string strSQL, String strPrimeKey, Object oFlag, params Object[] parameters)
           where T : class, IDBOperability, new()
        {
            TCommand cmd = null;
            TDataAdapter dataAdapter = null;
            TCommandBuilder cmdBuilder = null;
            DataSet dataSet = null;

            dataSet = ExecuteDataSet(strSQL, out cmd,
                out dataAdapter, out cmdBuilder, parameters);

            using (cmd)
            using (dataAdapter)
            using (cmdBuilder)
            using (dataSet)
            {

                DataTable table = dataSet.Tables[0];
                DataRowCollection rows = table.Rows;

                #region
                DataColumn dataColumn = table.Columns[strPrimeKey];
                if (dataColumn == null)
                {
                    throw new NullReferenceException();
                }
                table.PrimaryKey = new DataColumn[] { dataColumn };
                DataRow dataRow = null;
                dataRow = rows.Find(tValue.PrimaryKey);
                if (dataRow == null)
                {
                    dataRow = table.NewRow();
                }
                IDBRecord dbRecord = new DBDataRow(dataRow);
                tValue.SetRecordData(dbRecord, oFlag);
                if (dataRow.RowState == DataRowState.Detached)
                {
                    rows.Add(dataRow);
                }
                #endregion

                dataAdapter.Update(dataSet);
            }

        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Save<T>(List<T> tValue, string strSQL, String strPrimeKey, Object oFlag, params Object[] parameters)
           where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return;
            }
            TCommand cmd = null;
            TDataAdapter dataAdapter = null;
            TCommandBuilder cmdBuilder = null;
            DataSet dataSet = null;

            dataSet = ExecuteDataSet(strSQL, out cmd,
                out dataAdapter, out cmdBuilder, parameters);

            using (cmd)
            using (dataAdapter)
            using (cmdBuilder)
            using (dataSet)
            {
                DataTable table = dataSet.Tables[0];
                DataRowCollection rows = table.Rows;

                #region
                DataColumn dataColumn = table.Columns[strPrimeKey];
                if (dataColumn == null)
                {
                    throw new NullReferenceException();
                }
                table.PrimaryKey = new DataColumn[] { dataColumn };
                DataRow dataRow = null;
                foreach (IDBOperability item in tValue)
                {
                    dataRow = rows.Find(item.PrimaryKey);
                    if (dataRow == null)
                    {
                        dataRow = table.NewRow();
                    }
                    IDBRecord dbRecord = new DBDataRow(dataRow);
                    item.SetRecordData(dbRecord, oFlag);
                    if (dataRow.RowState == DataRowState.Detached)
                    {
                        rows.Add(dataRow);
                    }
                }
                #endregion

                dataAdapter.Update(dataSet);
            }

        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        public void Save<T>(List<T> tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Save<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        public void Save<T>(List<T> tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return;
            }
            String strPrimeKeys = DBHelper.PrimaryKeyPlaceHolder(0, tValue.Count);
            String strSQL = String.Format("SELECT {0} FROM {1}  WHERE {2} IN ({3})",
                    tblAttr.FieldList, tblAttr.TableName,
                    tblAttr.PrimaryKey, strPrimeKeys);
            Save<T>(tValue, strSQL, tblAttr.PrimaryKey, null, DBHelper.GetPrimaryKeyArray<T>(tValue));
        }



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        public void Save<T>(T tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Save<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        public void Save<T>(T tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            String strSQL = string.Format("SELECT {0} FROM {1} WHERE {2} = {{0}}",
                tblAttr.FieldList, tblAttr.TableName, tblAttr.PrimaryKey);
            Save<T>(tValue, strSQL, tblAttr.PrimaryKey, null, tValue.PrimaryKey);
        }


        #endregion
        /// <summary>
        /// 获取第一行第一列的值，一般用来获取自增字段值
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>第一行第一列的值</returns>
        public DBValue Scalar<T>(T tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            String strSQL = string.Format("SELECT {0} FROM {1} WHERE {2} = {{0}}",
                tblAttr.FieldList, tblAttr.TableName, tblAttr.PrimaryKey);
            return ExecuteScalar(strSQL, tValue.PrimaryKey);
        }

        #region Scalar


        #endregion

        #region Insert
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Insert<T>(T tValue, string strSQL, Object oFlag, params Object[] parameters)
           where T : class, IDBOperability, new()
        {
            TCommand cmd = null;
            TDataAdapter dataAdapter = null;
            TCommandBuilder cmdBuilder = null;
            DataSet dataSet = null;

            dataSet = ExecuteDataSet(strSQL, out cmd,
                out dataAdapter, out cmdBuilder, parameters);

            using (cmd)
            using (dataAdapter)
            using (cmdBuilder)
            using (dataSet)
            {
                DataTable table = dataSet.Tables[0];
                DataRowCollection rows = table.Rows;

                #region
                DataRow dataRow = null;
                dataRow = table.NewRow();
                rows.Add(dataRow);
                IDBRecord dbRecord = new DBDataRow(dataRow);
                tValue.SetRecordData(dbRecord, oFlag);
                #endregion

                dataAdapter.Update(dataSet);
            }

        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Insert<T>(List<T> tValue, string strSQL, Object oFlag, params Object[] parameters)
           where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return;
            }
            TCommand cmd = null;
            TDataAdapter dataAdapter = null;
            TCommandBuilder cmdBuilder = null;
            DataSet dataSet = null;

            dataSet = ExecuteDataSet(strSQL, out cmd,
                out dataAdapter, out cmdBuilder, parameters);

            using (cmd)
            using (dataAdapter)
            using (cmdBuilder)
            using (dataSet)
            {
                DataTable table = dataSet.Tables[0];
                DataRowCollection rows = table.Rows;

                #region
                DataRow dataRow = null;
                foreach (IDBOperability item in tValue)
                {
                    dataRow = table.NewRow();
                    rows.Add(dataRow);
                    IDBRecord dbRecord = new DBDataRow(dataRow);
                    item.SetRecordData(dbRecord, oFlag);
                }
                #endregion

                dataAdapter.Update(dataSet);
            }
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        public void Insert<T>(List<T> tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Insert<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        public void Insert<T>(List<T> tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return;
            }
            String strSQL = String.Format("SELECT {0} FROM {1}  WHERE 1 = 2",
                    tblAttr.FieldList, tblAttr.TableName);
            Insert<T>(tValue, strSQL, null);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        public void Insert<T>(T tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Insert<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        public void Insert<T>(T tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            String strSQL = String.Format("SELECT {0} FROM {1}  WHERE 1 = 2",
                    tblAttr.FieldList, tblAttr.TableName);
            Insert<T>(tValue, strSQL, null);
        }

        #endregion

        #region Update
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Update<T>(T tValue, string strSQL, Object oFlag, params Object[] parameters)
           where T : class, IDBOperability, new()
        {
            TCommand cmd = null;
            TDataAdapter dataAdapter = null;
            TCommandBuilder cmdBuilder = null;
            DataSet dataSet = null;

            dataSet = ExecuteDataSet(strSQL, out cmd,
                out dataAdapter, out cmdBuilder, parameters);

            using (cmd)
            using (dataAdapter)
            using (cmdBuilder)
            using (dataSet)
            {
                DataTable table = dataSet.Tables[0];
                DataRowCollection rows = table.Rows;

                #region
                DataRow dataRow = null;
                if (rows.Count != 1)
                {
                    throw new Exception("更新行数和查询的数据记录个数不对应");
                }
                else
                {
                    dataRow = rows[0];
                }

                IDBRecord dbRecord = new DBDataRow(dataRow);
                tValue.SetRecordData(dbRecord, oFlag);
                #endregion

                dataAdapter.Update(dataSet);
            }

        }

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        public void Update<T>(List<T> tValue, string strSQL, Object oFlag, params Object[] parameters)
          where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return;
            }
            TCommand cmd = null;
            TDataAdapter dataAdapter = null;
            TCommandBuilder cmdBuilder = null;
            DataSet dataSet = null;

            dataSet = ExecuteDataSet(strSQL, out cmd,
                out dataAdapter, out cmdBuilder, parameters);

            using (cmd)
            using (dataAdapter)
            using (cmdBuilder)
            using (dataSet)
            {
                DataTable table = dataSet.Tables[0];

                #region
                DataRowCollection rows = dataSet.Tables[0].Rows;
                if (rows.Count != tValue.Count)
                {
                    throw new Exception("更新行数和查询的数据记录个数不对应");
                }
                int nIndex = 0;
                foreach (IDBOperability item in tValue)
                {
                    item.SetRecordData(new DBDataRow(rows[nIndex++]), oFlag);
                }
                #endregion

                dataAdapter.Update(dataSet);
            }

        }

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        public void Update<T>(List<T> tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Update<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        public void Update<T>(List<T> tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return;
            }
            String strPrimeKeys = DBHelper.PrimaryKeyPlaceHolder(0, tValue.Count);
            String strSQL = String.Format("SELECT {0} FROM {1}  WHERE {2} IN ({3}) ORDER BY {2}",
                    tblAttr.FieldList, tblAttr.TableName,
                    tblAttr.PrimaryKey, strPrimeKeys);
            Update<T>(tValue, strSQL, null, DBHelper.GetPrimaryKeyArray<T>(tValue));
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        public void Update<T>(T tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            Update<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        public void Update<T>(T tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            String strSQL = String.Format("SELECT {0} FROM {1}  WHERE {2} = {{0}}",
                    tblAttr.FieldList, tblAttr.TableName, tblAttr.PrimaryKey);
            Update<T>(tValue, strSQL, null, tValue.PrimaryKey);
        }
        #endregion

        #region Delete
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>删除的行数</returns>
        public int Delete(DBTableAttribute tblAttr, String strWhere, params Object[] parameters)
        {
            String strSQL = string.Format("DELETE FROM {0} {1}",
                tblAttr.TableName, strWhere);
            return ExcuteNonQuery(strSQL, parameters);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns>删除的行数</returns>
        public int Delete<T>()
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Delete(tblAttr, "");
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>删除的行数</returns>
        public int Delete<T>(String strWhere, params Object[] parameters)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Delete(tblAttr, strWhere, parameters);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>删除的行数</returns>
        public int Delete<T>(List<T> tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Delete<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>删除的行数</returns>
        public int Delete<T>(List<T> tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return 0;
            }
            String strPrimeKeys = DBHelper.PrimaryKeyPlaceHolder(0, tValue.Count);
            String strWhere = string.Format("WHERE {0} IN ({1})",
                tblAttr.PrimaryKey, strPrimeKeys);
            return Delete(tblAttr, strWhere, DBHelper.GetPrimaryKeyArray<T>(tValue));
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>删除的行数</returns>
        public int Delete<T>(T tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Delete<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>删除的行数</returns>
        public int Delete<T>(T tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            String strWhere = string.Format("WHERE {0} = {{0}}",
                tblAttr.PrimaryKey);
            return Delete(tblAttr, strWhere, tValue.PrimaryKey);
        }

        #endregion

        #region Count
        /// <summary>
        /// 计算个数
        /// </summary>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>计算的个数</returns>
        public int Count(DBTableAttribute tblAttr, String strWhere, params Object[] parameters)
        {
            String strSQL = string.Format("SELECT COUNT(*) FROM {0} {1}",
                tblAttr.TableName, strWhere);
            return ExecuteScalar(strSQL, parameters);
        }

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <returns>计算的个数</returns>
        public int Count<T>()
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Count(tblAttr, "");
        }

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>计算的个数</returns>
        public int Count<T>(String strWhere, params Object[] parameters)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Count(tblAttr, strWhere, parameters);
        }

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>计算的个数</returns>
        public int Count<T>(List<T> tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Count<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>计算的个数</returns>
        public int Count<T>(List<T> tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            if (tValue.Count == 0)
            {
                return 0;
            }
            String strPrimeKeys = DBHelper.PrimaryKeyPlaceHolder(0, tValue.Count);
            String strWhere = string.Format("WHERE {0} IN ({1})",
                tblAttr.PrimaryKey, strPrimeKeys);
            return Count(tblAttr, strWhere, DBHelper.GetPrimaryKeyArray<T>(tValue));
        }

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>计算的个数</returns>
        public int Count<T>(T tValue)
            where T : class, IDBOperability, new()
        {
            DBTableAttribute tblAttr = DBTableAttribute.GetAttribute<T>();
            return Count<T>(tValue, tblAttr);
        }

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>计算的个数</returns>
        public int Count<T>(T tValue, DBTableAttribute tblAttr)
            where T : class, IDBOperability, new()
        {
            String strWhere = string.Format("WHERE {0} = {{0}}",
                tblAttr.PrimaryKey);
            return Count(tblAttr, strWhere, tValue.PrimaryKey);
        }
        #endregion

        #endregion

        #region IDbConnection

        /// <summary>
        /// 为打开的连接更改当前数据库
        /// </summary>
        /// <param name="databaseName">为要使用的连接指定数据库名称</param>
        public void ChangeDatabase(string databaseName)
        {
            Connection.ChangeDatabase(databaseName);
        }

        /// <summary>
        /// 关闭与数据库的连接
        /// </summary>
        public void Close()
        {
            Connection.Close();
        }

        /// <summary>
        /// 获取在尝试建立连接时终止尝试并生成错误之前所等待的时间
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return Connection.ConnectionString;
            }
            set
            {
                Connection.ConnectionString = value;
            }
        }

        /// <summary>
        /// 获取或设置用于打开数据库的字符串
        /// </summary>
        public int ConnectionTimeout
        {
            get
            {
                return Connection.ConnectionTimeout;
            }
        }

        /// <summary>
        /// 创建并返回一个与该连接相关联的 Command 对象
        /// </summary>
        /// <returns>一个与该连接相关联的 Command 对象</returns>
        public IDbCommand CreateCommand()
        {
            return CreateTCommand();
        }

        /// <summary>
        /// 获取当前数据库或连接打开后要使用的数据库的名称
        /// </summary>
        public string Database
        {
            get
            {
                return Connection.Database;
            }
        }

        /// <summary>
        /// 打开一个数据库连接，其设置由提供程序特定的 Connection 对象的 ConnectionString 属性指定
        /// </summary>
        public void Open()
        {
            Connection.Open();
        }

        /// <summary>
        /// 获取连接的当前状态
        /// </summary>
        public ConnectionState State
        {

            get
            {
                return Connection.State;
            }
        }

        /// <summary>
        /// 以指定的 System.Data.IsolationLevel 值开始一个数据库事务
        /// </summary>
        /// <param name="il">System.Data.IsolationLevel 值之一</param>
        /// <returns>表示新事务的对象</returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return Connection.BeginTransaction(il);
        }

        /// <summary>
        /// 开始数据库事务
        /// </summary>
        /// <returns>表示新事务的对象</returns>
        public IDbTransaction BeginTransaction()
        {
            return Connection.BeginTransaction();
        }

        #endregion

        #region IDBDatabase

        /// <summary>
        /// 数据库连接
        /// </summary>
        DbConnection IDBDatabase.Connection
        {
            get
            {
                return Connection;
            }
            set
            {
                Connection = value as TConnection;
            }
        }

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <returns>创建的Command</returns>
        DbCommand IDBDatabase.CreateTCommand()
        {
            return CreateTCommand();
        }

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <returns>创建的Command</returns>
        DbCommand IDBDatabase.CreateTCommand(string strCommandText)
        {
            return CreateTCommand(strCommandText);
        }

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <param name="cmdType">Command的CommandType</param>
        /// <returns>创建的Command</returns>
        DbCommand IDBDatabase.CreateTCommand(string strCommandText, CommandType cmdType)
        {
            return CreateTCommand(strCommandText, cmdType);
        }

        /// <summary>
        /// 创建CommandBuilder
        /// </summary>
        /// <returns>创建的CommandBuilder</returns>
        DbCommandBuilder IDBDatabase.CreateTCommandBuilder()
        {
            return CreateTCommandBuilder();
        }

        /// <summary>
        /// 创建Connection
        /// </summary>
        /// <returns>创建的Connection</returns>
        DbConnection IDBDatabase.CreateTConnection()
        {
            return CreateTConnection();
        }

        /// <summary>
        /// 创建DataAdapter
        /// </summary>
        /// <returns>创建的DataAdapter</returns>
        DbDataAdapter IDBDatabase.CreateTDataAdapter()
        {
            return CreateTDataAdapter();
        }

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <returns>创建的Parameter</returns>
        DbParameter IDBDatabase.CreateTParameter()
        {
            return CreateTParameter();
        }

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <param name="strName">Parameter的ParameterName</param>
        /// <param name="oValue">Parameter的Value</param>
        /// <returns>创建的Parameter</returns>
        DbParameter IDBDatabase.CreateTParameter(string strName, object oValue)
        {
            return CreateTParameter(strName, oValue);
        }

        /// <summary>
        /// 创建存储过程的Command，自动调用TCommandBuilder.DeriveParameters填充Command的Parameters
        /// </summary>
        /// <param name="strCommandText">Command的CommandText，存储过程名称</param>
        /// <returns>创建的Command</returns>
        DbCommand IDBDatabase.CreateProcedureTCommand(string strCommandText)
        {
            return CreateProcedureTCommand(strCommandText);
        }

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="cmdBuilder">生成的CommandBuilder</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        DataSet IDBDatabase.ExecuteDataSet(string strSQL, out DbCommand cmd,
            out DbDataAdapter dataAdapter, out DbCommandBuilder cmdBuilder,
            params Object[] parameters)
        {
            TCommand cmd_ = null;
            TDataAdapter dataAdapter_ = null;
            TCommandBuilder cmdBuilder_ = null;
            DataSet dataSet = ExecuteDataSet(strSQL, out cmd_, out dataAdapter_, out cmdBuilder_,
                parameters);
            cmd = cmd_;
            dataAdapter = dataAdapter_;
            cmdBuilder = cmdBuilder_;
            return dataSet;
        }

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        DataSet IDBDatabase.ExecuteDataSet(string strSQL, out DbCommand cmd,
            out DbDataAdapter dataAdapter, params Object[] parameters)
        {
            TCommand cmd_ = null;
            TDataAdapter dataAdapter_ = null;
            DataSet dataSet = ExecuteDataSet(strSQL, out cmd_, out dataAdapter_,
                parameters);
            cmd = cmd_;
            dataAdapter = dataAdapter_;
            return dataSet;
        }

        #endregion

    }
}

#pragma warning restore 0618