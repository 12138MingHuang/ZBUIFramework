using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 生成器类型枚举
/// </summary>
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

/// <summary>
/// 解析类型枚举
/// </summary>
public enum ParseType
{
    /// <summary>
    /// 名称解析
    /// </summary>
    Name,

    /// <summary>
    /// 标签解析
    /// </summary>
    Tag,
}

public class GeneratorConfig
{
    /// <summary>
    /// 用于查找组件生成器的路径。
    /// </summary>
    public static readonly string FindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/FindComponent";

    /// <summary>
    /// 用于绑定组件生成器的路径。
    /// </summary>
    public static readonly string BindComponentGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/BindComponent";

    /// <summary>
    /// 用于窗口生成器的路径。
    /// </summary>
    public static readonly string WindowGeneratorPath = Application.dataPath + "/ZMUIFrameWork/Scripts/Window";

    /// <summary>
    /// 用于存储对象数据列表的键。
    /// </summary>
    public static readonly string OBJDATALIST_KEY = "objDataList";

    /// <summary>
    /// 用于存储对象数据列表的路径。
    /// </summary>
    public static readonly string GENERATOR_CLASS_NAME = "GeneratorClassName";

    /// <summary>
    /// 静态只读字段，表示生成器类型，其默认值为 GeneratorType.BindComponent。
    /// </summary>
    public static readonly GeneratorType GeneratorType = GeneratorType.BindComponent;

    /// <summary>
    /// 包含一系列标签名称的静态只读字符串数组。
    /// </summary>
    public static readonly string[] TAGArr =
    {
        "Image", "RawImage", "Text", "Button", "Slider", "Dropdown", "InputField", "Canvas", "Panel", "ScrollRect",
        "Toggle"
    };

    /// <summary>
    /// 表示解析数据类型，其默认值为 ParseType.Name。
    /// </summary>
    public static readonly ParseType parseType = ParseType.Name;
}