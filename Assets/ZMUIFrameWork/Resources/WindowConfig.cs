using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[CreateAssetMenu(fileName = "WindowConfig", menuName = "WindowConfig", order = 0)]
public class WindowConfig : ScriptableObject
{
    [Space]
    [Header("UI文件夹路径列表，只填Resources下的文件夹名即可")]
    [Tooltip("windowPathList 用于指定需要生成配置的文件夹路径，以 Resources/ 为根目录。需手动填写，只填Resources下的文件夹名即可。")]
    public List<string> windowPathList = new List<string>();
    
    [Space]
    [Header("生成配置后的数据")]
    [Tooltip("windowList 用于保存生成配置后的数据。运行时自动读取")]
    public List<WindowData> windowList = new List<WindowData>();
    
    public void GenerateWindowConfig() // 更正了方法名拼写
    {
        // 如果windowRootArr为空，则给出提示并退出方法
        if (windowPathList.Count == 0)
        {
            Debug.LogError("windowRootArr为空，请添加文件夹路径以生成配置。");
            return;
        }
        //检测文件有没有新增，如果没有就不需要生成配置
        int count = 0;
        foreach (var item in windowPathList)
        {
            string folderPath = Application.dataPath + "/ZMUIFrameWork/Resources/" + item; // 简化路径拼接
            string[] filePathArr = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);
            foreach (var path in filePathArr)
            {
                if (path.EndsWith(".meta"))
                {
                    continue;
                }
                count++;
            }
        }


        if (count == windowList.Count)
        {
            Debug.Log("文件没有新增，不需要重新生成配置");
            return;
        }
        
        
        windowList.Clear();
        foreach (var item in windowPathList)
        {
            string folderPath = Application.dataPath + "/ZMUIFrameWork/Resources/" + item;
            string[] fileArr = Directory.GetFiles(folderPath, "*.prefab", SearchOption.AllDirectories);
            foreach (var file in fileArr)
            {
                if (file.EndsWith(".meta"))
                {
                    continue;
                }
                // 注意这里可能需要更改文件路径的构建方式，因为 item 不再是文件夹名的一部分（除非它恰好是）
                string fileName = Path.GetFileNameWithoutExtension(file);
                // 计算文件读取路径时，可能需要使用 file 的完整路径或者重新构建基于 item 的路径
                string path = item; // 如果 item 实际上表示了完整的路径前缀，这里可能不需要更改
                if (!item.EndsWith("/") && !item.EndsWith("\\")) // 确保路径分隔符
                {
                    path += "/";
                }
                path += fileName; // 假设 item 是路径前缀
                // 添加到列表
                windowList.Add(new WindowData() { name = fileName, path = path });
            }
        }
    }


    public string GetWindowPath(string windowName)
    {
        foreach (var item in windowList)
        {
            if (string.Equals(item.name, windowName))
            {
                return item.path;
            }
        }
        Debug.LogError(windowName + "不存在在配置文件中，请检查配置文件或者检查UI预制体文件");
        return null;
    }
}


[System.Serializable]
public class WindowData
{
    [Header("UI文件名")]
    public string name;
    [Header("UI文件路径")]
    public string path;
}