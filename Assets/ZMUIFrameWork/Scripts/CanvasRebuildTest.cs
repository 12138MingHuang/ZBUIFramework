using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class CanvasRebuildTest : MonoBehaviour
{
    private IList<ICanvasElement> mLayoutRebuildQueue;
    IList<ICanvasElement> mGraphicRebuildQueue;

    void Start()
    {
        Type type = typeof(CanvasUpdateRegistry);
        FieldInfo fieldInfo = type.GetField("m_LayoutRebuildQueue", BindingFlags.Instance | BindingFlags.NonPublic);
        this.mLayoutRebuildQueue = (IList<ICanvasElement>)fieldInfo.GetValue(CanvasUpdateRegistry.instance);
        
        fieldInfo = type.GetField("m_GraphicRebuildQueue", BindingFlags.Instance | BindingFlags.NonPublic);
        this.mGraphicRebuildQueue = (IList<ICanvasElement>)fieldInfo.GetValue(CanvasUpdateRegistry.instance);
    }

    void Update()
    {
        for(int i=0;i<this.mLayoutRebuildQueue.Count;i++)
        {
            var rebuild = this.mLayoutRebuildQueue[i];
            if (!ObjectValidForUpdate(rebuild))
                continue;
            Debug.LogFormat("{0} 引起 {1} 网格重建", rebuild.transform.name, rebuild.transform.GetComponent<Graphic>().canvas.name);
        }

        for (int i = 0; i < this.mGraphicRebuildQueue.Count; i++)
        {
            var rebuild = this.mGraphicRebuildQueue[i];
            if (!ObjectValidForUpdate(rebuild))
                    continue;
            Debug.LogFormat("{0} 引起 {1} 网格重建", rebuild.transform.name, rebuild.transform.GetComponent<Graphic>().canvas.name);
        }
    }

    private bool ObjectValidForUpdate(ICanvasElement element)
    {
        var vaild = element != null;
        var isUnityObject = element is UnityEngine.Object;
        if (isUnityObject)
        {
            vaild = (element as UnityEngine.Object) != null;
        }
        return vaild;
    }
}
