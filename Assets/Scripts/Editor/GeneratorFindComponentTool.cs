using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;

public class GeneratorFindComponentTool : Editor
{
    /// <summary>
    /// 对应物体路径字典
    /// key 物体的GUID
    /// value 代表物体的查找路径
    /// </summary>
    public static Dictionary<int, string> objFindPathDic;

    /// <summary>
    /// 查找对象数据
    /// </summary>
    public static List<EditorObjectData> objDataList;

    [MenuItem("GameObject/生成查找组件")]
    private static void CreatFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject; //获取当前选择物体
        if(obj == null )
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }
        objDataList = new List<EditorObjectData>();
        objFindPathDic = new Dictionary<int, string>();

        // 设置脚本生成路径
        if (!Directory.Exists(GeneratorConfig.FindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.FindComponentGeneratorPath);
        }

        PreWindowNodeData(obj.transform, obj.name);
    }

    /// <summary>
    /// 解析窗口节点数据
    /// </summary>
    /// <param name="trans">节点</param>
    /// <param name="WinName">节点名字</param>
    public static void PreWindowNodeData(Transform trans, string winName)
    {
        for(int i = 0; i < trans.childCount; i++)
        {
            GameObject obj = trans.GetChild(i).gameObject;
            string name = obj.name;
            if (name.Contains("[") && name.Contains("]"))
            {
                int index = name.IndexOf("]") + 1;
                string fieldName = name.Substring(index, name.Length - index);
                string fieldType = name.Substring(1, index - 2);
                EditorObjectData objData = new EditorObjectData
                {
                    fieldName = fieldName,
                    fieldType = fieldType,
                    insID = obj.GetInstanceID()
                };
                objDataList.Add(objData);

                //计算该结点的查找路径
                string objPath = name;
                Transform parent = obj.transform;
                while (parent != null && !parent.name.Equals(winName))
                {
                    parent = parent.parent;
                    if (parent != null)
                    {
                        objPath = objPath.Insert(0, parent.name + "/");
                    }
                }
                objFindPathDic.Add(obj.GetInstanceID(), objPath);
            }
            PreWindowNodeData(trans.GetChild(i), winName);
        }
    }
}

public class EditorObjectData
{
    /// <summary>
    /// 物体的GUID
    /// </summary>
    public int insID;

    /// <summary>
    /// 字段名字
    /// </summary>
    public string fieldName;

    /// <summary>
    /// 字段类型
    /// </summary>
    public string fieldType;
}
