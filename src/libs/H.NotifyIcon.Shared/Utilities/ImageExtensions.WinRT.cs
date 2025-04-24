using Microsoft.Win32;
using Windows.ApplicationModel.Resources.Core;

namespace H.NotifyIcon;

internal static partial class ImageExtensions
{
#if !HAS_MAUI
    internal static Stream ToStream(this Uri uri)
    {
        var prefix = uri.Scheme switch
        {
            "ms-appx" or "ms-appx-web" => AppContext.BaseDirectory,
            _ => string.Empty,
        };
        // additional schemes, like ms-appdata could be added here
        // see: https://learn.microsoft.com/en-us/windows/uwp/app-resources/uri-schemes
        var absolutePath = $"{prefix}{uri.LocalPath}";

        return File.OpenRead(absolutePath);
    }

    internal static async Task<Stream> ToStreamAsync(this Uri uri, CancellationToken cancellationToken = default)
    {
        var context = ResourceContext.GetForViewIndependentUse().Clone();

        using var personalizeKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Themes\Personalize");
        var systemUsesLightTheme = (int)(personalizeKey?.GetValue("SystemUsesLightTheme") ?? 0);

        context.QualifierValues["altform"] = systemUsesLightTheme != 0
            ? "unplated"
            : "lightunplated";

        var resource = ResourceManager.Current.MainResourceMap.GetSubtree("Files").GetValue(uri.LocalPath.Substring(1), context);

        return (await resource.GetValueAsStreamAsync()).AsStreamForRead();
    }

    public static Stream ToStream(this ImageSource imageSource)
    {
        switch (imageSource)
        {
            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;

                    return uri.ToStream();
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }

    public static async Task<Stream> ToStreamAsync(this ImageSource imageSource, CancellationToken cancellationToken = default)
    {
        switch(imageSource)
        {
            case BitmapImage bitmapImage:
                {
                    var uri = bitmapImage.UriSource;
                    
                    return await uri.ToStreamAsync(cancellationToken).ConfigureAwait(true);
                }

            default:
                throw new NotImplementedException($"ImageSource type: {imageSource.GetType()} is not supported");
        }
    }
#endif
}
