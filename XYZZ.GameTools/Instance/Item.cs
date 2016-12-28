using System;
using System.Drawing;

namespace XYZZ.GameTools.Instance
{
    /// <summary>
    /// 消耗品
    /// </summary>
    public class Item : IItem
    {
        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="character">目标角色</param>
        /// <param name="itemId">物品ID</param>
        public Item(ICharacter character, string itemId)
        {
            Name = "";
            Explain = "";
            Image = null;
            Sell = false;
            Price = 0;
        }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 物品说明
        /// </summary>
        public string Explain { get; }

        /// <summary>
        /// 物品图标
        /// </summary>
        public Image Image { get; }

        /// <summary>
        /// 物品拥有者
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
        /// 使用消耗品
        /// </summary>
        public Result Invoke()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 使用消耗品
        /// </summary>
        /// <param name="Character">目标角色</param>
        public Result Invoke(ICharacter Character)
        {
            throw new NotImplementedException();
        }
    }
}
