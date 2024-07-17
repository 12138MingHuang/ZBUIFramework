using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class UIWindowEditor : EditorWindow
{
    private string scriptContent;
    private string filePath;
    private Vector2 scroll = new Vector2();

    /// <summary>
    /// 显示代码展示窗口
    /// </summary>
    /// <param name="content"></param>
    /// <param name="filePath"></param>
    /// <param name="insterDic"></param>
    public static void ShowWindow(string content, string filePath, Dictionary<string, string> insterDic = null)
    {
        //创建代码展示窗口
        UIWindowEditor window = (UIWindowEditor)GetWindowWithRect(typeof(UIWindowEditor), new Rect(100, 50, 800, 700), true, "Window生成界面");
        window.scriptContent = content;
        window.filePath = filePath;

        //处理代码新增功能
        if(File.Exists(filePath) && insterDic != null)
        {
            //获取原始代码
            string originScript = File.ReadAllText(filePath);
            foreach (var item in insterDic)
            {
                if (!originScript.Contains(item.Key))
                {
                    int index = window.GetInsertIndex(originScript);
                    if (index != -1)
                    {
                        originScript = originScript.Insert(index, "\n\t\t" + item.Value + "\n");
                        window.scriptContent = originScript;
                    }
                }
            }
        }

        window.Show();
    }

    private void OnGUI()
    {
        //绘制scrollview
        scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(600), GUILayout.Width(800));
        EditorGUILayout.TextArea(scriptContent);
        EditorGUILayout.EndScrollView();
        EditorGUILayout.Space();

        //绘制脚本生成路径
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.TextArea("脚本生成路径：" + filePath);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        //绘制按钮
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("生成脚本", GUILayout.Height(30)))
        {
            //按钮事件
            ButtonClick();
        }
        EditorGUILayout.EndHorizontal();
    }

    public void ButtonClick()
    {
        //生成脚本文件
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        StreamWriter writer = File.CreateText(filePath);
        writer.Write(scriptContent);
        writer.Close();
        AssetDatabase.Refresh();
        if(EditorUtility.DisplayDialog("自动化生成工具", "生成脚本成功", "确定"))
        {
            Close();
        }
    }

    /// <summary>
    /// 获取插入代码的下标
    /// </summary>
    /// <param name="content">源代码</param>
    /// <returns></returns>
    public int GetInsertIndex(string content)
    {
        // 找到UI组件生成事件下面的第一个public所在位置进行插入
        Regex regionRegex = new Regex(@"#region UI组件生成事件");
        Match regionMatch = regionRegex.Match(content);

        if (!regionMatch.Success)
        {
            return -1;
        }

        int insertPosition = regionMatch.Index + regionMatch.Length;

        // 找到第一个public方法的位置
        Regex publicRegex = new Regex(@"\bpublic\b");
        MatchCollection publicMatches = publicRegex.Matches(content);

        foreach (Match match in publicMatches)
        {
            if (match.Index > insertPosition)
            {
                return match.Index;
            }
        }

        return -1;
    }
}
