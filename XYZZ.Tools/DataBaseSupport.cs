using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace XYZZ.Tools
{    /// <summary>
     /// 数据库辅助类
     /// </summary>
    public static class DataBaseSupport
    {
        /// <summary>
        /// 查询所有含有表名的数据库
        /// </summary>
        /// <param name="tableName">表名称</param>
        /// <param name="containsData">是否必须含有数据</param>
        /// <returns></returns>
        public static List<string> GetLibraryFromTable(string tableName, bool containsData)
        {
            List<string> libraryList = new List<string>();
            DataTable data = DataBase.ExecuteSql<DataTable>("select USERNAME from all_users");
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    if (DataBase.ExecuteSql<bool>("select 1 from {0}.{1}", row[0], tableName) || !containsData)
                    {
                        libraryList.Add(row[0].ToString());
                    }
                }
                catch { }
            }
            return libraryList;
        }

        /// <summary>
        /// 查找所有可以执行语句的数据库
        /// （暂时不能匹配from table1,table2 格式）
        /// </summary>
        /// <param name="sql">要执行的SQL语句</param>
        /// <param name="containsData">是否必须含有数据</param>
        /// <returns></returns>
        public static List<string> GetLibraryFromSQL(string sql, bool containsData)
        {
            List<string> libraryList = new List<string>();
            DataTable data = DataBase.ExecuteSql<DataTable>("select USERNAME from all_users");
            foreach (DataRow row in data.Rows)
            {
                try
                {
                    if (DataBase.ExecuteSql<bool>(
                        Regex.Replace(sql, "(from )(|[a-z_0-9]*\\.)+([a-z_0-9\"]{1,})", string.Format("$1{0}.$3", row[0]), RegexOptions.IgnoreCase)) ||
                        !containsData)
                    {
                        libraryList.Add(row[0].ToString());
                    }
                }
                catch { }
            }
            return libraryList;
        }
    }
}
