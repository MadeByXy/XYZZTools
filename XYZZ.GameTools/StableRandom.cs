using System;
using System.Threading;
using System.Threading.Tasks;
using XYZZ.Tools;

namespace XYZZ.GameTools
{
    /// <summary>
    /// 稳定随机
    /// </summary>
    public class StableRandom
    {
        private double originalRate;
        /// <summary>
        /// 原始几率
        /// </summary>
        public double OrginalRate
        {
            get { return originalRate; }
            private set
            {
                originalRate = value;
                ThisRate = originalRate;
            }
        }

        /// <summary>
        /// 当前几率
        /// </summary>
        private double ThisRate { get; set; }

        /// <summary>
        /// 实例化随机数
        /// </summary>
        /// <param name="rate">成功几率(0~1)</param>
        public StableRandom(double rate)
        {
            OrginalRate = rate.Range(0, 1);
        }

        /// <summary>
        /// 限时增加概率
        /// </summary>
        /// <param name="rate">增加的几率</param>
        /// <param name="second">时间(秒)</param>
        public void AddRate(double rate, int second)
        {
            double readAddRate = (OrginalRate + rate).Range(0, 1) - OrginalRate;
            OrginalRate += readAddRate;
            Task.Run(() =>
            {
                Thread.Sleep(second * 1000);
                OrginalRate -= readAddRate;
            });
        }

        /// <summary>
        /// 返回是否成功
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess()
        {
            int seed = Math.Abs((int)BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0));
            int result = new Random(seed).Next(1, 1000);
            if (ThisRate * 1000 >= result)
            {
                //成功，每次成功都会降低下次的概率
                ThisRate = (ThisRate * (1 - ThisRate)).Range(OrginalRate * OrginalRate, OrginalRate);
                return true;
            }
            else
            {
                //失败，每次失败都会提高下次的概率
                ThisRate = (ThisRate * (1 + ThisRate)).Range(OrginalRate, 1);
                return false;
            }
        }
    }
}
