using System.Runtime.InteropServices;
using H.NotifyIcon.Core;

namespace H.NotifyIcon.Interop;

internal static class NativeThemeUtilities
{
    public static void ApplyMenuTheme(PopupMenuThemeMode themeMode)
    {
        try
        {
            _ = SetPreferredAppMode(themeMode switch
            {
                PopupMenuThemeMode.System => PreferredAppMode.AllowDark,
                PopupMenuThemeMode.Light => PreferredAppMode.ForceLight,
                PopupMenuThemeMode.Dark => PreferredAppMode.ForceDark,
                _ => PreferredAppMode.Default,
            });
            FlushMenuThemes();
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }
        catch (BadImageFormatException)
        {
        }
    }

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("uxtheme.dll", EntryPoint = "#135", ExactSpelling = true)]
    private static extern PreferredAppMode SetPreferredAppMode(PreferredAppMode preferredAppMode);

    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("uxtheme.dll", EntryPoint = "#136", ExactSpelling = true)]
    private static extern void FlushMenuThemes();

    private enum PreferredAppMode
    {
        Default,
        AllowDark,
        ForceDark,
        ForceLight,
    }
}
