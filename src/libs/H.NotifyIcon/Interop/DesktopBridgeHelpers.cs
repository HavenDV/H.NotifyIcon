#pragma warning disable CA1416

namespace H.NotifyIcon.Interop;

/// <summary>
/// 
/// </summary>
public static class DesktopBridgeHelpers
{
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public static unsafe bool IsRunningAsUwp()
    {
        if (IsWindows7OrLower)
        {
            return false;
        }

        uint length = 0;
        var empty = stackalloc char[0];
        _ = PInvoke.GetCurrentPackageFullName(&length, empty);

        var text = stackalloc char[(int)length];
        var result = PInvoke.GetCurrentPackageFullName(&length, text);

        return (long)result != PInvoke.APPMODEL_ERROR_NO_PACKAGE;
    }

    private static bool IsWindows7OrLower
    {
        get
        {
            int versionMajor = Environment.OSVersion.Version.Major;
            int versionMinor = Environment.OSVersion.Version.Minor;
            double version = versionMajor + (double)versionMinor / 10;
            return version <= 6.1;
        }
    }
}
