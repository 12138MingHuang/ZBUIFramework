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
    private WindowBase GetWindow(string windowName)
    {
        // 尝试从字典中获取窗口
        this.mAllWindowDic.TryGetValue(windowName, out var window);
        return window;
    }

    /// <summary>
    /// 获取已经弹出的窗口
    /// </summary>
    /// <typeparam name="T">窗口类型，必须继承自 WindowBase。</typeparam>
    /// <returns></returns>
    public T GetWindow<T>() where T: WindowBase
    {
        Type type = typeof(T);
        string windowName = type.Name;
        foreach (var window in this.mVisibleWindowList)
        {
            if(window.Name == windowName)
            {
                return (T)window;
            }
        }
        Debug.LogError($"该窗口没有获取到：{windowName}");
        return null;
    }

    /// <summary>
    /// 隐藏指定名称的窗口。
    /// </summary>
    /// <param name="windowName">窗口名称。</param>
    private void HideWindow(string windowName)
    {
        // 获取指定名称的窗口实例
        WindowBase window = this.GetWindow(windowName);
        // 隐藏窗口
        this.HideWindow(window);
    }

    /// <summary>
    /// 隐藏指定的窗口实例。
    /// </summary>
    /// <param name="window">窗口实例。</param>
    private void HideWindow(WindowBase window)
    {
        // 检查窗口实例是否存在且当前是否可见
        if (window != null && window.Visible)
        {
            // 从可见窗口列表中移除
            this.mVisibleWindowList.Remove(window);
            // 设置窗口不可见
            window.SetVisible(false);
            // 调用窗口的 OnHide 方法
            window.OnHide();
        }
    }

    /// <summary>
    /// 隐藏指定类型的窗口。
    /// </summary>
    /// <typeparam name="T">窗口类型，必须继承自 WindowBase。</typeparam>
    public void HideWindow<T>() where T : WindowBase
    {
        // 获取窗口类型
        Type type = typeof(T);
        // 获取窗口名称
        string windowName = type.Name;
        // 隐藏指定名称的窗口
        this.HideWindow(windowName);
    }

    /// <summary>
    /// 私有方法：销毁指定名称的窗口。
    /// </summary>
    /// <param name="windowName">窗口名称。</param>
    private void DestroyWindow(string windowName)
    {
        WindowBase windowBase = this.GetWindow(windowName);
        this.DestroyWindow(windowBase);
    }

    /// <summary>
    /// 私有方法：销毁指定名称的窗口。
    /// </summary>
    /// <param name="windowName">窗口名称。</param>
    private void DestroyWindow(WindowBase window)
    {
        if (window != null)
        {
            // 如果字典中存在该窗口，移除它
            if (this.mAllWindowDic.ContainsKey(window.Name))
            {
                this.mAllWindowDic.Remove(window.Name);
                this.mAllWindowList.Remove(window);
                this.mVisibleWindowList.Remove(window);
            }
            window.SetVisible(false); // 设置窗口不可见
            window.OnHide(); // 调用窗口的隐藏方法
            window.OnDestroy(); // 调用窗口的销毁方法
            GameObject.Destroy(window.gameObject); // 销毁窗口的游戏对象
        }
    }

    /// <summary>
    /// 公有方法：销毁指定类型的窗口。
    /// </summary>
    /// <typeparam name="T">窗口类型，必须继承自 WindowBase。</typeparam>
    public void DestroyWindow<T>() where T : WindowBase
    {
        Type type = typeof(T);
        string windowName = type.Name;
        this.DestroyWindow(windowName);
    }

    /// <summary>
    /// 公有方法：销毁所有窗口，可选地排除指定的窗口。
    /// </summary>
    /// <param name="filterList">要排除的窗口名称列表（可选）。</param>
    public void DestroyAllWindow(List<string> filterList = null)
    {
        for (int i = this.mAllWindowList.Count - 1; i >= 0; i--)
        {
            WindowBase windowBase = this.mAllWindowList[i];
            // 跳过要过滤的窗口
            if (windowBase == null || (filterList != null && filterList.Contains(windowBase.Name)))
            {
                continue;
            }
            this.DestroyWindow(windowBase.Name);
        }
        Resources.UnloadUnusedAssets(); // 卸载未使用的资源
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
    /// <param name="windowBase">窗口实例。</param>
    /// <param name="windowName">窗口名称。</param>
    /// <returns>返回初始化后的窗口实例。</returns>
    private WindowBase InitializeWindow(WindowBase windowBase, string windowName)
    {
        // 生成对应的窗口预制体
        GameObject goWindow = this.TempLoadWindow(windowName);
        if (goWindow != null)
        {
            windowBase.gameObject = goWindow;
            windowBase.transform = goWindow.transform;
            windowBase.transform.SetAsLastSibling();
            windowBase.Canvas = goWindow.GetComponent<Canvas>();
            windowBase.Canvas.worldCamera = this.mUICamera;
            windowBase.Name = goWindow.name;
            windowBase.OnAwake();
            windowBase.SetVisible(true);
            windowBase.OnShow();

            // 设置窗口的 RectTransform 属性
            RectTransform rectTrans = goWindow.GetComponent<RectTransform>();
            rectTrans.anchorMax = Vector3.one;
            rectTrans.offsetMax = Vector3.zero;
            rectTrans.offsetMin = Vector3.zero;

            // 将窗口添加到管理列表
            this.mAllWindowDic.Add(windowName, windowBase);
            this.mAllWindowList.Add(windowBase);
            this.mVisibleWindowList.Add(windowBase);
            return windowBase;
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
        // 设置窗口的父节点、缩放、位置、旋转和名字
        window.transform.SetParent(this.mUIRoot);
        window.transform.localScale = Vector3.one;
        window.transform.localPosition = Vector3.zero;
        window.transform.localRotation = Quaternion.identity;
        window.name = windowName;

        return window;
    }
}

