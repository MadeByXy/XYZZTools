using System;
using System.Drawing;

namespace XYZZ.GameTools.Instance
{
    /// <summary>
    /// 装备
    /// </summary>
    public class Equipment : IItem
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="equipmentId">装备ID</param>
        public Equipment(string equipmentId)
        {
            Name = "";
            Explain = "";
            Image = null;
            Sell = false;
            Price = 0;
        }

        /// <summary>
        /// 装备部件
        /// </summary>
        public enum EquipmentEnum
        {
            /// <summary>
            /// 头部
            /// </summary>
            Head,
            /// <summary>
            /// 武器
            /// </summary>
            Weapon,
            /// <summary>
            /// 衣服
            /// </summary>
            Clothes,
            /// <summary>
            /// 鞋
            /// </summary>
            Shoe
        }

        /// <summary>
        /// 装备名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 装备说明
        /// </summary>
        public string Explain { get; }

        /// <summary>
        /// 装备位置
        /// </summary>
        public EquipmentEnum Position { get; }

        /// <summary>
        /// 装备图标
        /// </summary>
        public Image Image { get; }

        /// <summary>
        /// 装备拥有者
        /// </summary>
        public ICharacter Character { get; set; }

        /// <summary>
        /// 是否允许出售
        /// </summary>
        public bool Sell { get; }

        /// <summary>
        /// 是否允许交易
        /// </summary>
        public bool Transaction { get; }

        /// <summary>
        /// 出售价格
        /// </summary>
        public int Price { get; }

        /// <summary>
        /// 剩余使用次数
        /// </summary>
        public int Remaining { get; private set; }

        /// <summary>
        /// 佩戴装备
        /// </summary>
        public Result Invoke()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 佩戴装备
        /// </summary>
        /// <param name="Character">目标角色</param>
        public Result Invoke(ICharacter Character)
        {
            return Invoke();
        }
    }
}
