﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Timers;
using Config = System.Configuration.ConfigurationManager;

namespace XYZZ.Tools
{
    /// <summary>
    /// 数据库操作类
    /// </summary>
    public class DataBase
    {
        private enum DataBaseType
        {
            SqlServer,
            Oracle
        }
        private static DataBaseType dataBaseType { get; set; }
        private static DbConnection Conn;
        /// <summary>
        /// 数据库连接字段
        /// </summary>
        private static string ConnectionString { get; set; }
        private static Timer SwitchTimer { get; set; }
        private static void SetConn(string connectionString)
        {
            try
            {
                Conn = new SqlConnection(connectionString);
                dataBaseType = DataBaseType.SqlServer;
                return;
            }
            catch { }
            try
            {
                Conn = new OleDbConnection(connectionString);
                dataBaseType = DataBaseType.Oracle;
                return;
            }
            catch { }
        }

        /// <summary>
        /// <see cref="DataBase"/>的实例
        /// </summary>
        public static DataBase Instance = new DataBase();
        private DataBase()
        {
            try
            {
                ConnectionString = Config.AppSettings["ConnectionString"];
                SetConn(ConnectionString);

                //延迟关闭，避免多次重复开关数据库
                SwitchTimer = new Timer();
                SwitchTimer.Interval = 1000;
                SwitchTimer.Elapsed += new ElapsedEventHandler((object sender, ElapsedEventArgs e) => { Close(); });
                SwitchTimer.AutoReset = false;
            }
            catch { }
        }

        private static void TimerStart()
        {
            SwitchTimer.Stop();
            SwitchTimer.Start();
            Open();
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
            TimerStart();
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
            TimerStart();
            ExcuteNonQuery(sql);
        }

        private static DataTable GetDataTable(string sql)
        {
            try
            {
                DbCommand dbCommand = Conn.CreateCommand();
                dbCommand.CommandText = sql;
                DataTable dataTable = new DataTable();
                switch (dataBaseType)
                {
                    case DataBaseType.Oracle:
                        new OleDbDataAdapter((OleDbCommand)dbCommand).Fill(dataTable);
                        break;
                    case DataBaseType.SqlServer:
                        new SqlDataAdapter((SqlCommand)dbCommand).Fill(dataTable);
                        break;
                }
                return dataTable;
            }
            catch (Exception e) { throw e; }
        }

        private static int ExcuteNonQuery(string sql)
        {
            try
            {
                DbCommand dbCommand;
                dbCommand = Conn.CreateCommand();
                dbCommand.CommandText = sql;
                int result = dbCommand.ExecuteNonQuery();
                return result;
            }
            catch (Exception e) { throw e; }
        }

        private static void Open()
        {
            if (Conn.State != ConnectionState.Open)
            {
                Conn.Open();
            }
        }

        private static void Close()
        {
            if (Conn.State != ConnectionState.Closed)
            {
                Conn.Close();
            }
        }
    }
}