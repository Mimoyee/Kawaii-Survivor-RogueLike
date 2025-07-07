// 文件名：SkipUnityLogo.cs
// 放置路径：Assets/任意非Editor文件夹（如Assets/Scripts/）
#if !UNITY_EDITOR
using UnityEngine;
using UnityEngine.Rendering;

[UnityEngine.Scripting.Preserve]
public class SkipUnityLogo
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void BeforeSplashScreen()
    {
#if UNITY_WEBGL
        // WebGL需等待焦点事件触发
        Application.focusChanged += OnFocusChanged; 
#else
        // 其他平台异步跳过Logo
        System.Threading.Tasks.Task.Run(AsyncSkip); 
#endif
    }

#if UNITY_WEBGL
    private static void OnFocusChanged(bool hasFocus)
    {
        Application.focusChanged -= OnFocusChanged;
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate);
    }
#else
    private static void AsyncSkip()
    {
        // 立即停止Unity启动画面渲染
        SplashScreen.Stop(SplashScreen.StopBehavior.StopImmediate); 
    }
#endif
}
#endif