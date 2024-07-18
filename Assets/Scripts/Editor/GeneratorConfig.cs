using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GeneratorType
{
    /// <summary>
    /// 找组件的方式
    /// </summary>
    FindComponent,
    
    /// <summary>
    /// 绑定组件的方式
    /// </summary>
    BindComponent,
}

public class GeneratorConfig
{
    /// <summary>
    /// 用于查找组件生成器的路径。
    /// </summary>
    public static readonly string FindComponentGeneratorPath = Application.dataPath + "/Scripts/FindComponent";
    
    /// <summary>
    /// 用于绑定组件生成器的路径。
    /// </summary>
    public static readonly string BindComponentGeneratorPath = Application.dataPath + "/Scripts/BindComponent";

    /// <summary>
    /// 用于窗口生成器的路径。
    /// </summary>
    public static readonly string WindowGeneratorPath = Application.dataPath + "/Scripts/Window";

    /// <summary>
    /// 用于存储对象数据列表的键。
    /// </summary>
    public static readonly string OBJDATALIST_KEY = "objDataList";

    /// <summary>
    /// 用于存储对象数据列表的路径。
    /// </summary>
    public static readonly string GENERATOR_CLASS_NAME = "GeneratorClassName";
    
    public static readonly GeneratorType GeneratorType = GeneratorType.BindComponent;
}
