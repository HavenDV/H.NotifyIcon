#if HAS_WPF
namespace H.NotifyIcon;

[RoutedEvent("BalloonShowing", RoutedEventStrategy.Bubble, IsAttached = true, Category = CategoryName)]
[RoutedEvent("BalloonClosing", RoutedEventStrategy.Bubble, IsAttached = true, Category = CategoryName)]
[DependencyProperty<Popup>("CustomBalloon", IsReadOnly = true,
    Description = "Maintains a currently displayed custom balloon.", Category = CategoryName)]
public partial class TaskbarIcon
{
    #region Properties

    /// <summary>
    /// A timer that is used to close open balloon tooltips.
    /// </summary>
    private readonly Timer balloonCloseTimer;

    #endregion

    #region Methods

    /// <summary>
    /// A delegate to handle customer popup positions.
    /// </summary>
    public delegate System.Drawing.Point GetCustomPopupPosition();

    /// <summary>
    /// Specify a custom popup position
    /// </summary>
    public GetCustomPopupPosition? CustomPopupPosition { get; set; }

    /// <summary>
    /// Returns the location of the system tray
    /// </summary>
    /// <returns>Point</returns>
    public static System.Drawing.Point GetPopupTrayPosition()
    {
        return TrayInfo.GetTrayLocation().ScaleWithDpi();
    }

    /// <summary>
    /// Shows a custom control as a tooltip in the tray location.
    /// </summary>
    /// <param name="balloon"></param>
    /// <param name="animation">An optional animation for the popup.</param>
    /// <param name="timeout">The time after which the popup is being closed.
    /// Submit null in order to keep the balloon open indefinitely
    /// </param>
    /// <exception cref="ArgumentNullException">If <paramref name="balloon"/>
    /// is a null reference.</exception>
    public void ShowCustomBalloon(UIElement balloon, PopupAnimation animation, int? timeout)
    {
        var dispatcher = this.GetDispatcher();
        if (!dispatcher.CheckAccess())
        {
            var action = new Action(() => ShowCustomBalloon(balloon, animation, timeout));
            _ = dispatcher.Invoke(DispatcherPriority.Normal, action);
            return;
        }

        if (balloon == null)
        {
            throw new ArgumentNullException(nameof(balloon));
        }

        if (timeout.HasValue && timeout < 500)
        {
            throw new ArgumentOutOfRangeException(
                nameof(timeout),
                $"Invalid timeout of {timeout} milliseconds. Timeout must be at least 500 ms");
        }

        EnsureNotDisposed();

        // make sure we don't have an open balloon
        //lock (lockObject)
        {
            CloseBalloon();
        }

        // create an invisible popup that hosts the UIElement
        var popup = new Popup
        {
            AllowsTransparency = true
        };

        // provide the popup with the taskbar icon's data context
        UpdateDataContext(popup, null, DataContext);

        // don't animate by default - developers can use attached events or override
        popup.PopupAnimation = animation;

        // in case the balloon is cleaned up through routed events, the
        // control didn't remove the balloon from its parent popup when
        // if was closed the last time - just make sure it doesn't have
        // a parent that is a popup
        var parent = LogicalTreeHelper.GetParent(balloon) as Popup;
        if (parent != null)
        {
            parent.Child = null;
        }

        if (parent != null)
        {
            throw new InvalidOperationException(
                $"Cannot display control [{balloon}] in a new balloon popup - " +
                $"that control already has a parent. You may consider creating " +
                $"new balloons every time you want to show one.");
        }

        popup.Child = balloon;

        //don't set the PlacementTarget as it causes the popup to become hidden if the
        //TaskbarIcon's parent is hidden, too...
        //popup.PlacementTarget = this;

        popup.Placement = PopupPlacement;
        popup.StaysOpen = true;


        var position = CustomPopupPosition != null
            ? CustomPopupPosition()
            : GetPopupTrayPosition();
        popup.HorizontalOffset = position.X - 1;
        popup.VerticalOffset = position.Y - 1;

        //store reference
        //lock (lockObject)
        {
            CustomBalloon = popup;
        }

        // assign this instance as an attached property
        SetParentTaskbarIcon(balloon, this);

        // fire attached event
        balloon.RaiseEvent(new RoutedEventArgs(BalloonShowingEvent, this));

        // display item
        popup.IsOpen = true;

        if (timeout.HasValue)
        {
            // register timer to close the popup
            _ = balloonCloseTimer.Change(timeout.Value, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Resets the closing timeout, which effectively
    /// keeps a displayed balloon message open until
    /// it is either closed programmatically through
    /// <see cref="CloseBalloon"/> or due to a new
    /// message being displayed.
    /// </summary>
    public void ResetBalloonCloseTimer()
    {
        if (IsDisposed)
        {
            return;
        }

        //lock (lockObject)
        {
            //reset timer in any case
            _ = balloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }
    }

    /// <summary>
    /// Closes the current <see cref="CustomBalloon"/>, if the
    /// property is set.
    /// </summary>
    public void CloseBalloon()
    {
        if (IsDisposed)
        {
            return;
        }

        var dispatcher = this.GetDispatcher();
        if (!dispatcher.CheckAccess())
        {
            Action action = CloseBalloon;
            _ = dispatcher.Invoke(DispatcherPriority.Normal, action);
            return;
        }

        //lock (lockObject)
        {
            // reset timer in any case
            _ = balloonCloseTimer.Change(Timeout.Infinite, Timeout.Infinite);

            // reset old popup, if we still have one
            var popup = CustomBalloon;
            if (popup == null)
            {
                return;
            }

            var element = popup.Child;

            // announce closing
            var eventArgs = new RoutedEventArgs(BalloonClosingEvent, this);
            element.RaiseEvent(eventArgs);
            if (!eventArgs.Handled)
            {
                // if the event was handled, clear the reference to the popup,
                // but don't close it - the handling code has to manage this stuff now

                // close the popup
                popup.IsOpen = false;

                // remove the reference of the popup to the balloon in case we want to reuse
                // the balloon (then added to a new popup)
                popup.Child = null;

                // reset attached property
                if (element != null)
                {
                    SetParentTaskbarIcon(element, null);
                }
            }

            // remove custom balloon anyway
            CustomBalloon = null;
        }
    }


    /// <summary>
    /// Timer-invoke event which closes the currently open balloon and
    /// resets the <see cref="CustomBalloon"/> dependency property.
    /// </summary>
    private void CloseBalloonCallback(object? state)
    {
        if (IsDisposed)
        {
            return;
        }

        // switch to UI thread
        Action action = CloseBalloon;
        this.GetDispatcher().Invoke(action);
    }

    #endregion
}

#endif
