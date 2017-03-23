/*
*
* 文件名称：IDBDatabase.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：数据库接口
*
*/

using System;
namespace Database
{
    /// <summary>
    /// 数据库接口
    /// </summary>
    /// <typeparam name="TConnection">表示到数据库的连接</typeparam>
    /// <typeparam name="TCommand">表示要对数据源执行的 SQL 语句或存储过程</typeparam>
    /// <typeparam name="TParameter">表示 DbCommand 的参数，还可表示该参数到一个 DataSet 列的映射</typeparam>
    /// <typeparam name="TDataAdapter">表示用于填充 DataSet 和更新数据库的一组数据命令和到数据库的连接</typeparam>
    /// <typeparam name="TCommandBuilder">自动生成用于协调 DataSet 的更改与关联数据库的单表命令</typeparam>
    public interface IDBDatabase<TConnection, TCommand, TParameter, TDataAdapter, TCommandBuilder>
        : System.Data.IDbConnection, IDisposable
        where TConnection : global::System.Data.Common.DbConnection, new()
        where TCommand : global::System.Data.Common.DbCommand, new()
        where TParameter : global::System.Data.Common.DbParameter, new()
        where TDataAdapter : global::System.Data.Common.DbDataAdapter, new()
        where TCommandBuilder : global::System.Data.Common.DbCommandBuilder, new()
    {
        /// <summary>
        /// 开始数据库事务
        /// </summary>
        void BeginTrans();

        /// <summary>
        /// 以指定的隔离级别启动数据库事务
        /// </summary>
        /// <param name="il">指定事务的隔离级别</param>
        void BeginTrans(System.Data.IsolationLevel il);

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 数据库连接
        /// </summary>
        TConnection Connection { get; set; }
        /// <summary>
        /// 计算个数
        /// </summary>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>计算的个数</returns>
        int Count(DBTableAttribute tblAttr, string strWhere, params Object[] parameters);

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <returns>计算的个数</returns>
        int Count<T>() where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>计算的个数</returns>
        int Count<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>计算的个数</returns>
        int Count<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>计算的个数</returns>
        int Count<T>(string strWhere, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>计算的个数</returns>
        int Count<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>计算的个数</returns>
        int Count<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 创建Command
        /// </summary>
        TCommand CreateTCommand();

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <returns>创建的Command</returns>
        TCommand CreateTCommand(string strCommandText);

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <param name="cmdType">Command的CommandType</param>
        /// <returns>创建的Command</returns>
        TCommand CreateTCommand(string strCommandText, System.Data.CommandType cmdType);

        /// <summary>
        /// 创建CommandBuilder
        /// </summary>
        /// <returns>创建的CommandBuilder</returns>
        TCommandBuilder CreateTCommandBuilder();

        /// <summary>
        /// 创建Connection
        /// </summary>
        /// <returns>创建的Connection</returns>
        TConnection CreateTConnection();

        /// <summary>
        /// 创建DataAdapter
        /// </summary>
        /// <returns>创建的DataAdapter</returns>
        TDataAdapter CreateTDataAdapter();

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <returns>创建的Parameter</returns>
        TParameter CreateTParameter();

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <param name="strName">Parameter的ParameterName</param>
        /// <param name="oValue">Parameter的Value</param>
        /// <returns>创建的Parameter</returns>
        TParameter CreateTParameter(string strName, object oValue);

        /// <summary>
        /// 创建存储过程的Command，自动调用TCommandBuilder.DeriveParameters填充Command的Parameters
        /// </summary>
        /// <param name="strCommandText">Command的CommandText，存储过程名称</param>
        /// <returns>创建的Command</returns>
        TCommand CreateProcedureTCommand(string strCommandText);

        /// <summary>
        /// 设置CommandBuilder的参数
        /// 1.比如OLEDB驱动插入Access数据的时报错“insert into语法错误”，可能是因为字段名称占用了Access关键字，需要设置QuotePrefix为"[",QuoteSuffix设置为"]"
        /// 2.要做批量更新的时候设置cmdBuilder.DataAdapter.UpdateBatchSize为批更新数量
        /// </summary>
        event DBSetCommandBuilder DBSetCommandBuilder;

        /// <summary>
        /// 参数推导之前,可以做SQL日志记录
        /// </summary>
        event DBBeforeParameterDerive DBBeforeParameterDerive;

        /// <summary>
        /// 参数推导之后,可以做SQL日志记录
        /// </summary>
        event DBAfterParameterDerive DBAfterParameterDerive;

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>删除的行数</returns>
        int Delete(DBTableAttribute tblAttr, string strWhere, params Object[] parameters);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns>删除的行数</returns>
        int Delete<T>() where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(string strWhere, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 执行SQL语句，并返回影响的行数
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>影响的行数</returns>
        int ExcuteNonQuery(string strSQL, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DbDataReader
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DbDataReader</returns>
        System.Data.Common.DbDataReader ExecuteDataReader(string strSQL, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="cmdBuilder">生成的CommandBuilder</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        System.Data.DataSet ExecuteDataSet(string strSQL, out TCommand cmd, out TDataAdapter dataAdapter, out TCommandBuilder cmdBuilder, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        System.Data.DataSet ExecuteDataSet(string strSQL, out TCommand cmd, out TDataAdapter dataAdapter, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        System.Data.DataSet ExecuteDataSet(string strSQL, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回第一行第一列的值
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>第一行第一列的值</returns>
        DBValue ExecuteScalar(string strSQL, params Object[] parameters);

        /// <summary>
        /// 事务是否未提交或者回滚
        /// </summary>
        bool HasTransaction { get; }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Insert<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Insert<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Insert<T>(System.Collections.Generic.List<T> tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Insert<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Insert<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Insert<T>(T tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Save<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Save<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Save<T>(System.Collections.Generic.List<T> tValue, string strSQL, string strPrimeKey, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Save<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Save<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Save<T>(T tValue, string strSQL, string strPrimeKey, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 获取第一行第一列的值，一般用来获取自增字段值
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>第一行第一列的值</returns>
        DBValue Scalar<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Select<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strCondition">SQL语句的WHERE子句,ORDER子句或者LIMIT子句任意组合，为空则查询整个表</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr, string strCondition) where T : class, IDBOperability, new();

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
        void Select<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr, string strCondition, string strTop, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strCondition">SQL语句的WHERE子句,ORDER子句或者LIMIT子句任意组合，为空则查询整个表</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, string strCondition) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 当前事务
        /// </summary>
        System.Data.Common.DbTransaction Transaction { get; set; }

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Update<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Update<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Update<T>(System.Collections.Generic.List<T> tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Update<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Update<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Update<T>(T tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 参数推导
        /// </summary>
        IDBParameterDerive ParameterDerive { get; set; }
    }


    /// <summary>
    /// 数据库接口
    /// </summary>
    public interface IDBDatabase : System.Data.IDbConnection, IDisposable
    {
        /// <summary>
        /// 开始数据库事务
        /// </summary>
        void BeginTrans();

        /// <summary>
        /// 以指定的隔离级别启动数据库事务
        /// </summary>
        /// <param name="il">指定事务的隔离级别</param>
        void BeginTrans(System.Data.IsolationLevel il);

        /// <summary>
        /// 提交事务
        /// </summary>
        void Commit();

        /// <summary>
        /// 数据库连接
        /// </summary>
        System.Data.Common.DbConnection Connection { get; set; }

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>计算的个数</returns>
        int Count(DBTableAttribute tblAttr, string strWhere, params Object[] parameters);

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <returns>计算的个数</returns>
        int Count<T>() where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>计算的个数</returns>
        int Count<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>计算的个数</returns>
        int Count<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>计算的个数</returns>
        int Count<T>(string strWhere, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>计算的个数</returns>
        int Count<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 计算个数
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>计算的个数</returns>
        int Count<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <returns>创建的Command</returns>
        System.Data.Common.DbCommand CreateTCommand();

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <returns>创建的Command</returns>
        System.Data.Common.DbCommand CreateTCommand(string strCommandText);

        /// <summary>
        /// 创建Command
        /// </summary>
        /// <param name="strCommandText">Command的CommandText</param>
        /// <param name="cmdType">Command的CommandType</param>
        /// <returns>创建的Command</returns>
        System.Data.Common.DbCommand CreateTCommand(string strCommandText, System.Data.CommandType cmdType);

        /// <summary>
        /// 创建CommandBuilder
        /// </summary>
        /// <returns>创建的CommandBuilder</returns>
        System.Data.Common.DbCommandBuilder CreateTCommandBuilder();

        /// <summary>
        /// 创建Connection
        /// </summary>
        /// <returns>创建的Connection</returns>
        System.Data.Common.DbConnection CreateTConnection();

        /// <summary>
        /// 创建DataAdapter
        /// </summary>
        /// <returns>创建的DataAdapter</returns>
        System.Data.Common.DbDataAdapter CreateTDataAdapter();

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <returns>创建的Parameter</returns>
        System.Data.Common.DbParameter CreateTParameter();

        /// <summary>
        /// 创建Parameter
        /// </summary>
        /// <param name="strName">Parameter的ParameterName</param>
        /// <param name="oValue">Parameter的Value</param>
        /// <returns>创建的Parameter</returns>
        System.Data.Common.DbParameter CreateTParameter(string strName, object oValue);

        /// <summary>
        /// 创建存储过程的Command，自动调用TCommandBuilder.DeriveParameters填充Command的Parameters
        /// </summary>
        /// <param name="strCommandText">Command的CommandText，存储过程名称</param>
        /// <returns>创建的Command</returns>
        System.Data.Common.DbCommand CreateProcedureTCommand(string strCommandText);

        /// <summary>
        /// 设置CommandBuilder的参数
        /// 1.比如OLEDB驱动插入Access数据的时报错“insert into语法错误”，可能是因为字段名称占用了Access关键字，需要设置QuotePrefix为"[",QuoteSuffix设置为"]"
        /// 2.要做批量更新的时候设置cmdBuilder.DataAdapter.UpdateBatchSize为批更新数量
        /// </summary>
        event DBSetCommandBuilder DBSetCommandBuilder;

        /// <summary>
        /// 参数推导之前,可以做SQL日志记录
        /// </summary>
        event DBBeforeParameterDerive DBBeforeParameterDerive;

        /// <summary>
        /// 参数推导之后,可以做SQL日志记录
        /// </summary>
        event DBAfterParameterDerive DBAfterParameterDerive;

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>删除的行数</returns>
        int Delete(DBTableAttribute tblAttr, string strWhere, params Object[] parameters);

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <returns>删除的行数</returns>
        int Delete<T>() where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="strWhere">SQL语句的WHERE子句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(string strWhere, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>删除的行数</returns>
        int Delete<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 执行SQL语句，并返回影响的行数
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>影响的行数</returns>
        int ExcuteNonQuery(string strSQL, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DbDataReader
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DbDataReader</returns>
        System.Data.Common.DbDataReader ExecuteDataReader(string strSQL, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="cmdBuilder">生成的CommandBuilder</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        System.Data.DataSet ExecuteDataSet(string strSQL, out System.Data.Common.DbCommand cmd, out System.Data.Common.DbDataAdapter dataAdapter, out System.Data.Common.DbCommandBuilder cmdBuilder, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="cmd">生成的Command</param>
        /// <param name="dataAdapter">生成的DataAdapter</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        System.Data.DataSet ExecuteDataSet(string strSQL, out System.Data.Common.DbCommand cmd, out System.Data.Common.DbDataAdapter dataAdapter, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回DataSet
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>返回的DataSet</returns>
        System.Data.DataSet ExecuteDataSet(string strSQL, params Object[] parameters);

        /// <summary>
        /// 执行SQL语句，并返回第一行第一列的值
        /// </summary>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        /// <returns>第一行第一列的值</returns>
        DBValue ExecuteScalar(string strSQL, params Object[] parameters);

        /// <summary>
        /// 事务是否未提交或者回滚
        /// </summary>
        bool HasTransaction { get; }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Insert<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Insert<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Insert<T>(System.Collections.Generic.List<T> tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Insert<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Insert<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Insert<T>(T tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 回滚事务
        /// </summary>
        void Rollback();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Save<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Save<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Save<T>(System.Collections.Generic.List<T> tValue, string strSQL, string strPrimeKey, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Save<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Save<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="strPrimeKey">主键名称</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Save<T>(T tValue, string strSQL, string strPrimeKey, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 获取第一行第一列的值，一般用来获取自增字段值
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <returns>第一行第一列的值</returns>
        DBValue Scalar<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Select<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        /// <param name="strCondition">SQL语句的WHERE子句,ORDER子句或者LIMIT子句任意组合，为空则查询整个表</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr, string strCondition) where T : class, IDBOperability, new();

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
        void Select<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr, string strCondition, string strTop, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strCondition">SQL语句的WHERE子句,ORDER子句或者LIMIT子句任意组合，为空则查询整个表</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, string strCondition) where T : class, IDBOperability, new();

        /// <summary>
        /// 查询数据
        /// 最好事先用Count(*)计算记录个数后，设置List的Capacity属性分配内存空间
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Select<T>(System.Collections.Generic.List<T> tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 当前事务
        /// </summary>
        System.Data.Common.DbTransaction Transaction { get; set; }

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Update<T>(System.Collections.Generic.List<T> tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Update<T>(System.Collections.Generic.List<T> tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// LIST中的数据必须和SQL查询出来的数据按主键排序顺序一一对应
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Update<T>(System.Collections.Generic.List<T> tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        void Update<T>(T tValue) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="tblAttr">数据表信息</param>
        void Update<T>(T tValue, DBTableAttribute tblAttr) where T : class, IDBOperability, new();

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">记录类</typeparam>
        /// <param name="tValue">数据记录</param>
        /// <param name="strSQL">Command要执行的SQL语句</param>
        /// <param name="oFlag">用来控制数据记录参与操作的字段</param>
        /// <param name="parameters">要传递给命令的参数数组</param>
        void Update<T>(T tValue, string strSQL, object oFlag, params Object[] parameters) where T : class, IDBOperability, new();

        /// <summary>
        /// 参数推导
        /// </summary>
        IDBParameterDerive ParameterDerive { get; set; }
    }
}
