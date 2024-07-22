/*
 *-------------------------
 *Title:UI表现层脚本自动生成工具
 *Author:ZHANGBIN
 *Date:2024/7/20 15:01:48
 *Description:改脚本只负责UI界面的交互，表现上的更新，不建议在此填写业务层的相关逻辑
 *注意：以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上新增
 *--------------------------
 */
using UnityEngine;
using UnityEngine.UI;
using ZMUIFrameWork;

public class ChatWindow : WindowBase
{
	public ChatWindowDataComponent dataCompt = null;

	#region 生命周期函数
	//调用机制与Mono Awake一致
	public override void OnAwake()
	{
		base.OnAwake();
		dataCompt = gameObject.GetComponent<ChatWindowDataComponent>();
		dataCompt.InitComponent(this);
	}
	//当界面显示时调用。
	public override void OnShow()
	{
		base.OnShow();
	}
	//当界面隐藏时调用。
	public override void OnHide()
	{
		base.OnHide();
	}
	//当界面销毁时调用。
	public override void OnDestroy()
	{
		base.OnDestroy();
	}
	#endregion

	#region API Function

	#endregion

	#region UI组件生成事件
	public void OnCloseButtonClick()
	{

		HideWindow();
	}

	#endregion
}
