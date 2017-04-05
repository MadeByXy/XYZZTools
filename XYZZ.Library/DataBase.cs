using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using Config = System.Configuration.ConfigurationSettings;
#pragma warning disable 0618

namespace XYZZ.Library
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class DataBase
    {
        private enum DataBaseEnum
        {
            SqlServer,
            Oracle
        }
        private DataBaseEnum DataBaseType { get; set; }
        /// <summary>
        /// 数据库连接字段
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// 当前线程名称
        /// </summary>
        private static string CurrentThreadName
        {
            get { return System.Threading.Thread.CurrentThread.Name ?? ""; }
        }

        /// <summary>
        /// <see cref="DataBase"/>的实例
        /// </summary>
        public static readonly DataBase Instance = new DataBase();
        /// <summary>
        /// <see cref="DataBase"/>的线程实例
        /// </summary>
        private static Dictionary<string, DataBase> ThreadDic = new Dictionary<string, DataBase>();

        private DataBase()
        {
            new DataBase(Config.AppSettings["ConnectionString"]);
        }

        private DataBase(string connectionString)
        {
            ConnectionString = connectionString;
            try
            {
                new SqlConnection(connectionString);
                DataBaseType = DataBaseEnum.SqlServer;
                return;
            }
            catch { }
            try
            {
                new OleDbConnection(connectionString);
                DataBaseType = DataBaseEnum.Oracle;
                return;
            }
            catch { }
            throw new Exception("不正确的数据库连接字段");
        }

        /// <summary>
        /// 添加线程实例
        /// </summary>
        /// <param name="connectionString">数据库连接参数</param>
        public static void SetThreadInstance(string connectionString)
        {
            lock (ThreadDic)
            {
                if (!ThreadDic.ContainsKey(CurrentThreadName))
                {
                    ThreadDic.Add(CurrentThreadName, new DataBase(connectionString));
                }
            }
        }

        /// <summary>
        /// 清除线程实例
        /// </summary>
        public static void RemoveThreadInstance()
        {
            ThreadDic.Remove(CurrentThreadName);
        }

        /// <summary>
        /// 执行SQL，并返回指定类型的结果
        /// <para>T类型说明：</para>
        /// <para><see cref="int"/>：查询返回查询行数，其余返回受影响的行数</para>
        /// <para><see cref="string"/>/<see cref="decimal"/>：返回第一项数据</para>
        /// <para><see cref="DataTable"/>：返回查询结果</para>
        /// <para><see cref="bool"/>：查询返回是否存在目标，其余返回是否存在受影响行</para>
        /// </summary>
        public static T ExecuteSql<T>(string sql, params object[] args)
        {
            sql = string.Format(sql, args);
            DataTable tempDataTable;
            switch (typeof(T).Name)
            {
                case nameof(String):
                    tempDataTable = GetDataTable(sql);
                    return (T)(object)(tempDataTable == null || tempDataTable.Rows.Count == 0 ? "" : tempDataTable.Rows[0][0].ToString());
                case nameof(Int32):
                    if (sql.Split(' ')[0].ToLower() == "select")
                    {
                        tempDataTable = GetDataTable(sql);
                        return (T)(object)(tempDataTable == null ? 0 : tempDataTable.Rows.Count);
                    }
                    else
                    {
                        return (T)(object)ExcuteNonQuery(sql);
                    }
                case nameof(DataTable):
                    return (T)(object)GetDataTable(sql);
                case nameof(Decimal):
                    return (T)(object)Convert.ToDecimal(ExecuteSql<string>(sql));
                case nameof(Boolean):
                    if (sql.Split(' ')[0].ToLower() == "select")
                    {
                        tempDataTable = GetDataTable(sql);
                        return (T)(object)(tempDataTable == null ? false : tempDataTable.Rows.Count != 0);
                    }
                    else
                    {
                        return (T)(object)(ExcuteNonQuery(sql) > 0);
                    }
                default:
                    return default(T);
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        public static void ExecuteSql(string sql, params object[] args)
        {
            sql = string.Format(sql, args);
            ExcuteNonQuery(sql);
        }

        private static DataTable GetDataTable(string sql)
        {
            if (ThreadDic.ContainsKey(CurrentThreadName))
            {
                return ThreadDic[CurrentThreadName].GetDataTableInstance(sql);
            }
            else
            {
                return Instance.GetDataTableInstance(sql);
            }
        }

        private static int ExcuteNonQuery(string sql)
        {
            if (ThreadDic.ContainsKey(CurrentThreadName))
            {
                return ThreadDic[CurrentThreadName].ExcuteNonQueryInstance(sql);
            }
            else
            {
                return Instance.ExcuteNonQueryInstance(sql);
            }
        }

        /// <summary>
        /// 获取数据库连接
        /// </summary>
        private DbConnection GetConnection()
        {
            switch (DataBaseType)
            {
                case DataBaseEnum.SqlServer:
                    return new SqlConnection(ConnectionString);
                case DataBaseEnum.Oracle:
                    return new OleDbConnection(ConnectionString);
                default:
                    return null;
            }
        }

        private DataTable GetDataTableInstance(string sql)
        {
            using (DbConnection conn = GetConnection())
            {
                conn.Open();
                DbCommand dbCommand = conn.CreateCommand();
                dbCommand.CommandText = sql;
                DataTable dataTable = new DataTable();
                switch (DataBaseType)
                {
                    case DataBaseEnum.SqlServer:
                        new SqlDataAdapter((SqlCommand)dbCommand).Fill(dataTable);
                        break;
                    case DataBaseEnum.Oracle:
                        new OleDbDataAdapter((OleDbCommand)dbCommand).Fill(dataTable);
                        break;
                }
                conn.Close();
                return dataTable;
            }
        }

        private int ExcuteNonQueryInstance(string sql)
        {
            using (DbConnection conn = GetConnection())
            {
                conn.Open();
                DbCommand dbCommand = conn.CreateCommand();
                dbCommand.CommandText = sql;
                int result = dbCommand.ExecuteNonQuery();
                conn.Close();
                return result;
            }
        }
    }
}
