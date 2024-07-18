using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "UISetting", menuName = "UISetting", order = 0)]
public class UISetting : ScriptableObject
{
    private static UISetting instance;
    public static UISetting Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<UISetting>("UISetting");
            }
            return instance;
        }
    }

    [Space]
    [Header("遮罩系统设置")]
    [Tooltip("启用单遮罩模式")]
    public bool SINGMASK_SYSTEM;

}
