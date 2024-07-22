using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class SystemUIEditor : Editor
{
    [InitializeOnLoadMethod]
    private static void InitEditor()
    {
        Debug.Log("SystemUIEditor Init");
        //监听heirarchy面板事件，当hierarchy面板发生改变时触发
        EditorApplication.hierarchyChanged += HandleTextOrImageRaycast;
    }

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
}
