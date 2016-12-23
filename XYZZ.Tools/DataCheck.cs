using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
#pragma warning disable 1591

namespace XYZZ.Tools
{
    /// <summary>
    /// 数据合法性验证
    /// </summary>
    public static class DataCheck
    {
        /// <summary>
        /// 验证数据是否合法
        /// </summary>
        /// <param name="parameter">数据</param>
        /// <param name="resultMessage">提示信息</param>
        /// <returns>验证结果</returns>
        public static bool AttributeCheck(IModel parameter, out string resultMessage)
        {
            if (parameter == null)
            {
                resultMessage = "传入值不能为空";
                return false;
            }
            else
            {
                foreach (PropertyInfo property in parameter.GetType().GetProperties())
                {
                    foreach (Attribute checkAttribute in Attribute.GetCustomAttributes(property).OrderByDescending(x => ((ICheck)x).Order))
                    {
                        string message = "";
                        if (!((ICheck)checkAttribute).Check(property.GetValue(parameter), ref message))
                        {
                            resultMessage = message;
                            return false;
                        }
                    }
                }
                resultMessage = parameter.Result();
                return true;
            }
        }

        /// <summary>
        /// 数据原型
        /// </summary>
        public interface IModel
        {
            /// <summary>
            /// 数据验证成功后返回的信息(预留)
            /// </summary>
            /// <returns></returns>
            string Result();
            /// <summary>
            /// 绑定数据
            /// </summary>
            /// <param name="data">数据源</param>
            void DataBind(JObject data);
            /// <summary>
            /// 绑定数据
            /// </summary>
            /// <param name="data">数据源</param>
            void DataBind(Dictionary<string, string> data);
            /// <summary>
            /// 验证数据是否正确
            /// <see cref="AttributeCheck(IModel, out string)"/>方法+自定义验证
            /// </summary>
            /// <param name="result"></param>
            /// <returns></returns>
            bool Check(out string result);
        }

        private interface ICheck
        {
            /// <summary>
            /// 调用顺序
            /// 建议：区间检测设为0，格式检测设为1，非空检测设为2
            /// </summary>
            int Order { get; }
            /// <summary>
            /// 字段名称
            /// </summary>
            string Name { get; set; }

            /// <summary>
            /// 验证数据是否合法
            /// </summary>
            /// <param name="value">值</param>
            /// <param name="resultMessage">错误信息</param>
            /// <returns></returns>
            bool Check(object value, ref string resultMessage);
        }

        /// <summary>
        /// 数字区间检测
        /// </summary>
        public class NumberIntervalAttribute : Attribute, ICheck
        {
            public string Name { get; set; }
            public int Order { get { return 0; } }
            public double Min { get; set; }
            public double Max { get; set; }

            public bool Check(object value, ref string resultMessage)
            {
                double realValue = double.Parse(value.ToString());
                if (Min <= realValue && Max >= realValue)
                {
                    return true;
                }
                else
                {
                    resultMessage = string.Format("{0}超出范围", Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// 时间区间检测
        /// </summary>
        public class DateIntervalAttribute : Attribute, ICheck
        {
            private string earliest, latest;
            public int Order { get { return 0; } }
            public string Name { get; set; } = "日期";
            private DateTime RarliestDate { get; set; }
            private DateTime LatestDate { get; set; }
            public string Earliest
            {
                get { return earliest; }
                set
                {
                    earliest = value;
                    RarliestDate = DateTime.Parse(earliest);
                }
            }
            public string Latest
            {
                get { return latest; }
                set
                {
                    latest = value;
                    LatestDate = DateTime.Parse(latest);
                }
            }

            public bool Check(object value, ref string resultMessage)
            {
                if (RarliestDate <= (DateTime)value && LatestDate >= (DateTime)value)
                {
                    return true;
                }
                else
                {
                    resultMessage = string.Format("{0}超出范围", Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// 时间格式检测
        /// </summary>
        public class DateAttribute : Attribute, ICheck
        {
            public string Name { get; set; }
            public int Order { get { return 1; } }
            /// <summary>
            /// 允许的时间格式
            /// </summary>
            public string[] Format { get; set; }

            public bool Check(object value, ref string resultMessage)
            {
                try
                {
                    DateTime.ParseExact(value.ToString(), Format, null, DateTimeStyles.None);
                    return true;
                }
                catch
                {
                    resultMessage = string.Format("{0}格式错误", Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// Email格式检测
        /// </summary>
        public class EmailAttribute : Attribute, ICheck
        {
            public string Name { get; set; } = "邮箱";
            public int Order { get { return 1; } }
            public bool Check(object value, ref string resultMessage)
            {
                if (Regex.IsMatch(value.ToString(), @"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[\w!#$%&'*+/=?^_`{|}~-]+)*@(?:[\w](?:[\w-]*[\w])?\.)+[\w](?:[\w-]*[\w])?"))
                {
                    return true;
                }
                else
                {
                    resultMessage = string.Format("{0}格式错误", Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// 枚举类型检测
        /// </summary>
        public class EnumAttribute : Attribute, ICheck
        {
            public string Name { get; set; }
            public int Order { get { return 1; } }
            public Type EnumType { get; set; }
            public bool Check(object value, ref string resultMessage)
            {
                if (Enum.IsDefined(EnumType, value))
                {
                    return true;
                }
                else
                {
                    resultMessage = string.Format("{0}格式错误", Name);
                    return false;
                }
            }
        }

        /// <summary>
        /// 非空检测
        /// </summary>
        public class NonNullAttribute : Attribute, ICheck
        {
            public string Name { get; set; }
            public int Order { get { return 2; } }
            public bool Check(object value, ref string resultMessage)
            {
                if (value != null)
                {
                    return true;
                }
                else
                {
                    resultMessage = string.Format("{0}不能为空", Name);
                    return false;
                }
            }
        }
    }
}