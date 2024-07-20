using System;
using UnityEngine;

/// <summary>
/// 与MonoBehaviour类似是所有界面都必须继承的最基础最顶层的类
/// 界面管理基类，所有界面必须继承自此类。
/// 负责管理界面的生命周期和基础属性。
/// </summary>
public abstract class WindowBehaviour
{
    /// <summary>
    /// 当前窗口的 GameObject。
    /// </summary>
    public GameObject gameObject { get; set; }

    /// <summary>
    /// 当前窗口的 Transform，代表 UI 自身。
    /// </summary>
    public Transform transform { get; set; }

    /// <summary>
    /// 窗口所在的 Canvas。
    /// </summary>
    public Canvas Canvas { get; set; }

    /// <summary>
    /// 窗口的名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 窗口是否可见。
    /// </summary>
    public bool Visible { get; protected set; }

    /// <summary>
    /// 堆栈弹出弹出回调
    /// </summary>
    public Action<WindowBase> PopStackListener { get; set; }

    /// <summary>
    /// 是否通过堆栈系统弹出的弹窗
    /// </summary>
    public bool PopStack { get; set; }

    /// <summary>
    /// 当 GameObject 创建时调用，仅执行一次。
    /// 类似于 MonoBehaviour 的 Awake 方法。
    /// </summary>
    public virtual void OnAwake() { }

    /// <summary>
    /// 当界面显示时调用一次。
    /// 类似于 MonoBehaviour 的 Enable 方法。
    /// </summary>
    public virtual void OnShow() { }

    /// <summary>
    /// 每帧更新调用。
    /// </summary>
    public virtual void OnUpdate() { }

    /// <summary>
    /// 当界面隐藏时调用一次。
    /// 类似于 MonoBehaviour 的 Disable 方法。
    /// </summary>
    public virtual void OnHide() { }

    /// <summary>
    /// 当界面销毁时调用一次。
    /// 类似于 MonoBehaviour 的 OnDestroy 方法。
    /// </summary>
    public virtual void OnDestroy() { }

    /// <summary>
    /// 设置界面的可见性。
    /// </summary>
    /// <param name="isVisible">是否可见。</param>
    public virtual void SetVisible(bool isVisible) { }
}
