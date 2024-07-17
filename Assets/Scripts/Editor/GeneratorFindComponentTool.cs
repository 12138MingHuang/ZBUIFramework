using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using System.IO;
using System;
using System.Text;
using UnityEngine.Events;
using Unity.Plastic.Newtonsoft.Json;

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

    [MenuItem("GameObject/生成查找组件", false, 0)]
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
        // 解析窗口节点数据
        PreWindowNodeData(obj.transform, obj.name);

        //存储字段名称
        string datalistJson = JsonConvert.SerializeObject(objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, datalistJson);

        //foreach (var item in objFindPathDic)
        //{
        //    Debug.Log("物体GUID：" + item.Key);
        //    Debug.Log("物体的查找路径：" + item.Value);
        //}

        //foreach (var item in objDataList)
        //{
        //    Debug.Log("组件物体GUID：" + item.insID);
        //    Debug.Log("组件物体字段名：" + item.fieldName);
        //    Debug.Log("组件物体字段类型：" + item.fieldType);
        //}

        // 生成CS脚本文件
        string csCotent = CreatCS(obj.name);
        Debug.Log("CS脚本生成成功：\n" + csCotent);
        string csPath = $"{GeneratorConfig.FindComponentGeneratorPath}/{obj.name}UIComponent.cs";

        UIWindowEditor.ShowWindow(csCotent, csPath);

        ////生成脚本文件
        //if (File.Exists(csPath))
        //{
        //    File.Delete(csPath);
        //}
        //StreamWriter writer = File.CreateText(csPath);
        //writer.Write(csCotent);
        //writer.Close();
        //AssetDatabase.Refresh();
        //Debug.Log("csPath:" + csPath);
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
                string objPath = name;  // UIContent/[Button]Close
                Transform parent = obj.transform;
                while (parent != null && !parent.name.Equals(winName))
                {
                    parent = parent.parent;
                    if (parent != null && !parent.name.Equals(winName))
                    {
                        objPath = objPath.Insert(0, parent.name + "/");
                    }
                }

                objFindPathDic.Add(obj.GetInstanceID(), objPath);
            }
            PreWindowNodeData(trans.GetChild(i), winName);
        }
    }

    /// <summary>
    /// 生成CS脚本文件
    /// </summary>
    /// <param name="name">解析到的字符串</param>
    private static string CreatCS(string name)
    {
        StringBuilder sb = new StringBuilder();
        string nameSpaceName = "ZMUIFrameWork";
        //添加引用，注释，引入命名空间
        sb.AppendLine("/*");
        sb.AppendLine(" *-------------------------");
        sb.AppendLine(" *Title:UI自动化组件查找代码工具");
        sb.AppendLine(" *Author:ZHANGBIN");
        sb.AppendLine(" *Date:" + DateTime.Now.ToString());
        sb.AppendLine(" *Description:需要查找的变量要以[变量类型]变量名的形式命名，例如[Text]textName,然后右键窗口UI物体根结点，一键生成UI逐渐查找脚本即可");
        sb.AppendLine(" *注意：以下文件是自动生成的，任何手动修改都会被下次生成覆盖，若手动修改，尽量避免自动生成");
        sb.AppendLine(" *--------------------------");
        sb.AppendLine(" */");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEngine.UI;");

        sb.AppendLine();

        //生成命名空间
        if(!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }

        //生成类名
        sb.AppendLine($"\tpublic class {name}UIComponent");
        sb.AppendLine("\t{");

        //根据字段类型列表 声明字段
        foreach (var item in objDataList)
        {
            sb.AppendLine($"\t\tpublic {item.fieldType} {item.fieldName}{item.fieldType};\n");
        }

        //声明初始化接口
        sb.AppendLine("\t\tpublic void InitComponent(WindowBase target)");
        sb.AppendLine("\t\t{");
        sb.AppendLine("\t\t\t//组件查找");

        //根据查找路径字典和字段数据列表生成组件查找代码
        foreach (var item in objFindPathDic)
        {
            EditorObjectData itemData = GetEditorObjectData(item.Key);
            string reFieldName = itemData.fieldName + itemData.fieldType;

            if (string.Equals("GameObject", itemData.fieldType))
            {
                sb.AppendLine($"\t\t\t{reFieldName} = target.transform.Find(\"{item.Value}\").gameObject;");
            }
            else if(string.Equals("Transform", itemData.fieldType))
            {
                sb.AppendLine($"\t\t\t{reFieldName} = target.transform.Find(\"{item.Value}\").transform;");
            }
            else
            {
                sb.AppendLine($"\t\t\t{reFieldName} = target.transform.Find(\"{item.Value}\").GetComponent<{itemData.fieldType}>();");
            }
        }
        sb.AppendLine("\t\t\t");
        sb.AppendLine("\t\t\t");
        sb.AppendLine("\t\t\t//组件事件绑定");

        //得到逻辑类WindowBase => LoginWindow
        sb.AppendLine($"\t\t\t{name} mWindow = ({name})target;");

        //生成UI事件绑定代码
        foreach (var item in objDataList)
        {
            string type = item.fieldType;
            string fieldName = item.fieldName;
            string suffix = "";
            if (type.Contains("Button"))
            {
                suffix = "Click";
                sb.AppendLine($"\t\t\ttarget.AddButtonClickListener({fieldName}{type}, mWindow.On{fieldName}Button{suffix});");
            }
            if (type.Contains("InputField"))
            {
                sb.AppendLine($"\t\t\ttarget.AddInputFieldListener({fieldName}{type}, mWindow.On{fieldName}InputChange, mWindow.On{fieldName}InputEnd);");
            }
            if (type.Contains("Toggle"))
            {
                suffix = "Change";
                sb.AppendLine($"\t\t\ttarget.AddToggleClickListener({fieldName}{type}, mWindow.On{fieldName}Toggle{suffix});");
            }
        }
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t}");
        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine("}");
        }
        return sb.ToString();
    }

    public static EditorObjectData GetEditorObjectData(int insID)
    {
        foreach (var item in objDataList)
        {
            if (item.insID == insID)
            {
                return item;
            }
        }
        return null;
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

