namespace H.NotifyIcon.Interop;

/// <summary>
/// A Interop proxy to for a taskbar icon (NotifyIcon) that sits in the system's
/// taskbar notification area ("system tray").
/// </summary>
#if NET5_0_OR_GREATER
[System.Runtime.Versioning.SupportedOSPlatform("windows5.1.2600")]
#elif NETSTANDARD2_0_OR_GREATER || NET451_OR_GREATER
#else
#error Target Framework is not supported
#endif
internal static class TrayIconMethods
{
    private static bool SendMessage(NOTIFY_ICON_MESSAGE command, NOTIFYICONDATAW32 data)
    {
        return PInvoke.Shell_NotifyIcon(command, in data);
    }

    private static bool SendMessage(NOTIFY_ICON_MESSAGE command, NOTIFYICONDATAW64 data)
    {
        return PInvoke.Shell_NotifyIcon(command, in data);
    }

    private static bool SendModifyMessage(NOTIFYICONDATAW32 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_MODIFY, data);
    }

    private static bool SendModifyMessage(NOTIFYICONDATAW64 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_MODIFY, data);
    }

    private static bool SendDeleteMessage(NOTIFYICONDATAW32 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_DELETE, data);
    }

    private static bool SendDeleteMessage(NOTIFYICONDATAW64 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_DELETE, data);
    }

    private static bool SendSetVersionMessage(NOTIFYICONDATAW32 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, data);
    }

    private static bool SendSetVersionMessage(NOTIFYICONDATAW64 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_SETVERSION, data);
    }

    private static bool SendSetFocusMessage(NOTIFYICONDATAW32 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_SETFOCUS, data);
    }

    private static bool SendSetFocusMessage(NOTIFYICONDATAW64 data)
    {
        return SendMessage(NOTIFY_ICON_MESSAGE.NIM_SETFOCUS, data);
    }

    public static unsafe bool ShowNotification(
        IntPtr handle,
        Guid id,
        NOTIFY_ICON_DATA_FLAGS flags,
        string title,
        string message,
        uint infoFlags,
        IntPtr balloonIconHandle,
        uint timeoutInMilliseconds)
    {
        if (Environment.Is64BitProcess)
        {
            var data = new NOTIFYICONDATAW64
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW64),
                uFlags = flags,
                hWnd = new HWND(handle),
                guidItem = id,
                dwInfoFlags = infoFlags,
                hBalloonIcon = new HICON(balloonIconHandle),
                Anonymous =
                {
                    uTimeout = timeoutInMilliseconds,
                }
            };
            message.SetTo(&data.szInfo._0, data.szInfo.Length);
            title.SetTo(&data.szInfoTitle._0, data.szInfoTitle.Length);

            return SendModifyMessage(data);
        }
        else
        {
            var data = new NOTIFYICONDATAW32
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW32),
                uFlags = flags,
                hWnd = new HWND(handle),
                guidItem = id,
                dwInfoFlags = infoFlags,
                hBalloonIcon = new HICON(balloonIconHandle),
                Anonymous =
                {
                    uTimeout = timeoutInMilliseconds,
                }
            };
            message.SetTo(&data.szInfo._0, data.szInfo.Length);
            title.SetTo(&data.szInfoTitle._0, data.szInfoTitle.Length);

            return SendModifyMessage(data);
        }
    }

    public static unsafe bool TrySetVersion(
        IntPtr handle,
        Guid id,
        NotifyIconVersion version)
    {
        if (Environment.Is64BitProcess)
        {
            var data = new NOTIFYICONDATAW64
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW64),
                hWnd = new HWND(handle),
                guidItem = id,
                Anonymous =
                {
                    uVersion = (uint)version,
                }
            };

            return SendSetVersionMessage(data);
        }
        else
        {
            var data = new NOTIFYICONDATAW32
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW32),
                hWnd = new HWND(handle),
                guidItem = id,
                Anonymous =
                {
                    uVersion = (uint)version,
                }
            };

            return SendSetVersionMessage(data);
        }
    }

    public static bool TrySetMostRecentVersion(
        IntPtr handle,
        Guid id,
        out NotifyIconVersion version)
    {
        version = NotifyIconVersion.Vista;
        var status = TrySetVersion(
            handle: handle,
            id: id,
            version: version);
        if (!status)
        {
            version = NotifyIconVersion.Win2000;
            status = TrySetVersion(
                handle: handle,
                id: id,
                version: version);
        }
        if (!status)
        {
            version = NotifyIconVersion.Win95;
            status = TrySetVersion(
                handle: handle,
                id: id,
                version: version);
        }

        return status;
    }

    public static unsafe bool SetFocus(
        IntPtr handle,
        Guid id)
    {
        if (Environment.Is64BitProcess)
        {
            var data = new NOTIFYICONDATAW64
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW64),
                hWnd = new HWND(handle),
                guidItem = id,
            };

            return SendSetFocusMessage(data);
        }
        else
        {
            var data = new NOTIFYICONDATAW32
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW32),
                hWnd = new HWND(handle),
                guidItem = id,
            };

            return SendSetFocusMessage(data);
        }
    }
}
