using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMUIMain : MonoBehaviour
{
    private void Awake()
    {
        UIModule.Instance.Initialize();
        LoginWindow loginWindow = UIModule.Instance.PopUpWindow<LoginWindow>();
        loginWindow.Test();

        LoginWindow getWindow = UIModule.Instance.GetWindow<LoginWindow>();
        getWindow.Test2();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIModule.Instance.PopUpWindow<LoginWindow>();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UIModule.Instance.HideWindow<LoginWindow>();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            UIModule.Instance.DestroyWindow<LoginWindow>();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            UIModule.Instance.DestroyAllWindow();
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            Debug.Log("是否启用单遮模式：" + UISetting.Instance.SINGMASK_SYSTEM);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            UIModule.Instance.PopUpWindow<AccountWindow>();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            UIModule.Instance.PopUpWindow<SignUpWindow>();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            UIModule.Instance.HideWindow<AccountWindow>();
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            UIModule.Instance.HideWindow<SignUpWindow>();
        }
    }
}
