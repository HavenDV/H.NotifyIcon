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
        if (Environment.OSVersion.Version <= new Version(major: 6, minor: 1))
        {
            return false;
        }

        uint length = 0;
        var empty = stackalloc char[0];
        _ = PackageMethods.GetCurrentPackageFullName(&length, empty);

        var text = stackalloc char[(int)length];
        var error = PackageMethods.GetCurrentPackageFullName(&length, text);

        return error != PackageMethods.APPMODEL_ERROR_NO_PACKAGE;
    }
}
