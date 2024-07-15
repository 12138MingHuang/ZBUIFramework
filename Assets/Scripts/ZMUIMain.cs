using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMUIMain : MonoBehaviour
{
    private void Awake()
    {
        LoginWindow loginWindow = UIModule.Instance.PopUpWindow<LoginWindow>();
        loginWindow.Test();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            UIModule.Instance.Initialize();
            UIModule.Instance.PopUpWindow<LoginWindow>();
        }
    }
}
