using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZMUIMain : MonoBehaviour
{
    private void Awake()
    {
        UIModule.Instance.Initialize();
        LoginWindow loginWindow = UIModule.Instance.PopUpWindow<LoginWindow>();
    }
    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    UIModule.Instance.PopUpWindow<AccountWindow>();
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    UIModule.Instance.PopUpWindow<SignUpWindow>();
        //}

        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    UIModule.Instance.HideWindow<AccountWindow>();
        //}

        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    UIModule.Instance.HideWindow<SignUpWindow>();
        //}
    }
}
