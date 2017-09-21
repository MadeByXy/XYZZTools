namespace XYZZ.Tools
{
    /// <summary>
    /// 参数实体
    /// </summary>
    public class Parameter
    {
        /// <summary>
        /// 实例化参数
        /// </summary>
        public Parameter() { }

        /// <summary>
        /// 实例化参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数值</param>
        public Parameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 参数名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object Value { get; set; }
    }
}