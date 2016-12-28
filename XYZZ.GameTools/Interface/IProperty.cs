namespace XYZZ.GameTools
{
    /// <summary>
    /// 属性接口
    /// </summary>
    public  interface IProperty<T>
    {
        /// <summary>
        /// 属性显示信息
        /// </summary>
        ShowInfo PropertyInfo { get; }

        /// <summary>
        /// 原始属性值
        /// </summary>
        T PropertyValue { get; }

        /// <summary>
        /// 永久增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        void Buff(T addedValue);

        /// <summary>
        /// 限时增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        /// <param name="millisecond">增益时间</param>
        void Buff(T addedValue, int millisecond);

        /// <summary>
        /// 清除所有减益效果
        /// </summary>
        /// <param name="clearMode">清除模式</param>
        void Clear(ClearModeEnum clearMode);
    }

    /// <summary>
    /// 显示信息
    /// </summary>
    public class ShowInfo
    {
        /// <summary>
        /// 显示文字
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// 属性状态
        /// </summary>
        public PropertyStatusEnum Status { get; set; }
    }

    /// <summary>
    /// 清除模式
    /// </summary>
    public enum ClearModeEnum
    {
        /// <summary>
        /// 清除所有
        /// </summary>
        All,
        /// <summary>
        /// 清除增益效果
        /// </summary>
        Buff,
        /// <summary>
        /// 清除减益效果
        /// </summary>
        Debuff
    }

    /// <summary>
    /// 属性状态枚举
    /// </summary>
    public enum PropertyStatusEnum
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal,
        /// <summary>
        /// 增益
        /// </summary>
        Buff,
        /// <summary>
        /// 减益
        /// </summary>
        Debuff
    }
}
