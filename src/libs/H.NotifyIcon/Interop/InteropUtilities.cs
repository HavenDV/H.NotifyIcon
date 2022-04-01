using System.Runtime.InteropServices;

namespace H.NotifyIcon.Interop;

internal static class InteropUtilities
{
    /// <exception cref="COMException"></exception>
    public static HWND EnsureNonNull(this HWND value)
    {
        if (value.Value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static ushort EnsureNonZero(this ushort value)
    {
        if (value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static uint EnsureNonZero(this uint value)
    {
        if (value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }

    /// <exception cref="COMException"></exception>
    public static BOOL EnsureNonZero(this BOOL value)
    {
        if (value.Value == 0)
        {
            Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
        }

        return value;
    }
}
