namespace H.NotifyIcon.Interop;

/// <summary>
/// 
/// </summary>
[SupportedOSPlatform("windows5.0")]
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
