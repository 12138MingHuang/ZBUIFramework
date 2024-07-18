using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

public class GeneratorBindComponentTool : Editor
{

    /// <summary>
    ///     查找对象数据
    /// </summary>
    public static List<EditorObjectData> objDataList;

    [MenuItem("GameObject/生成绑定组件脚本", false, 2)]
    private static void CreatFindComponentScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject; //获取当前选择物体
        if (obj == null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }
        objDataList = new List<EditorObjectData>();

        // 设置脚本生成路径
        if (!Directory.Exists(GeneratorConfig.BindComponentGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.BindComponentGeneratorPath);
        }
        // 解析窗口节点数据
        PreWindowNodeData(obj.transform, obj.name);

        //存储字段名称
        string datalistJson = JsonConvert.SerializeObject(objDataList);
        PlayerPrefs.SetString(GeneratorConfig.OBJDATALIST_KEY, datalistJson);

        // 生成CS脚本文件
        string csCotent = CreatCS(obj.name);
        Debug.Log("CS脚本生成成功：\n" + csCotent);
        string csPath = $"{GeneratorConfig.BindComponentGeneratorPath}/{obj.name}DataComponent.cs";

        UIWindowEditor.ShowWindow(csCotent, csPath);
        EditorPrefs.SetString(GeneratorConfig.GENERATOR_CLASS_NAME, obj.name + "DataComponent");
    }

    /// <summary>
    ///     解析窗口节点数据
    /// </summary>
    /// <param name="trans">节点</param>
    /// <param name="winName">节点名字</param>
    public static void PreWindowNodeData(Transform trans, string winName)
    {
        for (int i = 0; i < trans.childCount; i++)
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
            }
            PreWindowNodeData(trans.GetChild(i), winName);
        }
    }

    /// <summary>
    ///     生成CS脚本文件
    /// </summary>
    /// <param name="name">解析到的字符串</param>
    private static string CreatCS(string name)
    {
        StringBuilder sb = new StringBuilder();
        string nameSpaceName = "ZMUIFrameWork";
        //添加引用，注释，引入命名空间
        sb.AppendLine("/*");
        sb.AppendLine(" *-------------------------");
        sb.AppendLine(" *Title:UI自动化组件生成代码工具");
        sb.AppendLine(" *Author:ZHANGBIN");
        sb.AppendLine(" *Date:" + DateTime.Now);
        sb.AppendLine(" *Description:需要查找的变量要以[变量类型]变量名的形式命名，例如[Text]textName,然后右键窗口UI物体根结点，一键生成UI数据组件脚本即可");
        sb.AppendLine(" *注意：以下文件是自动生成的，任何手动修改都会被下次生成覆盖，若手动修改，尽量避免自动生成");
        sb.AppendLine(" *--------------------------");
        sb.AppendLine(" */");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEngine.UI;");

        sb.AppendLine();

        //生成命名空间
        if (!string.IsNullOrEmpty(nameSpaceName))
        {
            sb.AppendLine($"namespace {nameSpaceName}");
            sb.AppendLine("{");
        }

        //生成类名
        sb.AppendLine($"\tpublic class {name}DataComponent : MonoBehaviour");
        sb.AppendLine("\t{");

        //根据字段类型列表 声明字段
        foreach (var item in objDataList)
        {
            sb.AppendLine($"\t\tpublic {item.fieldType} {item.fieldName}{item.fieldType};\n");
        }

        //声明初始化接口
        sb.AppendLine("\t\tpublic void InitComponent(WindowBase target)");
        sb.AppendLine("\t\t{");
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

    /// <summary>
    /// 往窗口物体自动添加组件，脚本编译好后系统自动调用
    /// </summary>
    [UnityEditor.Callbacks.DidReloadScripts]
    public static void AddComponent2Window()
    {
        //如果当前不是生成数据脚本的回调，则不执行
        string className = EditorPrefs.GetString(GeneratorConfig.GENERATOR_CLASS_NAME);
        if(string.IsNullOrEmpty(className))
        {
            return;
        }
        //1.通过反射的方式，从程序集中找到这个脚本，把它挂载到当前窗口物体上
        //获取所有程序集
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        //遍历所有程序集，找到CSharp程序集
        Assembly cSharpAssembly = assemblies.FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp");
        //获取类所在的程序集路径
        string relClassName = "ZMUIFrameWork." + className;
        Type type = cSharpAssembly.GetType(relClassName);
        if(type == null)
        {
            return;
        }
        
        //获取到要挂载的那个物体
        string windowObjName = className.Replace("DataComponent", "");
        GameObject windowObj = GameObject.Find(windowObjName);
        if (windowObj == null)
        {
            windowObj = GameObject.Find(("UIRoot/" + windowObjName));
            if (windowObj == null)
            {
                return;
            }
        }
        //先获取现在窗口上有没有这个数据组件， 如果没有才进行挂载
        Component component = windowObj.GetComponent(type);
        if (component == null)
        {
            component = windowObj.AddComponent(type);
        }
        
        //2.通过反射的方式，遍历数据列表，找到对应的字段，赋值
        //获取对象数据列表
        string dataListJson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
        List<EditorObjectData> objDataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(dataListJson);
        //获取脚本所有字段
        FieldInfo[] fields = type.GetFields();
        //遍历字段，找到对应的字段，赋值
        foreach (var item in fields)
        {
            foreach (var objectData in objDataList)
            {
                if(item.Name == objectData.fieldName + objectData.fieldType)
                {
                    //根据InsId找到对应的对象
                    GameObject uiObject = EditorUtility.InstanceIDToObject(objectData.insID) as GameObject;
                    //设置该字段所对应的对象
                    if(string.Equals(objectData.fieldType, "GameObject"))
                    {
                        item.SetValue(component, uiObject);
                    }
                    else
                    {
                        item.SetValue(component, uiObject.GetComponent(objectData.fieldType));
                    }
                    break;
                }
            }
        }
        
        //删除存储数据列表的EditorPrefs,防止改动其他脚本时候再次调用该接口
        EditorPrefs.DeleteKey(GeneratorConfig.GENERATOR_CLASS_NAME);
    }
}
