namespace H.NotifyIcon.Interop;

/// <summary>
/// 
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.0")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
internal static class User32Methods
{
    internal static nint SetWindowLong(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong)
    {
        return Environment.Is64BitProcess
            ? PInvoke.SetWindowLongPtr(hWnd, nIndex, dwNewLong)
            : PInvoke.SetWindowLong(hWnd, nIndex, (int)dwNewLong);
    }

    internal static nint GetWindowLong(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex)
    {
        return Environment.Is64BitProcess
            ? PInvoke.GetWindowLongPtr(hWnd, nIndex)
            : PInvoke.GetWindowLong(hWnd, nIndex);
    }
}
