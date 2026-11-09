using System.Runtime.InteropServices;

namespace H.NotifyIcon.Interop;

/// <summary>
/// Package identity APIs, declared by hand because CsWin32 would generate
/// the whole WIN32_ERROR enum (over 3,000 members) for their return type.
/// </summary>
internal static unsafe class PackageMethods
{
    internal const uint APPMODEL_ERROR_NO_PACKAGE = 15700;

    [SupportedOSPlatform("windows8.0")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("kernel32.dll", ExactSpelling = true)]
    internal static extern uint GetCurrentPackageId(uint* bufferLength, byte* buffer);

    [SupportedOSPlatform("windows8.0")]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    [DllImport("kernel32.dll", ExactSpelling = true)]
    internal static extern uint GetCurrentPackageFullName(uint* packageFullNameLength, char* packageFullName);
}
