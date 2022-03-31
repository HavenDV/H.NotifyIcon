namespace H.NotifyIcon.Interop;

/// <summary>
/// Util and extension methods.
/// </summary>
public static class Extensions
{
    #region GetBalloonFlag

    /// <summary>
    /// Gets Interop flags that matches a given <see cref="BalloonIcon"/>.
    /// https://docs.microsoft.com/en-us/windows/win32/api/shellapi/ns-shellapi-notifyicondataa#niif_none-0x00000000
    /// </summary>
    public static uint GetBalloonFlag(this BalloonIcon icon)
    {
        switch (icon)
        {
            case BalloonIcon.None:
                return PInvoke.NIIF_NONE;
            case BalloonIcon.Info:
                return PInvoke.NIIF_INFO;
            case BalloonIcon.Warning:
                return PInvoke.NIIF_WARNING;
            case BalloonIcon.Error:
                return PInvoke.NIIF_ERROR;
            default:
                throw new ArgumentOutOfRangeException("icon");
        }
    }

    #endregion

    #region evaluate listings

    /// <summary>
    /// Checks a list of candidates for equality to a given
    /// reference value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value">The evaluated value.</param>
    /// <param name="candidates">A liste of possible values that are
    /// regarded valid.</param>
    /// <returns>True if one of the submitted <paramref name="candidates"/>
    /// matches the evaluated value. If the <paramref name="candidates"/>
    /// parameter itself is null, too, the method returns false as well,
    /// which allows to check with null values, too.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="candidates"/>
    /// is a null reference.</exception>
    public static bool Is<T>(this T value, params T[] candidates)
    {
        if (candidates == null) return false;

        foreach (var t in candidates)
        {
            if (value.Equals(t)) return true;
        }

        return false;
    }

    #endregion

    #region match MouseEvent to PopupActivation

    /// <summary>
    /// Checks if a given <see cref="PopupActivationMode"/> is a match for
    /// an effectively pressed mouse button.
    /// </summary>
    public static bool IsMatch(this MouseEvent me, PopupActivationMode activationMode)
    {
        switch (activationMode)
        {
            case PopupActivationMode.LeftClick:
                return me == MouseEvent.IconLeftMouseUp;
            case PopupActivationMode.RightClick:
                return me == MouseEvent.IconRightMouseUp;
            case PopupActivationMode.LeftOrRightClick:
                return me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconRightMouseUp);
            case PopupActivationMode.LeftOrDoubleClick:
                return me.Is(MouseEvent.IconLeftMouseUp, MouseEvent.IconDoubleClick);
            case PopupActivationMode.DoubleClick:
                return me.Is(MouseEvent.IconDoubleClick);
            case PopupActivationMode.MiddleClick:
                return me == MouseEvent.IconMiddleMouseUp;
            case PopupActivationMode.All:
                //return true for everything except mouse movements
                return me != MouseEvent.MouseMove;
            default:
                throw new ArgumentOutOfRangeException("activationMode");
        }
    }

    #endregion
}
