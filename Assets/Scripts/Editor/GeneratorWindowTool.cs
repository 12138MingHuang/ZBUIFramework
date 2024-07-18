using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

public class GeneratorWindowTool : Editor
{
    private static Dictionary<string, string> methodDic = new Dictionary<string, string>();

    [MenuItem("GameObject/生成Window脚本", false, 3)]
    private static void CreatWindowScripts()
    {
        GameObject obj = Selection.objects.First() as GameObject; //获取当前选择物体
        if (obj == null)
        {
            Debug.LogError("需要选择 GameObject");
            return;
        }

        // 设置脚本生成路径
        if (!Directory.Exists(GeneratorConfig.WindowGeneratorPath))
        {
            Directory.CreateDirectory(GeneratorConfig.WindowGeneratorPath);
        }

        // 生成CS脚本文件
        string csCotent = CreatWindowCS(obj.name);
        Debug.Log("CS脚本生成成功：\n" + csCotent);
        string csPath = $"{GeneratorConfig.WindowGeneratorPath}/{obj.name}.cs";

        UIWindowEditor.ShowWindow(csCotent, csPath, methodDic);
    }

    /// <summary>
    /// 生成CS脚本文件
    /// </summary>
    /// <param name="name">解析到的字符串</param>
    private static string CreatWindowCS(string name)
    {
        // 拿去字段名称
        string datalistJson = PlayerPrefs.GetString(GeneratorConfig.OBJDATALIST_KEY);
        List<EditorObjectData> objDataList = JsonConvert.DeserializeObject<List<EditorObjectData>>(datalistJson);
        methodDic.Clear();
        StringBuilder sb = new StringBuilder();

        //添加引用，注释，引入命名空间
        sb.AppendLine("/*");
        sb.AppendLine(" *-------------------------");
        sb.AppendLine(" *Title:UI表现层脚本自动生成工具");
        sb.AppendLine(" *Author:ZHANGBIN");
        sb.AppendLine(" *Date:" + DateTime.Now.ToString());
        sb.AppendLine(" *Description:改脚本只负责UI界面的交互，表现上的更新，不建议在此填写业务层的相关逻辑");
        sb.AppendLine(" *注意：以下文件是自动生成的，再次生成不会覆盖原有的代码，会在原有的代码上新增");
        sb.AppendLine(" *--------------------------");
        sb.AppendLine(" */");
        sb.AppendLine("using UnityEngine;");
        sb.AppendLine("using UnityEngine.UI;");
        sb.AppendLine("using ZMUIFrameWork;");

        sb.AppendLine();

        //生成类名
        sb.AppendLine($"public class {name} : WindowBase");
        sb.AppendLine("{");

        //生成字段
        if (GeneratorConfig.GeneratorType == GeneratorType.FindComponent)
        {
            sb.AppendLine($"\tpublic {name}UIComponent uiCompt = new {name}UIComponent();");
        }
        else if (GeneratorConfig.GeneratorType == GeneratorType.BindComponent)
        {
            sb.AppendLine($"\tpublic {name}DataComponent dataCompt = new {name}DataComponent();");
        }

        //生成生命周期函数
        sb.AppendLine();
        sb.AppendLine("\t#region 生命周期函数");
        //生成OnAwake
        sb.AppendLine("\t//调用机制与Mono Awake一致");
        sb.AppendLine("\tpublic override void OnAwake()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tbase.OnAwake();");
        if (GeneratorConfig.GeneratorType == GeneratorType.FindComponent)
        {
            sb.AppendLine("\t\tuiCompt.InitComponent(this);");
        }
        else if (GeneratorConfig.GeneratorType == GeneratorType.BindComponent)
        {
            sb.AppendLine("\t\tdataCompt.InitComponent(this);");
        }
        sb.AppendLine("\t}");

        //生成OnHide
        sb.AppendLine("\t//当界面隐藏时调用。");
        sb.AppendLine("\tpublic override void OnHide()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tbase.OnHide();");
        sb.AppendLine("\t}");

        //生成OnDestroy
        sb.AppendLine("\t//当界面销毁时调用。");
        sb.AppendLine("\tpublic override void OnDestroy()");
        sb.AppendLine("\t{");
        sb.AppendLine("\t\tbase.OnDestroy();");
        sb.AppendLine("\t}");

        sb.AppendLine("\t#endregion");

        sb.AppendLine();

        //API Function
        sb.AppendLine("\t#region API Function");
        sb.AppendLine();
        sb.AppendLine("\t#endregion");

        sb.AppendLine();

        //UI组件生成事件
        sb.AppendLine("\t#region UI组件生成事件");

        foreach (var item in objDataList)
        {
            string type = item.fieldType;
            string methodName = $"On{item.fieldName}";
            string suffix = ""; // 事件名后缀
            if (type.Contains("Button"))
            {
                suffix = "ButtonClick";
                CreatMethod(sb, ref methodDic, methodName + suffix);
            }
            else if (type.Contains("InputField"))
            {
                suffix = "InputChange";
                CreatMethod(sb, ref methodDic, methodName + suffix, "string text");
                suffix = "InputEnd";
                CreatMethod(sb, ref methodDic, methodName + suffix, "string text");
            }
            else if (type.Contains("Toggle"))
            {
                suffix = "ToggleChange";
                CreatMethod(sb, ref methodDic, methodName + suffix, "bool state, Toggle toggle");
            }
        }

        sb.AppendLine("\t#endregion");

        sb.AppendLine("}");

        return sb.ToString();
    }

    /// <summary>
    /// 生成UI事件方法
    /// 此方法用于根据给定的参数生成UI事件处理方法的代码片段。
    /// </summary>
    /// <param name="sb">StringBuilder实例，用于存储生成的代码片段。</param>
    /// <param name="methodDic">包含事件处理方法的字典，键为方法名，值为方法实现。</param>
    /// <param name="methodName">要生成的事件处理方法的名称。</param>
    /// <param name="param">事件处理方法的参数列表（如果有的话）。</param>
    public static void CreatMethod(StringBuilder sb, ref Dictionary<string, string> methodDic, string methodName, string param = "")
    {
        //声明UI组件事件
        sb.AppendLine($"\tpublic void {methodName}({param})");
        sb.AppendLine("\t{");
        sb.AppendLine(); // 添加一个空行
        if (methodName == "OnCloseButtonClick")
        {
            sb.AppendLine("\t\tHideWindow();");
        }
        sb.AppendLine("\t}");

        sb.AppendLine(); // 添加一个换行符

        //存储UI组件事件 提供给后续新增代码使用
        StringBuilder builder = new StringBuilder();
        builder.AppendLine($"\tpublic void {methodName}({param})");
        builder.AppendLine("\t{");
        builder.AppendLine("\t\t");
        builder.AppendLine("\t}");
        methodDic.Add(methodName, builder.ToString());
    }
}
