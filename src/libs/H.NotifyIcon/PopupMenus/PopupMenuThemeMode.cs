namespace H.NotifyIcon.Core;

/// <summary>
/// Defines how native popup menus should select their light or dark theme.
/// </summary>
public enum PopupMenuThemeMode
{
    /// <summary>
    /// Uses the Windows default native menu app mode.
    /// </summary>
    Default,

    /// <summary>
    /// Allows native popup menus to follow the current Windows app theme.
    /// This is the default value.
    /// </summary>
    System,

    /// <summary>
    /// Forces native popup menus to use a light theme when supported by Windows.
    /// </summary>
    Light,

    /// <summary>
    /// Forces native popup menus to use a dark theme when supported by Windows.
    /// </summary>
    Dark,
}
