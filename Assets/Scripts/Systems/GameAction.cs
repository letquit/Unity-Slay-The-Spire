using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏动作抽象基类，用于定义游戏中的各种动作行为
/// </summary>
public abstract class GameAction
{
    /// <summary>
    /// 预反应动作列表，在执行主要动作之前触发的相关反应动作
    /// </summary>
    public List<GameAction> PerReactions { get; private set; } = new();

    /// <summary>
    /// 执行中反应动作列表，在执行主要动作过程中触发的相关反应动作
    /// </summary>
    public List<GameAction> PerformReactions { get; private set; } = new();

    /// <summary>
    /// 后反应动作列表，在执行主要动作之后触发的相关反应动作
    /// </summary>
    public List<GameAction> PostReactions { get; private set; } = new();
}
