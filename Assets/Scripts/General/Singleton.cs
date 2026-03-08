using System;
using UnityEngine;

/// <summary>
/// 单例模式基类，用于创建继承自MonoBehaviour的单例对象
/// </summary>
/// <typeparam name="T">继承自MonoBehaviour的类型参数</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    /// <summary>
    /// 获取当前单例实例的静态属性
    /// </summary>
    public static T Instance { get; private set;}

    /// <summary>
    /// Unity生命周期方法，在对象被激活时调用，用于初始化单例实例
    /// </summary>
    protected virtual void Awake()
    {
        // 检查是否已存在实例，如果存在则销毁当前对象
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        // 设置当前对象为单例实例
        Instance = this as T;
    }

    /// <summary>
    /// Unity生命周期方法，在应用程序退出时调用，用于清理单例实例
    /// </summary>
    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// 持久化单例模式基类，继承自Singleton，用于创建在场景切换时不被销毁的单例对象
/// </summary>
/// <typeparam name="T">继承自MonoBehaviour的类型参数</typeparam>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    /// <summary>
    /// 重写的Awake方法，在父类初始化完成后设置对象在场景加载时不被销毁
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
        // 设置游戏对象在场景切换时保持存在
        DontDestroyOnLoad(gameObject);
    }
}