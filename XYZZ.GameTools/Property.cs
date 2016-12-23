using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYZZ.Tools;

namespace XYZZ.GameTools
{
    /// <summary>
    /// 几率类型属性
    /// </summary>
    public class RateProperty : IProperty<double>
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="value">几率数值（0~1）</param>
        public RateProperty(double value)
        {
            PropertyValue = value;
        }
        private double PPropertyValue;

        /// <summary>
        /// 属性显示信息
        /// </summary>
        public ShowInfo PropertyInfo { get; private set; }

        /// <summary>
        /// 概率值
        /// </summary>
        public double PropertyValue
        {
            get { return PPropertyValue; }
            private set
            {
                PPropertyValue = value.Range(0, 1);
                SetPropertyInfo();
            }
        }

        private void SetPropertyInfo()
        {
            PropertyInfo = new ShowInfo()
            {
                Text = (PropertyValue * 100).ToString("###%")
            };
        }

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
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 清除所有效果
        /// </summary>
        public void ClearAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 返回随机结果
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            throw new NotImplementedException();
        }
    }

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
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 清除所有效果
        /// </summary>
        public void ClearAll()
        {
            throw new NotImplementedException();
        }
    }
}
