using System;
using System.Drawing;

namespace XYZZ.GameTools.Instance
{
    /// <summary>
    /// 技能
    /// </summary>
    public class Skill : IItem
    {
        private Delegate Delegate { get; }

        /// <summary>
        /// 实例化
        /// </summary>
        /// <param name="skillId">技能ID</param>
        public Skill(string skillId)
        {
            Name = "";
            Explain = "";
            Image = null;
            Delegate = new Func<Result>(() =>
            {
                return new Result();
            });
        }

        /// <summary>
        /// 物品名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 技能说明
        /// </summary>
        public string Explain { get; }

        /// <summary>
        /// 物品图标
        /// </summary>
        public Image Image { get; }

        /// <summary>
        /// 技能拥有者
        /// </summary>
        public ICharacter Character { get; set; }

        /// <summary>
        /// 使用技能
        /// </summary>
        public Result Invoke()
        {
            return (Result)Delegate.DynamicInvoke();
        }

        /// <summary>
        /// 使用技能
        /// </summary>
        /// <param name="Character">目标角色</param>
        public Result Invoke(ICharacter Character)
        {
            return (Result)Delegate.DynamicInvoke(Character);
        }
    }
}
