using System.Collections.Generic;
using System.Drawing;

namespace XYZZ.GameTools
{
    /// <summary>
    /// 结果信息
    /// </summary>
    public struct Result
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// 结果信息
        /// </summary>
        string Message { get; }
    }

    /// <summary>
    /// 物品接口
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// 物品名称
        /// </summary>
        string Name { get; }

        /// <summary>
        /// 物品说明
        /// </summary>
        string Explain { get; }

        /// <summary>
        /// 物品图标
        /// </summary>
        Image Image { get; }

        /// <summary>
        /// 拥有者
        /// </summary>
        ICharacter Character { get; set; }

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <returns>是否执行成功</returns>
        Result Invoke();

        /// <summary>
        /// 使用物品
        /// </summary>
        /// <param name="Character">目标角色</param>
        Result Invoke(ICharacter Character);
    }
}
