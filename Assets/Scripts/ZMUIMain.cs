using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMUIMain : MonoBehaviour
{
    private void Awake()
    {
        LoginWindow loginWindow = UIModule.Instance.PopUpWindow<LoginWindow>();
        loginWindow.Test();

        LoginWindow getWindow = UIModule.Instance.GetWindow<LoginWindow>();
        getWindow.Test2();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIModule.Instance.Initialize();
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
    }
}
