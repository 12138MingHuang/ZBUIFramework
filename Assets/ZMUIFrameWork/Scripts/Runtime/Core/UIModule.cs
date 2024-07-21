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
    /// 窗口配置表
    /// </summary>
    private WindowConfig mwindowConfig;
    
    /// <summary>
    /// 窗口堆栈，用于窗口的层级管理,弹窗的循环弹出
    /// </summary>
    private Queue<WindowBase> mWindowStack = new Queue<WindowBase>();

    /// <summary>
    /// 开始弹出堆栈的标志，可以用来处理多种情况，比如：正在出栈中有其他界面弹出，可以直接放到栈内进行弹出
    /// </summary>
    private bool mStartPopStackWindowStatus = false;

    /// <summary>
    /// 初始化 UI 管理器
    /// </summary>
    public void Initialize()
    {
        // 获取 UI 摄像机
        this.mUICamera = GameObject.Find("UICamera").GetComponent<Camera>();
        // 获取 UI 根节点
        this.mUIRoot = GameObject.Find("UIRoot").transform;
        // 加载窗口配置表
        this.mwindowConfig = Resources.Load<WindowConfig>("WindowConfig");

        // 只在编辑器环境下
#if UNITY_EDITOR
        this.mwindowConfig.GenerateWindowConfig();
#endif
    }
    
    #region 窗口管理

    /// <summary>
    /// 预加载接口，只加载物体，不调用生命周期
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public void PreLoadWindow<T>() where T : WindowBase, new()
    {
        // 获取窗口类型
        Type type = typeof(T);
        // 获取窗口名称
        string windowName = type.Name;
        T newWindow = new T();
        
        // 生成对应的窗口预制体
        GameObject goWindow = this.TempLoadWindow(windowName);
        if (goWindow != null)
        {
            newWindow.gameObject = goWindow;
            newWindow.transform = goWindow.transform;
            newWindow.transform.SetAsLastSibling();
            newWindow.Canvas = goWindow.GetComponent<Canvas>();
            newWindow.Canvas.worldCamera = this.mUICamera;
            newWindow.Name = goWindow.name;
            newWindow.OnAwake();
            newWindow.SetVisible(false);
            // 设置窗口的 RectTransform 属性
            RectTransform rectTrans = goWindow.GetComponent<RectTransform>();
            rectTrans.anchorMax = Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin = Vector2.zero;

            // 将窗口添加到管理列表
            this.mAllWindowDic.Add(windowName, newWindow);
            this.mAllWindowList.Add(newWindow);
        }
        Debug.Log("预加载窗口：" + windowName);
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
    /// 弹出窗口并返回窗口实例。
    /// 如果窗口已存在，则显示已存在的窗口；否则，初始化并显示新窗口。
    /// </summary>
    /// <param name="window">需要弹出的窗口对象。</param>
    /// <returns>返回弹出的窗口实例。</returns>
    private WindowBase PopUpWindow(WindowBase window)
    {
        // 获取窗口类型
        Type type = window.GetType();
        // 获取窗口名称
        string windowName = type.Name;
        // 获取已存在的窗口
        WindowBase windowBase = this.GetWindow(windowName);

        // 如果窗口已存在，则显示窗口
        if (windowBase != null)
        {
            return this.ShowWindow(windowName);
        }
        
        return this.InitializeWindow(window, windowName);
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
    public void HideWindow(string windowName)
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
            this.SetWindowMaskVisiable();
            // 调用窗口的 OnHide 方法
            window.OnHide();
        }
        //在出栈的情况下，上一个界面隐藏时，自动打开栈中的下一个界面，内部已经判断了，是否属于堆栈系统的
        this.PopNextStackWindow(window);
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
            if (this.mAllWindowDic.TryGetValue(window.Name, out var windowInstance))
            {
                this.mAllWindowDic.Remove(window.Name);
                this.mAllWindowList.Remove(window);
                this.mVisibleWindowList.Remove(window);
            }
            window.SetVisible(false); // 设置窗口不可见
            this.SetWindowMaskVisiable();
            window.OnHide(); // 调用窗口的隐藏方法
            window.OnDestroy(); // 调用窗口的销毁方法
            GameObject.Destroy(window.gameObject); // 销毁窗口的游戏对象

            // 清理引用，帮助垃圾回收
            window = null;
            System.GC.Collect();
            //在出栈的情况下，上一个界面销毁时，自动打开栈中的下一个界面，内部已经判断了，是否属于堆栈系统的
            this.PopNextStackWindow(window);
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
                // 添加到可见窗口列表
                this.mVisibleWindowList.Add(window);
                // 将窗口移动到最上层
                window.transform.SetAsLastSibling();
                // 设置窗口为可见
                window.SetVisible(true);
                // 新增遮罩层设置
                this.SetWindowMaskVisiable();
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
            rectTrans.anchorMax = Vector2.one;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.offsetMin = Vector2.zero;

            // 将窗口添加到管理列表
            this.mAllWindowDic.Add(windowName, windowBase);
            this.mAllWindowList.Add(windowBase);
            this.mVisibleWindowList.Add(windowBase);

            this.SetWindowMaskVisiable();
            return windowBase;
        }
        Debug.LogError($"没有加载到对应的窗口，窗口名字 {windowName}");
        return null;
    }

    /// <summary>
    /// 设置窗口遮罩的可见性
    /// </summary>
    private void SetWindowMaskVisiable()
    {
        if(!UISetting.Instance.SINGMASK_SYSTEM)
        {
            return;
        }
        WindowBase maxOrderWindowBase = null; //最大渲染层级的窗口
        int maxOrder = 0; //最大渲染层级
        int maxIndex = 0; //最大排序下标，在相同父节点下的位置下标
        //1.关闭所有窗口的Mask 设置不可见
        //2.从所有窗口中找到一个层级最大的窗口，把Mask设置为可见
        for (int i = 0; i < this.mVisibleWindowList.Count; i++)
        {
            WindowBase window = this.mVisibleWindowList[i];
            if (window != null && window.gameObject != null)
            {
                window.SetMaskVisiable(false);
                if(maxOrderWindowBase == null)
                {
                    maxOrderWindowBase = window;
                    maxOrder = window.Canvas.sortingOrder;
                    maxIndex = window.transform.GetSiblingIndex();
                }
                else
                {
                    //找到最大渲染层级，拿到它
                    if(window.Canvas.sortingOrder > maxOrder)
                    {
                        maxOrderWindowBase = window;
                        maxOrder = window.Canvas.sortingOrder;
                    }
                    else if(window.Canvas.sortingOrder == maxOrder && maxIndex < window.transform.GetSiblingIndex())
                    {
                        maxOrderWindowBase = window;
                        maxIndex = window.transform.GetSiblingIndex();
                    }
                }
            }
        }
        if (maxOrderWindowBase != null)
        {
            maxOrderWindowBase.SetMaskVisiable(true);
        }
    }

    /// <summary>
    /// 加载窗口预制体。
    /// </summary>
    /// <param name="windowName">窗口名称。</param>
    /// <returns>返回加载的窗口预制体实例。</returns>
    private GameObject TempLoadWindow(string windowName)
    {
        // 从资源中加载窗口预制体
        GameObject window = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>(this.mwindowConfig.GetWindowPath(windowName)), this.mUIRoot);
        // 设置窗口的父节点、缩放、位置、旋转和名字
        //window.transform.SetParent(this.mUIRoot); //先实例化预制体再设置父节点时，Unity可能会在设置父节点之前对预制体进行一些默认的初始化操作，这可能会导致预制体在层级顺序上出现问题。直接在实例化时设置父节点可以确保预制体从一开始就处于正确的层级结构中。
        window.transform.localScale = Vector3.one;
        window.transform.localPosition = Vector3.zero;
        window.transform.localRotation = Quaternion.identity;
        window.name = windowName;

        return window;
    }
    
    #endregion
    
    #region 堆栈系统

    /// <summary>
    /// 将一个界面实例压入窗口堆栈中
    /// </summary>
    /// <typeparam name="T">窗口类型，必须继承自WindowBase，且支持无参数实例化</typeparam>
    /// <param name="popCallBack">当界面从堆栈中弹出时执行的回调函数，可以为空</param>
    public void PushWindowToStack<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
    {
        T windowBase = new T();
        windowBase.PopStackListener = popCallBack;
        this.mWindowStack.Enqueue(windowBase);
    }

    /// <summary>
    /// 弹出栈顶的窗口
    /// </summary>
    /// <remarks>
    /// 此方法用于开始弹出栈顶的窗口。如果栈顶的窗口弹出流程已经开始，则不会执行任何操作。
    /// </remarks>
    public void StartPopFirstStackWindow()
    {
        if(this.mStartPopStackWindowStatus) return;
        this.mStartPopStackWindowStatus = true; // 已经开始进行堆栈窗口弹出的流程了
        this.PopStackWindow();
    }
    
    /// <summary>
    /// 压入并立即弹出堆栈中的弹窗
    /// </summary>
    /// <typeparam name="T">弹窗类型，必须继承自 WindowBase，且有无参数的构造函数</typeparam>
    /// <param name="popCallBack">弹窗弹出后的回调函数，传入参数为弹出的 WindowBase 类型的弹窗实例（可选）</param>
    public void PushAndPopStackWindow<T>(Action<WindowBase> popCallBack = null) where T : WindowBase, new()
    {
        this.PushWindowToStack<T>(popCallBack);
        this.StartPopFirstStackWindow();
    }
    
    /// <summary>
    /// 弹出下一个栈中的窗口
    /// </summary>
    /// <param name="windowBase">要弹出的窗口对象</param>
    private void PopNextStackWindow(WindowBase windowBase)
    {
        if (windowBase != null && this.mStartPopStackWindowStatus && windowBase.PopStack)
        {
            windowBase.PopStack = false;
            this.PopStackWindow();
        }
    }
    
    /// <summary>
    /// 弹出栈顶的窗口，并触发其弹出时的回调函数（如果已设置）。
    /// </summary>
    /// <returns>如果成功弹出窗口，则返回true；否则返回false。</returns>
    public bool PopStackWindow()
    {
        // 检查窗口堆栈是否为空
        if (this.mWindowStack.Count <= 0)
        {
            // 如果为空，则标记起始弹出窗口状态为false
            this.mStartPopStackWindowStatus = false;
            // 未能弹出窗口，返回false
            return false;
        }

        // 从堆栈中移除并获取栈顶的窗口
        WindowBase window = this.mWindowStack.Dequeue();

        // 弹出（或显示）栈顶的窗口，并返回该窗口的实例
        WindowBase topWindow = this.PopUpWindow(window);

        // 如果之前的窗口有PopStackListener设置，则将其复制到新弹出的窗口
        // 这样新弹出的窗口在弹出时也会触发相同的回调函数
        topWindow.PopStackListener = window.PopStackListener;

        // 设置新弹出的窗口的PopStack属性为true，可能用于标识窗口状态或触发某些逻辑
        topWindow.PopStack = true;

        // 如果新弹出的窗口有PopStackListener设置，则执行其回调函数
        // 传递新弹出的窗口实例作为参数
        topWindow.PopStackListener?.Invoke(topWindow);

        // 在执行完回调函数后，清空新弹出的窗口的PopStackListener
        // 可能是为了避免重复触发或释放资源
        topWindow.PopStackListener = null;

        // 成功弹出窗口，返回true
        return true;
    }
    
    /// <summary>
    /// 清空窗口堆栈
    /// </summary>
    public void ClearStackWindows()
    {
        this.mWindowStack.Clear();
        // 标记起始弹出窗口状态为false
        this.mStartPopStackWindowStatus = false;
    }
    
    #endregion
}