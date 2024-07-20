using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// UI窗口基类，扩展自 WindowBehaviour，负责部分共用功能的统一化处理。
/// </summary>
public class WindowBase : WindowBehaviour
{
    /// <summary>
    /// 所有注册了按钮点击事件的按钮列表。
    /// </summary>
    private List<Button> mAllButtonList = new List<Button>();

    /// <summary>
    /// 所有注册了开关状态变化事件的开关列表。
    /// </summary>
    private List<Toggle> mAllToggleList = new List<Toggle>();

    /// <summary>
    /// 所有注册了输入框内容变化和结束编辑事件的输入框列表。
    /// </summary>
    private List<InputField> mAllInputFieldList = new List<InputField>();

    /// <summary>
    /// 遮罩节点
    /// </summary>
    private CanvasGroup mUIMask;
    
    /// <summary>
    /// 界面内容节点
    /// </summary>
    protected Transform mUIContent;
    
    /// <summary>
    /// 是否禁用动画。
    /// </summary>
    protected bool mDisableAnimation = false;


    /// <summary>
    /// 初始化基类组件
    /// </summary>
    private void InitializeBaseComponent()
    {
        this.mUIMask = transform.Find("UIMask").GetComponent<CanvasGroup>();
        this.mUIContent = transform.Find("UIContent").transform;
    }

    #region 生命周期函数
    /// <summary>
    /// 当界面对象创建时调用。
    /// </summary>
    public override void OnAwake()
    {
        base.OnAwake();
        // 在此可以进行界面对象的初始化操作
        this.InitializeBaseComponent();
    }

    /// <summary>
    /// 当界面显示时调用。
    /// </summary>
    public override void OnShow()
    {
        base.OnShow();
        // 在此处理界面显示时的逻辑
        this.ShowAnimation();
    }

    /// <summary>
    /// 每帧更新调用。
    /// </summary>
    public override void OnUpdate()
    {
        base.OnUpdate();
        // 在此可以添加界面每帧更新的逻辑
    }

    /// <summary>
    /// 当界面隐藏时调用。
    /// </summary>
    public override void OnHide()
    {
        base.OnHide();
        // 在此处理界面隐藏时的逻辑
    }

    /// <summary>
    /// 当界面销毁时调用。
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();

        // 移除所有事件监听器并清空列表
        this.RemoveAllButtonListener();
        this.RemoveAllToggleListener();
        this.RemoveAllInputFieldListener();

        this.mAllButtonList.Clear();
        this.mAllToggleList.Clear();
        this.mAllInputFieldList.Clear();
    }

    public void HideWindow()
    {
        // UIModule.Instance.HideWindow(Name);
        this.HideAnimation();
    }

    /// <summary>
    /// 设置界面的可见性。
    /// </summary>
    /// <param name="isVisible">是否可见。</param>
    public override void SetVisible(bool isVisible)
    {
        base.SetVisible(isVisible);
        // FIXME 临时代码
        this.gameObject.SetActive(isVisible);
        this.Visible = isVisible;
    }

    /// <summary>
    /// 设置遮罩的可见性
    /// </summary>
    /// <param name="isVisiable">是否可见</param>
    public void SetMaskVisiable(bool isVisiable)
    {
        if (!UISetting.Instance.SINGMASK_SYSTEM)
        {
            return;
        }
        this.mUIMask.alpha = isVisiable ? 1 : 0;
    }
    #endregion

    #region 事件管理
    /// <summary>
    /// 添加按钮点击监听器。
    /// </summary>
    /// <param name="btn">要添加监听器的按钮。</param>
    /// <param name="action">点击按钮时触发的动作。</param>
    public void AddButtonClickListener(Button btn, UnityAction action)
    {
        if (btn != null)
        {
            if (!this.mAllButtonList.Contains(btn))
            {
                this.mAllButtonList.Add(btn);
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(action);
        }
    }

    /// <summary>
    /// 添加开关状态变化监听器。
    /// </summary>
    /// <param name="toggle">要添加监听器的开关。</param>
    /// <param name="action">状态变化时触发的动作。</param>
    public void AddToggleClickListener(Toggle toggle, UnityAction<bool, Toggle> action)
    {
        if (toggle != null)
        {
            if (!this.mAllToggleList.Contains(toggle))
            {
                this.mAllToggleList.Add(toggle);
            }
            toggle.onValueChanged.RemoveAllListeners();
            toggle.onValueChanged.AddListener((isOn) =>
            {
                action?.Invoke(isOn, toggle);
            });
        }
    }

    /// <summary>
    /// 添加输入框内容变化和结束编辑监听器。
    /// </summary>
    /// <param name="input">要添加监听器的输入框。</param>
    /// <param name="onChangeAction">内容变化时触发的动作。</param>
    /// <param name="endAction">结束编辑时触发的动作。</param>
    public void AddInputFieldListener(InputField input, UnityAction<string> onChangeAction, UnityAction<string> endAction)
    {
        if (input != null)
        {
            if (!this.mAllInputFieldList.Contains(input))
            {
                this.mAllInputFieldList.Add(input);
            }
            input.onValueChanged.RemoveAllListeners();
            input.onEndEdit.RemoveAllListeners();
            input.onValueChanged.AddListener(onChangeAction);
            input.onEndEdit.AddListener(endAction);
        }
    }

    /// <summary>
    /// 移除所有按钮点击监听器。
    /// </summary>
    public void RemoveAllButtonListener()
    {
        foreach (var item in mAllButtonList)
        {
            item.onClick.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 移除所有开关状态变化监听器。
    /// </summary>
    public void RemoveAllToggleListener()
    {
        foreach (var item in mAllToggleList)
        {
            item.onValueChanged.RemoveAllListeners();
        }
    }

    /// <summary>
    /// 移除所有输入框内容变化和结束编辑监听器。
    /// </summary>
    public void RemoveAllInputFieldListener()
    {
        foreach (var item in mAllInputFieldList)
        {
            item.onValueChanged.RemoveAllListeners();
            item.onEndEdit.RemoveAllListeners();
        }
    }
    #endregion
    
    #region 动画管理

    public void ShowAnimation()
    {
        //基础UI不需要动画（层级是否>99,这里默认基础弹窗0-99层级）
        if (Canvas.sortingOrder > 99 && this.mDisableAnimation == false)
        {
            //mask动画（需求而定）
            this.mUIMask.alpha = 0;
            this.mUIMask.DOFade(1, .2f);//这里会引发一个堆栈弹出表现上的问题，会感觉闪烁一下，其实可以，判断是不是堆栈弹窗，是就不播
            
            //缩放动画
            this.mUIContent.localScale = Vector3.one* .7f;
            this.mUIContent.DOScale(Vector3.one, .4f).SetEase(Ease.OutBack);
        }
    }

    public void HideAnimation()
    {
        if (Canvas.sortingOrder > 99 && this.mDisableAnimation == false)
        {
            this.mUIContent
                .DOScale(Vector3.one * 1.1f, .2f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    UIModule.Instance.HideWindow(Name);
                });
        }
        else
        {
            UIModule.Instance.HideWindow(Name);
        }
    }

    #endregion
}
