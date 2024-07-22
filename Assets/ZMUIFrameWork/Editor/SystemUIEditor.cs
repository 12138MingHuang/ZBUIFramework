using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

/// <summary>
/// SystemUIEditor 编辑器类，用于处理Unity编辑器中特定的UI元素和相机设置
/// </summary>
public class SystemUIEditor : Editor
{
    /// <summary>
    /// 初始化编辑器时调用的方法
    /// </summary>
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        Debug.Log("SystemUIEditor Init");
        //监听heirarchy面板事件，当hierarchy面板发生改变时触发
        EditorApplication.hierarchyChanged += HandleTextOrImageRaycast;
        EditorApplication.hierarchyChanged += LoadWindowCamera;
    }

    /// <summary>
    /// 处理文本或图片组件的射线投射属性
    /// </summary>
    private static void HandleTextOrImageRaycast()
    {
        GameObject obj = Selection.activeGameObject;
        if (obj == null) return;
        if(obj.name.Contains("Text"))
        {
            Text text = obj.GetComponent<Text>();
            if (text != null)
            {
                text.raycastTarget = false;
            }
        }
        else if (obj.name.Contains("Image"))
        {
            Image image = obj.GetComponent<Image>();
            if (image != null)
            {
                image.raycastTarget = false;
            }
            else
            {
                RawImage rawImage = obj.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    rawImage.raycastTarget = false;
                }    
            }
        }
    }

    /// <summary>
    /// 加载窗口相机设置
    /// </summary>
    private static void LoadWindowCamera()
    {
        if (Selection.activeGameObject != null)
        {
            GameObject uiCameraObj = GameObject.Find("UICamera");
            if (uiCameraObj != null)
            {
                Camera camera = uiCameraObj.GetComponent<Camera>();
                if (Selection.activeGameObject.name.Contains("Window"))
                {
                    Canvas canvas = Selection.activeGameObject.GetComponent<Canvas>();
                    if (canvas != null)
                    {
                        canvas.worldCamera = camera;
                    }
                }
            }
        }
    }
}


