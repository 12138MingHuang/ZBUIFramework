using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI 管理器类，负责整个所有面板的管理，并向外部提供一些 API 调用面板
/// </summary>
public class UIModule : Singleton<UIModule>
{
    /// <summary>
    /// UI 摄像机
    /// </summary>
    private Camera mUICamera;

    /// <summary>
    /// UI 根节点
    /// </summary>
    private Transform mUIRoot;

    /// <summary>
    /// 存储所有窗口的字典
    /// </summary>
    private Dictionary<string, WindowBase> mAllWindowDic = new Dictionary<string, WindowBase>();

    /// <summary>
    /// 存储所有窗口的列表
    /// </summary>
    private List<WindowBase> mAllWindowList = new List<WindowBase>();

    /// <summary>
    /// 存储所有可见窗口的列表
    /// </summary>
    private List<WindowBase> mVisibleWindowList = new List<WindowBase>();

    /// <summary>
    /// 初始化 UI 管理器
    /// </summary>
    public void Initialize()
    {
        // 获取 UI 摄像机
        this.mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        // 获取 UI 根节点
        this.mUIRoot = GameObject.Find("UIRoot").transform;
    }

    /// <summary>
    /// 弹出窗口，如果窗口已存在则显示，否则初始化并显示。
    /// </summary>
    /// <typeparam name="T">窗口类型，必须继承自 WindowBase。</typeparam>
    /// <returns>返回弹出的窗口实例。</returns>
    public T PopUpWindow<T>() where T : WindowBase, new()
    {
        // 获取窗口类型
        Type type = typeof(T);
        // 获取窗口名称
        string windowName = type.Name;
        // 获取已存在的窗口
        WindowBase window = this.GetWindow(windowName);

        // 如果窗口已存在，则显示窗口
        if (window != null)
        {
            return this.ShowWindow(windowName) as T;
        }

        // 否则初始化并显示新窗口
        T newWindow = new T();
        return this.InitializeWindow(newWindow, windowName) as T;
    }

    /// <summary>
    /// 获取已存在的窗口。
    /// </summary>
    /// <param name="windowName">窗口名称。</param>
    /// <returns>返回窗口实例，如果不存在则返回 null。</returns>
    public WindowBase GetWindow(string windowName)
    {
        // 尝试从字典中获取窗口
        this.mAllWindowDic.TryGetValue(windowName, out var window);
        return window;
    }

    /// <summary>
    /// 显示指定的窗口。
    /// </summary>
    /// <param name="windowName">窗口名称。</param>
    /// <returns>返回显示的窗口实例。</returns>
    private WindowBase ShowWindow(string windowName)
    {
        // 尝试从字典中获取窗口
        if (this.mAllWindowDic.TryGetValue(windowName, out var window))
        {
            // 如果窗口存在且未显示，则显示窗口
            if (window.gameObject != null && !window.Visible)
            {
                // 添加到所有窗口列表
                this.mAllWindowList.Add(window);
                // 将窗口移动到最上层
                window.transform.SetAsLastSibling();
                // 设置窗口为可见
                window.SetVisible(true);
                // 触发窗口显示事件
                window.OnShow();
            }
            return window;
        }
        else
        {
            Debug.LogError($"{windowName} 窗口不存在，请调用 PopUpWindow 方法进行弹出");
        }
        return null;
    }

    /// <summary>
    /// 初始化窗口。
    /// </summary>
    /// <typeparam name="T">窗口类型，必须继承自 WindowBase。</typeparam>
    /// <param name="window">窗口实例。</param>
    /// <param name="windowName">窗口名称。</param>
    /// <returns>返回初始化后的窗口实例。</returns>
    private T InitializeWindow<T>(T t, string windowName) where T : WindowBase, new()
    {
        // 生成对应的窗口预制体
        GameObject goWindow = this.TempLoadWindow(windowName);
        if (goWindow != null)
        {
            t.gameObject = goWindow;
            t.transform = goWindow.transform;
            t.Canvas = goWindow.GetComponent<Canvas>();
            t.transform.SetAsLastSibling();
            t.OnAwake();
            t.SetVisible(true);
            t.OnShow();

            // 设置窗口的 RectTransform 属性
            RectTransform rectTrans = goWindow.GetComponent<RectTransform>();
            rectTrans.anchorMax = Vector3.one;
            rectTrans.offsetMax = Vector3.zero;
            rectTrans.offsetMin = Vector3.zero;

            // 将窗口添加到管理列表
            this.mAllWindowDic.Add(windowName, t);
            this.mAllWindowList.Add(t);
            this.mVisibleWindowList.Add(t);
            return t;
        }
        Debug.LogError($"没有加载到对应的窗口，窗口名字 {windowName}");
        return null;
    }

    /// <summary>
    /// 加载窗口预制体。
    /// </summary>
    /// <param name="windowName">窗口名称。</param>
    /// <returns>返回加载的窗口预制体实例。</returns>
    private GameObject TempLoadWindow(string windowName)
    {
        // 从资源中加载窗口预制体
        GameObject window = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>($"Window/{windowName}"));
        // 设置窗口的父节点、缩放、位置和旋转
        window.transform.localScale = Vector3.one;
        window.transform.localPosition = Vector3.zero;
        window.transform.localRotation = Quaternion.identity;
        return window;
    }
}

