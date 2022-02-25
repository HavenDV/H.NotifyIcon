// Workaround for https://github.com/microsoft/WindowsAppSDK/issues/1686
global using Version = Microsoft.WindowsAppSDK.Runtime.Fix.Version;

namespace Microsoft.WindowsAppSDK.Runtime.Fix;

public class Version
{
    public const ushort Major = 0;
    public const ushort Minor = 319;
    public const ushort Build = 455;
    public const ushort Revision = 0;
    public const ulong UInt64 = 0x0000013F01C70000;
    public const string DotQuadString = "0.319.455.0";
}