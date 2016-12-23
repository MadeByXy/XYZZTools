using System;
using System.Drawing;

namespace XYZZ.GameTools
{
    /// <summary>
    /// 几率属性抽象类
    /// </summary>
    public abstract class RateProperty<T> : IProperty<T>
    {
        /// <summary>
        /// 属性显示信息
        /// </summary>
        public ShowInfo PropertyInfo { get; protected set; }

        /// <summary>
        /// 原始属性值
        /// </summary>
        public T PropertyValue { get; protected set; }

        /// <summary>
        /// 永久增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        public abstract void Buff(T addedValue);

        /// <summary>
        /// 限时增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        /// <param name="millisecond">增益时间</param>
        public abstract void Buff(T addedValue, int millisecond);

        /// <summary>
        /// 清除所有减益效果
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// 清除所有效果
        /// </summary>
        public abstract void ClearAll();

        /// <summary>
        /// 返回随机结果
        /// </summary>
        /// <returns></returns>
        public abstract bool IsSuccess();
    }

    /// <summary>
    /// 数值属性抽象类
    /// </summary>
    public abstract class ValueProperty<T> : IProperty<T>
    {
        /// <summary>
        /// 属性显示信息
        /// </summary>
        public ShowInfo PropertyInfo { get; protected set; }

        /// <summary>
        /// 原始属性值
        /// </summary>
        public T PropertyValue { get; protected set; }

        /// <summary>
        /// 永久增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        public abstract void Buff(T addedValue);

        /// <summary>
        /// 限时增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        /// <param name="millisecond">增益时间</param>
        public abstract void Buff(T addedValue, int millisecond);

        /// <summary>
        /// 清除所有减益效果
        /// </summary>
        public abstract void Clear();

        /// <summary>
        /// 清除所有效果
        /// </summary>
        public abstract void ClearAll();
    }

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
        void Clear();

        /// <summary>
        /// 清除所有效果
        /// </summary>
        void ClearAll();
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
        public PropertyEnum Status { get; set; }
    }

    /// <summary>
    /// 属性状态枚举
    /// </summary>
    public enum PropertyEnum
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
