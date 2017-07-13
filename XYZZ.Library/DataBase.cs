using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Reflection;
using Config = System.Configuration.ConfigurationSettings;
#pragma warning disable 0618

namespace XYZZ.Library
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class DataBase
    {
        #region  公共参数
        /// <summary>
        /// 当前数据库连接字段
        /// </summary>
        private string ConnectionString { get; set; }

        /// <summary>
        /// 当前数据库类型
        /// </summary>
        private Type DataBaseType { get; set; }

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
        public static readonly DataBase Instance = new DataBase(Config.AppSettings["ConnectionString"]);

        /// <summary>
        /// <see cref="DataBase"/>的线程实例
        /// </summary>
        private static Dictionary<string, DataBase> ThreadDic = new Dictionary<string, DataBase>();

        /// <summary>
        /// 可加载的数据库类型
        /// </summary>
        private static Dictionary<Type, Type> LibraryList;
        #endregion

        private DataBase(string connectionString)
        {
            if (LibraryList == null)
            {
                LibraryList = new Dictionary<Type, Type>() {
                    { typeof(SqlConnection) ,typeof(SqlDataAdapter)},
                    { typeof(OleDbConnection),typeof(OleDbDataAdapter)},
                    { typeof(OracleConnection),typeof(OracleDataAdapter)},
                    { typeof(SQLiteConnection),typeof(SQLiteDataAdapter)} };
            }
            ConnectionString = connectionString;
            foreach (Type type in LibraryList.Keys)
            {
                try
                {
                    Activator.CreateInstance(type, connectionString);
                    DataBaseType = type;
                    return;
                }
                catch { }
            }
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

        #region  执行SQL
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
            try
            {
                sql = string.Format(sql, args);
                MethodInfo method = typeof(DataBase).GetMethod(string.Format("ExecuteSql_{0}", typeof(T).Name), BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                {
                    return (T)method.Invoke(null, new object[] { sql });
                }
                else
                {
                    return default(T);
                }
            }
            catch { throw; }
        }

        /// <summary>
        /// 返回第一项数据
        /// </summary>
        private static string ExecuteSql_String(string sql)
        {
            DataTable dataTable = GetDataTable(sql);
            return dataTable == null || dataTable.Rows.Count == 0 ? "" : dataTable.Rows[0][0].ToString();
        }

        /// <summary>
        /// 返回第一项数据
        /// </summary>
        private static decimal ExecuteSql_Decimal(string sql)
        {
            return Convert.ToDecimal(ExecuteSql_String(sql));
        }

        /// <summary>
        /// 返回查询结果
        /// </summary>
        private static DataTable ExecuteSql_DataTable(string sql)
        {
            return GetDataTable(sql);
        }

        /// <summary>
        /// 查询返回查询行数，其余返回受影响的行数
        /// </summary>
        private static int ExecuteSql_Int32(string sql)
        {
            if (sql.Split(' ')[0].ToLower() == "select")
            {
                DataTable dataTable = GetDataTable(sql);
                return dataTable == null ? 0 : dataTable.Rows.Count;
            }
            else
            {
                return ExcuteNonQuery(sql);
            }
        }

        /// <summary>
        /// 查询返回是否存在目标，其余返回是否存在受影响行
        /// </summary>
        private static bool ExecuteSql_Boolean(string sql)
        {
            if (sql.Split(' ')[0].ToLower() == "select")
            {
                DataTable dataTable = GetDataTable(sql);
                return dataTable == null ? false : dataTable.Rows.Count != 0;
            }
            else
            {
                return ExcuteNonQuery(sql) > 0;
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
        #endregion

        #region  内部实现
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

        private DataTable GetDataTableInstance(string sql)
        {
            using (DbConnection conn = (DbConnection)Activator.CreateInstance(DataBaseType, ConnectionString))
            {
                conn.Open();
                DbCommand command = conn.CreateCommand();
                command.CommandText = sql;
                DataTable dataTable = new DataTable();
                ((DbDataAdapter)Activator.CreateInstance(LibraryList[DataBaseType], command)).Fill(dataTable);
                conn.Close();
                return dataTable;
            }
        }

        private int ExcuteNonQueryInstance(string sql)
        {
            using (DbConnection conn = (DbConnection)Activator.CreateInstance(DataBaseType, ConnectionString))
            {
                conn.Open();
                DbCommand conmand = conn.CreateCommand();
                conmand.CommandText = sql;
                int result = conmand.ExecuteNonQuery();
                conn.Close();
                return result;
            }
        }
        #endregion
    }
}
