using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏动作系统，负责管理和执行游戏中的各种动作
/// </summary>
public class ActionSystem : Singleton<ActionSystem>
{
    private List<GameAction> reactions = null;
    public bool IsPerforming { get; private set; } = false;
    private static Dictionary<Type, List<Action<GameAction>>> preSubs = new();
    private static Dictionary<Type, List<Action<GameAction>>> postSubs = new();
    private static Dictionary<Type, Func<GameAction, IEnumerator>> performers = new();

    /// <summary>
    /// 执行指定的游戏动作
    /// </summary>
    /// <param name="action">要执行的游戏动作</param>
    /// <param name="OnPerformFinished">动作执行完成后的回调函数</param>
    public void Perform(GameAction action, Action OnPerformFinished = null)
    {
        if (IsPerforming) return;
        IsPerforming = true;
        StartCoroutine(Flow(action, () =>
        {
            IsPerforming = false;
            OnPerformFinished?.Invoke();
        }));
    }

    /// <summary>
    /// 添加一个反应动作到当前动作系统中
    /// </summary>
    /// <param name="gameAction">要添加的反应动作</param>
    public void AddReaction(GameAction gameAction)
    {
        reactions?.Add(gameAction);
    }

    /// <summary>
    /// 执行动作流程，包括预处理、执行和后处理阶段
    /// </summary>
    /// <param name="action">要执行的动作</param>
    /// <param name="OnFlowFinished">流程执行完成后的回调函数</param>
    /// <returns>协程迭代器</returns>
    private IEnumerator Flow(GameAction action, Action OnFlowFinished = null)
    {
        reactions = action.PerReactions;
        PerformSubscribers(action, preSubs);
        yield return PerformReactions();
        
        reactions = action.PerformReactions;
        yield return PerformPerformer(action);
        yield return PerformReactions();
        
        reactions = action.PostReactions;
        PerformSubscribers(action, postSubs);
        yield return PerformReactions();
        
        OnFlowFinished?.Invoke();
    }

    /// <summary>
    /// 执行指定动作的表演者逻辑
    /// </summary>
    /// <param name="action">要执行表演者逻辑的动作</param>
    /// <returns>协程迭代器</returns>
    private IEnumerator PerformPerformer(GameAction action)
    {
        Type type = action.GetType();
        if (performers.ContainsKey(type))
        {
            yield return performers[type](action);
        }
    }

    /// <summary>
    /// 执行指定类型的订阅者回调
    /// </summary>
    /// <param name="action">触发订阅者的动作</param>
    /// <param name="subs">订阅者字典</param>
    private void PerformSubscribers(GameAction action, Dictionary<Type, List<Action<GameAction>>> subs)
    {
        Type type = action.GetType();
        if (subs.ContainsKey(type))
        {
            foreach (var sub in subs[type])
            {
                sub(action);
            }
        }
    }

    /// <summary>
    /// 执行当前反应列表中的所有反应动作
    /// </summary>
    /// <returns>协程迭代器</returns>
    private IEnumerator PerformReactions()
    {
        foreach (var reaction in reactions)
        {
            yield return Flow(reaction);
        }
    }
    
    /// <summary>
    /// 为指定类型的动作附加表演者函数
    /// </summary>
    /// <typeparam name="T">动作类型</typeparam>
    /// <param name="performer">表演者函数</param>
    public static void AttachPerformer<T>(Func<T, IEnumerator> performer) where T : GameAction
    {
        Type type = typeof(T);
        IEnumerator WrappedPerformer(GameAction action) => performer((T)action);
        if (performers.ContainsKey(type)) performers[type] = WrappedPerformer;
        else performers.Add(type, WrappedPerformer);
    }

    /// <summary>
    /// 移除指定类型动作的表演者函数
    /// </summary>
    /// <typeparam name="T">动作类型</typeparam>
    public static void DetachPerformer<T>() where T : GameAction
    {
        Type type = typeof(T);
        if (performers.ContainsKey(type)) performers.Remove(type);
    }

    /// <summary>
    /// 订阅指定类型动作的反应处理
    /// </summary>
    /// <typeparam name="T">动作类型</typeparam>
    /// <param name="reaction">反应处理函数</param>
    /// <param name="timing">反应时机（预处理或后处理）</param>
    public static void SubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        void WrappedReaction(GameAction action) => reaction((T)action);
        if (subs.ContainsKey(typeof(T)))
        {
            subs[typeof(T)].Add(WrappedReaction);
        }
        else
        {
            subs.Add(typeof(T), new());
            subs[typeof(T)].Add(WrappedReaction);
        }
    }
    
    /// <summary>
    /// 取消订阅指定类型动作的反应处理
    /// </summary>
    /// <typeparam name="T">动作类型</typeparam>
    /// <param name="reaction">反应处理函数</param>
    /// <param name="timing">反应时机（预处理或后处理）</param>
    public static void UnsubscribeReaction<T>(Action<T> reaction, ReactionTiming timing) where T : GameAction
    {
        Dictionary<Type, List<Action<GameAction>>> subs = timing == ReactionTiming.PRE ? preSubs : postSubs;
        if (subs.ContainsKey(typeof(T)))
        {
            void WrappedReaction(GameAction action) => reaction((T)action);
            subs[typeof(T)].Remove(WrappedReaction);
        }
    }
}
