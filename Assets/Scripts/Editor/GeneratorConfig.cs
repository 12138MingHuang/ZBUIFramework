using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorConfig
{
    /// <summary>
    /// 用于查找组件生成器的路径。
    /// </summary>
    public static string FindComponentGeneratorPath = Application.dataPath + "/Scripts/FindComponent";

    /// <summary>
    /// 用于窗口生成器的路径。
    /// </summary>
    public static string WindowGeneratorPath = Application.dataPath + "/Scripts/Window";

    /// <summary>
    /// 用于存储对象数据列表的键。
    /// </summary>
    public static string OBJDATALIST_KEY = "objDataList";
}
