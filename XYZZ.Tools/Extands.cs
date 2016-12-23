using System;

namespace XYZZ.Tools
{
    /// <summary>
    /// 扩展方法
    /// </summary>
    public static class Extands
    {
        /// <summary>
        /// 获取范围区间的值
        /// </summary>
        public static T Range<T>(this T value, T range1, T range2) where T : IComparable
        {
            if (range1.CompareTo(range2) > 0)
            {
                T temp = range1;
                range1 = range2;
                range2 = temp;
            }
            return value.CompareTo(range1) < 0 ? range1 : value.CompareTo(range2) > 0 ? range2 : value;
        }

        /// <summary>
        /// 判断值是否处于区间
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public static bool Between<T>(this T value, T range1, T range2) where T : IComparable
        {
            return value.Range(range1, range2).CompareTo(value) == 0;
        }

        /// <summary>
        /// 判断数据内容是否为类型值
        /// </summary>
        public static bool IsType(this string text, string typeName)
        {
            Type type = Type.GetType(typeName);
            string name = typeof(int).Name;
            if (type == null)
            {
                throw new Exception("类型名称错误");
            }
            object[] param = new object[] { text, type.DefaultValue() };
            var Method = type.GetMethod("TryParse", new Type[] { typeof(string), type.MakeByRefType() });
            return Method == null || (bool)Method.Invoke(null, param);
        }

        /// <summary>
        /// 获取类型默认值
        /// </summary>
        public static object DefaultValue(this Type targetType)
        {
            return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
        }

        /// <summary>
        /// 将目标转换成指定类型的值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        public static T SafeParse<T>(this object value) where T : IConvertible
        {
            return value.PriSafeParse<T>();
        }

        /// <summary>
        /// 将目标转换成指定类型的值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败返回的默认值</param>
        /// <returns></returns>
        public static T SafeParse<T>(this object value, T defaultValue) where T : IConvertible
        {
            return value.PriSafeParse(defaultValue);
        }

        /// <summary>
        /// 将目标转换成指定类型的值
        /// </summary>
        /// <typeparam name="T">目标类型</typeparam>
        /// <param name="value"></param>
        /// <param name="defaultValue">转换失败返回的默认值</param>
        /// <param name="AllowEmpty">是否允许空值</param>
        /// <returns>AllowEmpty值为否时空值将做转换失败处理(返回defaultValue)</returns>
        public static T SafeParse<T>(this object value, T defaultValue, bool AllowEmpty) where T : IConvertible
        {
            return value.PriSafeParse(defaultValue, AllowEmpty);
        }

        private static T PriSafeParse<T>(this object value, T defaultValue = default(T), bool allowEmpty = true)
        {
            if (value == null)
            {
                return defaultValue;
            }
            bool nonEmpty;  //指示目标值是否非空
            switch (typeof(T).Name)
            {
                case nameof(String):
                    nonEmpty = !string.IsNullOrEmpty(value.ToString());
                    return allowEmpty || nonEmpty ? (T)(object)value.ToString() : defaultValue;
                default:
                    object[] param = new object[] { value.ToString(), default(T) };
                    var Method = typeof(T).GetMethod("TryParse", new Type[] {
                        typeof(string),
                        typeof(T).MakeByRefType()
                    });
                    if (Method != null)
                    {
                        bool result = (bool)Method.Invoke(null, param);
                        nonEmpty = !param[1].Equals(default(T));
                        return result && (allowEmpty || nonEmpty) ? (T)param[1] : defaultValue;
                    }
                    else
                    {
                        return defaultValue;
                    }
            }
        }
    }
}
