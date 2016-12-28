using System;
using System.Threading;
using System.Threading.Tasks;
using XYZZ.Tools;

namespace XYZZ.GameTools.Instance
{
    /// <summary>
    /// 值类型属性
    /// </summary>
    public class ValueProperty : IProperty<double>
    {
        /// <summary>
        /// 属性显示信息
        /// </summary>
        public ShowInfo PropertyInfo { get; private set; }

        /// <summary>
        /// 原始属性值
        /// </summary>
        public double PropertyValue { get; private set; }

        /// <summary>
        /// 永久增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        public void Buff(double addedValue)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 限时增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        /// <param name="millisecond">增益时间</param>
        public void Buff(double addedValue, int millisecond)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 清除所有减益效果
        /// </summary>
        /// <param name="clearMode">清除模式</param>
        public void Clear(ClearModeEnum clearMode)
        {
            throw new NotImplementedException();
        }
    }
}
