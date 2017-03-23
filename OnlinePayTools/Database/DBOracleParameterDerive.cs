/*
*
* 文件名称：DBOracleParameterDerive.cs
* 当前版本：1.00
* 作    者：zyq5945
*	 Email: zyq5945@126.com
* 发布Blog: http://blog.csdn.net/zyq5945/article
* 完成日期：2013年01月07日
* 摘    要：ORACLE参数推导类，ORACLE是以:开头的命名参数,SQL里有“:”，但PARAMETER里没有“:”
*
*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Database
{
    /// <summary>
    /// ORACLE参数推导类，ORACLE是以:开头的命名参数,SQL里有“:”，但PARAMETER里没有“:”
    /// </summary>
    public class DBOracleParameterDerive:DBParameterDerive
    {
        private static DBParameterDerive m_Instance;

        public static DBParameterDerive Instance
        {
            get { return m_Instance; }
            private set { m_Instance = value; }
        }

        static DBOracleParameterDerive()
        {
            Instance = new DBOracleParameterDerive();
        }

        public override void Derive(System.Data.Common.DbCommand cmd, string query, params Object[] parameters)
        {
            if (parameters.Length == 0)
            {
                cmd.CommandText = query;
                return;
            }

            int nParamIndex = 0;
            //cmd.Parameters.Clear();
            cmd.CommandText = Regex.Replace(query, delegate (Match m) 
            {
                int nValueIndex = GetMatchIndex(m);
                Object oValue = parameters[nValueIndex];
                nParamIndex = nValueIndex;

                String paramName = String.Format("p{0}", nParamIndex);
                if (!cmd.Parameters.Contains(paramName))
                {
                    System.Data.Common.DbParameter param = cmd.CreateParameter();
                    param.ParameterName = paramName;
                    if (oValue != null)
                    {
                        param.Value = oValue;
                    }
                    else
                    {
                        param.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(param);
                }

                return String.Format(":{0}", paramName);
            });

        }
    }
}
