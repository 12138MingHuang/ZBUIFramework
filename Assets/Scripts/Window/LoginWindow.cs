using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginWindow : WindowBase
{
    public override void OnAwake()
    {
        base.OnAwake();
        Debug.Log("LoginWindow OnAwake");
    }

    public override void OnShow()
    {
        base.OnShow();
        Debug.Log("LoginWindow OnShow");
    }

    public void Test()
    {
        Debug.Log("测试-------");
    }
}
