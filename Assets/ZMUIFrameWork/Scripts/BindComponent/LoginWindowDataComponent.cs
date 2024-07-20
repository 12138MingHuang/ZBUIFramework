/*
 *-------------------------
 *Title:UI自动化组件生成代码工具
 *Author:ZHANGBIN
 *Date:2024/7/20 14:47:08
 *Description:需要查找的变量要以[变量类型]变量名的形式命名，例如[Text]textName,然后右键窗口UI物体根结点，一键生成UI数据组件脚本即可
 *注意：以下文件是自动生成的，任何手动修改都会被下次生成覆盖，若手动修改，尽量避免自动生成
 *--------------------------
 */
using UnityEngine;
using UnityEngine.UI;

namespace ZMUIFrameWork
{
	public class LoginWindowDataComponent : MonoBehaviour
	{
		public Button LoginButton;

		public void InitComponent(WindowBase target)
		{
			//组件事件绑定
			LoginWindow mWindow = (LoginWindow)target;
			target.AddButtonClickListener(LoginButton, mWindow.OnLoginButtonClick);
		}
	}
}
