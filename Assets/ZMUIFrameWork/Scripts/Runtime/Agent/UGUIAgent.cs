using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UGUI 控件的辅助类，用于设置 Unity UI 控件的可见性。
/// </summary>
public static class UGUIAgent
{
    /// <summary>
    /// 设置 GameObject 的可见性。
    /// </summary>
    /// <param name="go">要设置可见性的 GameObject。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this GameObject go, bool isVisible)
    {
        go.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Transform 的可见性（此方法有误，Transform 没有 transform 属性，应为自身）。
    /// </summary>
    /// <param name="transform">要设置可见性的 Transform。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Transform transform, bool isVisible)
    {
        transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Button 的可见性。
    /// </summary>
    /// <param name="button">要设置可见性的 Button。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Button button, bool isVisible)
    {
        button.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Text 的可见性。
    /// </summary>
    /// <param name="text">要设置可见性的 Text。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Text text, bool isVisible)
    {
        text.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Image 的可见性。
    /// </summary>
    /// <param name="image">要设置可见性的 Image。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Image image, bool isVisible)
    {
        image.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 RawImage 的可见性。
    /// </summary>
    /// <param name="rawImage">要设置可见性的 RawImage。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this RawImage rawImage, bool isVisible)
    {
        rawImage.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 ScrollRect 的可见性。
    /// </summary>
    /// <param name="scrollRect">要设置可见性的 ScrollRect。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this ScrollRect scrollRect, bool isVisible)
    {
        scrollRect.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Scrollbar 的可见性。
    /// </summary>
    /// <param name="scrollbar">要设置可见性的 Scrollbar。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Scrollbar scrollbar, bool isVisible)
    {
        scrollbar.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Slider 的可见性。
    /// </summary>
    /// <param name="slider">要设置可见性的 Slider。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Slider slider, bool isVisible)
    {
        slider.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 InputField 的可见性。
    /// </summary>
    /// <param name="inputField">要设置可见性的 InputField。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this InputField inputField, bool isVisible)
    {
        inputField.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Toggle 的可见性。
    /// </summary>
    /// <param name="toggle">要设置可见性的 Toggle。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Toggle toggle, bool isVisible)
    {
        toggle.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Dropdown 的可见性。
    /// </summary>
    /// <param name="dropdown">要设置可见性的 Dropdown。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Dropdown dropdown, bool isVisible)
    {
        dropdown.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }

    /// <summary>
    /// 设置 Mask 的可见性。
    /// </summary>
    /// <param name="mask">要设置可见性的 Mask。</param>
    /// <param name="isVisible">是否可见。</param>
    public static void SetVisible(this Mask mask, bool isVisible)
    {
        mask.transform.localScale = isVisible ? Vector3.one : Vector3.zero;
    }
}
