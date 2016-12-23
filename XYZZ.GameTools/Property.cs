using System;
using System.Threading;
using System.Threading.Tasks;
using XYZZ.Tools;

namespace XYZZ.GameTools
{
    /// <summary>
    /// 几率类型属性
    /// </summary>
    public class RateProperty : IProperty<double>
    {
        #region 本地参数
        private double originalValue, buffValue, debuffValue;
        /// <summary>
        /// 原始值
        /// </summary>
        private double OriginalValue
        {
            get { return originalValue; }
            set { originalValue = value.Range(0, 1); SetPropertyInfo(); }
        }

        /// <summary>
        /// 补偿值
        /// </summary>
        private double CompensationValue { get; set; }

        /// <summary>
        /// 增益值
        /// </summary>
        private double BuffValue
        {
            get { return buffValue; }
            set { buffValue = Math.Max(value, 0); }
        }

        /// <summary>
        /// 减益值
        /// </summary>
        private double DebuffValue
        {
            get { return debuffValue; }
            set { debuffValue = Math.Min(value, 0); }
        }

        /// <summary>
        /// 增益取消标记
        /// </summary>
        private CancellationTokenSource BuffCancel;

        /// <summary>
        /// 减益取消标记
        /// </summary>
        private CancellationTokenSource DebuffCancel;
        #endregion

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="value">几率数值(0~1)</param>
        public RateProperty(double value)
        {
            OriginalValue = value;
            BuffValue = 0;
            DebuffValue = 0;
            BuffCancel = new CancellationTokenSource();
            DebuffCancel = new CancellationTokenSource();
        }

        /// <summary>
        /// 属性显示信息
        /// </summary>
        public ShowInfo PropertyInfo { get; private set; }

        /// <summary>
        /// 概率值
        /// </summary>
        public double PropertyValue
        {
            get { return (OriginalValue + BuffValue + DebuffValue + CompensationValue).Range(0, 1); }
        }

        /// <summary>
        /// 设定属性显示信息
        /// </summary>
        private void SetPropertyInfo()
        {
            double value = BuffValue + DebuffValue;
            PropertyInfo = new ShowInfo()
            {
                Text = (PropertyValue * 100).ToString("###%"),
                Status = (value == 0 ? PropertyStatusEnum.Normal : (value > 0 ? PropertyStatusEnum.Buff : PropertyStatusEnum.Debuff))
            };
        }

        /// <summary>
        /// 永久增益
        /// </summary>
        /// <param name="addedValue">增益值</param>
        public void Buff(double addedValue)
        {
            OriginalValue += addedValue;
        }

        /// <summary>
        /// 限时增益
        /// </summary>
        /// <param name="addedValue">增益概率(-1~1)</param>
        /// <param name="millisecond">增益时间</param>
        public void Buff(double addedValue, int millisecond)
        {
            if (addedValue != 0)
            {
                if (addedValue > 0)
                {
                    //增益
                    BuffValue += addedValue;
                    Task.Run(() =>
                    {
                        Thread.Sleep(millisecond);
                        BuffValue -= addedValue;
                    }, BuffCancel.Token);
                }
                else
                {
                    //减益
                    DebuffValue += addedValue;
                    Task.Run(() =>
                    {
                        Thread.Sleep(millisecond);
                        DebuffValue -= addedValue;
                    }, DebuffCancel.Token);
                }
                SetPropertyInfo();
            }
        }

        /// <summary>
        /// 清除所有减益效果
        /// </summary>
        /// <param name="clearMode">清除模式</param>
        public void Clear(ClearModeEnum clearMode)
        {
            if (clearMode == ClearModeEnum.Buff || clearMode == ClearModeEnum.All)
            {
                BuffCancel.Cancel();
                BuffValue = 0;
            }
            if (clearMode == ClearModeEnum.Debuff || clearMode == ClearModeEnum.All)
            {
                DebuffCancel.Cancel();
                DebuffValue = 0;
            }
            SetPropertyInfo();
        }

        /// <summary>
        /// 返回随机结果
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            int seed = Math.Abs((int)BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0));
            int result = new Random(seed).Next(1, 1000);
            if (PropertyValue * 1000 >= result)
            {
                //成功，每次成功都会降低下次的概率
                CompensationValue = Math.Min(0, CompensationValue - 0.3 * (1 - PropertyValue));
                return true;
            }
            else
            {
                //失败，每次失败都会提高下次的概率
                CompensationValue = Math.Max(0, CompensationValue + 0.3 * PropertyValue);
                return false;
            }
        }
    }

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
