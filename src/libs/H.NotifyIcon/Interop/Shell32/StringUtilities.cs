namespace H.NotifyIcon.Interop;

internal static class StringUtilities
{
    public static unsafe void SetTo(
        this string text,
        char* start,
        int maxLength)
    {
        text ??= string.Empty;

        for (var i = 0; i < text.Length && i < maxLength; i++)
        {
            start[i] = text[i];
        }

        start[Math.Min(text.Length, maxLength - 1)] = '\0';
    }
}
