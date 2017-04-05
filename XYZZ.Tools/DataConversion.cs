using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace XYZZ.Tools
{
    public static partial class Extands
    {
        private const string Wildcard = "Result";

        /// <summary>
        /// 实体类转Json
        /// </summary>
        /// <param name="data">实体类数据</param>
        /// <returns></returns>
        public static JObject ToJson(this object data)
        {
            if (data.GetType() == typeof(string)) { return ToJson(data.ToString()); }
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(data.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, data);
                return JObject.Parse(Encoding.UTF8.GetString(memoryStream.ToArray()));
            }
        }

        /// <summary>
        /// 字典型转Json
        /// </summary>
        /// <param name="data">字典数据</param>
        /// <returns></returns>
        public static JObject ToJson(this Dictionary<string, string> data)
        {
            JObject jObject = new JObject();
            foreach (string key in data.Keys)
            {
                jObject.Add(key, data[key]);
            }
            return jObject;
        }

        /// <summary>
        /// 字符串转换为Json
        /// </summary>
        /// <param name="data">字符串信息</param>
        /// <returns></returns>
        public static JObject ToJson(this string data)
        {
            JObject jObject = new JObject();
            jObject.Add(Wildcard, data);
            return jObject;
        }

        /// <summary>
        /// 实体类转换为Xml
        /// </summary>
        /// <param name="data">实体类数据</param>
        /// <returns></returns>
        public static string ToXml(this object data)
        {
            if (data.GetType() == typeof(string)) { return ToXml(data.ToString()); }
            XmlSerializer serializer = new XmlSerializer(data.GetType());
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.Serialize(stream, data);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// 字符串转换为Xml
        /// </summary>
        /// <param name="data">字符串信息</param>
        /// <returns></returns>
        public static string ToXml(this string data)
        {
            return string.Format("<{0}>{1}</{0}>", Wildcard, data);
        }

        /// <summary>
        /// 字典型转换为实体类
        /// </summary>
        /// <param name="parameterInfo">数据原型</param>
        /// <param name="data">参数数据</param>
        /// <returns></returns>
        public static object ToEntity(ParameterInfo parameterInfo, Dictionary<string, object> data)
        {
            Type tyoe = parameterInfo.ParameterType;
            if (parameterInfo.ParameterType.Namespace.Split('.')[0] == nameof(XYZZ))
            {
                //实体类
                object result = Activator.CreateInstance(parameterInfo.ParameterType);
                foreach (PropertyInfo property in result.GetType().GetProperties())
                {
                    if (data.ContainsKey(property.Name))
                    {
                        property.SetValue(result, SafeParse(data[property.Name], property.PropertyType), null);
                    }
                }
                return result;
            }
            else
            {
                switch (parameterInfo.ParameterType.Name)
                {
                    case "JObject":
                        return ToJson(data);
                    default:
                        return data.ContainsKey(parameterInfo.Name) ?
                            SafeParse(data[parameterInfo.Name], parameterInfo.ParameterType) :
                            DefaultValue(parameterInfo.ParameterType);
                }
            }
        }

        /// <summary>
        /// 数据表转换为实体类
        /// </summary>
        /// <typeparam name="T">实体类类型</typeparam>
        /// <param name="data">数据表</param>
        /// <returns></returns>
        public static List<T> ToEntity<T>(DataTable data)
        {
            List<T> resultList = new List<T>();
            foreach (DataRow row in data.Rows)
            {
                T result = Activator.CreateInstance<T>();
                foreach (PropertyInfo property in result.GetType().GetProperties())
                {
                    if (data.Columns.Contains(property.Name))
                    {
                        property.SetValue(result, SafeParse(row[property.Name], property.PropertyType), null);
                    }
                }
                resultList.Add(result);
            }
            return resultList;
        }
    }
}
