using H.NotifyIcon.EfficiencyMode;

namespace H.NotifyIcon;

/// <summary>
/// Provides the most useful extensions to Window in the context of using TrayIcon.
/// </summary>
[CLSCompliant(false)]
public static class WindowExtensions
{
    /// <summary>
    /// Hides the window and optionally enables the Efficiency Mode for the current process.
    /// </summary>
    /// <returns></returns>
    public static void Hide(
        this Window window,
        bool enableEfficiencyMode = true)
    {
        window = window ?? throw new ArgumentNullException(nameof(window));

#if HAS_WPF
        window.Hide();
#elif !HAS_UNO && !HAS_MAUI
        WindowUtilities.HideWindow(WindowNative.GetWindowHandle(window));
#elif HAS_MAUI_WINUI
        WindowUtilities.HideWindow(WindowNative.GetWindowHandle(window.Handler!.PlatformView));
#endif

        // Important note: in .Net Framework if your executable assembly manifest doesn't explicitly state
        // that your exe assembly is compatible with Windows 8.1 and Windows 10.0, System.Environment.OSVersion
        // will return Windows 8 version, which is 6.2, instead of 6.3 and 10.0!
        if (enableEfficiencyMode &&
            Environment.OSVersion.Platform == PlatformID.Win32NT &&
            Environment.OSVersion.Version >= new Version(10, 0, 16299))
        {
#pragma warning disable CA1416 // Validate platform compatibility
            EfficiencyModeUtilities.SetEfficiencyMode(true);
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }
    
#if HAS_MAUI    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static void Activate(this Window window)
    {
        window = window ?? throw new ArgumentNullException(nameof(window));

#if HAS_MAUI_WINUI
        WindowUtilities.SetForegroundWindow(WindowNative.GetWindowHandle(window.Handler!.PlatformView));
#endif
    }
#endif
    
    /// <summary>
    /// Shows the window and optionally disables the Efficiency Mode for the current process.
    /// </summary>
    /// <returns></returns>
    public static void Show(
        this Window window,
        bool disableEfficiencyMode = true)
    {
        window = window ?? throw new ArgumentNullException(nameof(window));

#if HAS_WPF
        window.Show();
#elif !HAS_UNO && !HAS_MAUI
        WindowUtilities.ShowWindow(WindowNative.GetWindowHandle(window));
#elif HAS_MAUI_WINUI
        WindowUtilities.ShowWindow(WindowNative.GetWindowHandle(window.Handler!.PlatformView));
#endif

        // Important note: in .Net Framework if your executable assembly manifest doesn't explicitly state
        // that your exe assembly is compatible with Windows 8.1 and Windows 10.0, System.Environment.OSVersion
        // will return Windows 8 version, which is 6.2, instead of 6.3 and 10.0!
        if (disableEfficiencyMode &&
            Environment.OSVersion.Platform == PlatformID.Win32NT &&
            Environment.OSVersion.Version >= new Version(10, 0, 16299))
        {
#pragma warning disable CA1416 // Validate platform compatibility
            EfficiencyModeUtilities.SetEfficiencyMode(false);
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static void ShowInTaskbar(
        this Window window)
    {
        window = window ?? throw new ArgumentNullException(nameof(window));

#if HAS_WPF
        window.ShowInTaskbar = true;
#elif !HAS_UNO && !HAS_MAUI
        WindowUtilities.ShowWindowInTaskbar(WindowNative.GetWindowHandle(window));
#elif HAS_MAUI_WINUI
        WindowUtilities.ShowWindowInTaskbar(WindowNative.GetWindowHandle(window.Handler!.PlatformView));
#endif
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static void HideInTaskbar(
        this Window window)
    {
        window = window ?? throw new ArgumentNullException(nameof(window));

#if HAS_WPF
        window.ShowInTaskbar = false;
#elif !HAS_UNO && !HAS_MAUI
        WindowUtilities.HideWindowInTaskbar(WindowNative.GetWindowHandle(window));
#elif HAS_MAUI_WINUI
        WindowUtilities.HideWindowInTaskbar(WindowNative.GetWindowHandle(window.Handler!.PlatformView));
#endif
    }
}
