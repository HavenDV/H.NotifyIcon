namespace H.NotifyIcon;

public partial class TaskbarIcon
{
    #region Properties

    private bool IsContextMenuVisible { get; set; }
    private Window? ContextMenuWindow { get; set; }
    private nint? ContextMenuWindowHandle { get; set; }
    private AppWindow? ContextMenuAppWindow { get; set; }
    private MenuFlyout? ContextMenuFlyout { get; set; }
    private bool FirstMeasure { get; set; } = true;
    private bool PointerActionInContextMenuWindow { get; set; }
    private bool SecondMeasureAfterPointerAction { get; set; }

#pragma warning disable CA1822 // Mark members as static
    partial void OnContextMenuModeChanged(ContextMenuMode oldValue, ContextMenuMode newValue)
#pragma warning restore CA1822 // Mark members as static
    {
        if (newValue is ContextMenuMode.SecondWindow)
        {
            PrepareContextMenuWindow();
        }
    }

    #endregion
    
    #region Methods

    private void ShowContextMenuInSecondWindowMode(System.Drawing.Point cursorPosition)
    {
        if (ContextMenuWindowHandle == null ||
            ContextMenuFlyout == null)
        {
            return;
        }

        var size = MeasureFlyout(
            flyout: ContextMenuFlyout,
            availableSize: new Size(10000.0, 10000.0),
            firstMeasure: FirstMeasure,
            pointerActionInContextMenuWindow: PointerActionInContextMenuWindow,
            secondMeasureAfterPointerAction: SecondMeasureAfterPointerAction);
        FirstMeasure = false;
        if (PointerActionInContextMenuWindow)
        {
            SecondMeasureAfterPointerAction = true;
        }
        var rectangle = CursorUtilities.CalculatePopupWindowPosition(
            cursorPosition.X,
            cursorPosition.Y,
            (int)size.Width,
            (int)size.Height);

        ContextMenuAppWindow?.MoveAndResize(rectangle.ToRectInt32());
        _ = WindowUtilities.ShowWindow(ContextMenuWindowHandle.Value);
        _ = WindowUtilities.SetForegroundWindow(ContextMenuWindowHandle.Value);
    }

    private void PrepareContextMenuWindow()
    {
        if (ContextFlyout == null ||
            ContextMenuMode != ContextMenuMode.SecondWindow)
        {
            return;
        }

        var frame = new Frame
        {
            Background = new SolidColorBrush(Colors.Transparent),
        };
        var window = new Window()
        {
            Content = frame,
        };

        ActualThemeChanged += (_, _) => frame.RequestedTheme = ActualTheme;

        var handle = WindowNative.GetWindowHandle(window);
        WindowUtilities.SetRoundedCorners(handle);
        WindowUtilities.MakeTransparent(handle);

#if !HAS_UNO
        var id = Win32Interop.GetWindowIdFromWindow(handle);
        var appWindow = AppWindow.GetFromWindowId(id);
        appWindow.IsShownInSwitchers = false;

        var presenter = (OverlappedPresenter)appWindow.Presenter;
        presenter.IsMaximizable = false;
        presenter.IsMinimizable = false;
        presenter.IsResizable = false;
        presenter.IsAlwaysOnTop = true;
        presenter.SetBorderAndTitleBar(false, false);
#endif

        var flyout = new MenuFlyout
        {
            AreOpenCloseAnimationsEnabled = ContextFlyout.AreOpenCloseAnimationsEnabled,
            Placement = FlyoutPlacementMode.Full,
        };
        flyout.Closed += async (_, _) =>
        {
            if (!flyout.AreOpenCloseAnimationsEnabled ||
                !IsContextMenuVisible)
            {
                _ = WindowUtilities.HideWindow(handle);
                return;
            }

            await Task.Delay(1).ConfigureAwait(true);
            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
        };
        foreach (var flyoutItemBase in ((MenuFlyout)ContextFlyout).Items)
        {
            flyout.Items.Add(flyoutItemBase);
            flyoutItemBase.Tapped += (_, _) =>
            {
                IsContextMenuVisible = false;
                flyout.Hide();
                _ = WindowUtilities.HideWindow(handle);
            };
            flyoutItemBase.PointerMoved += (_, _) => PointerActionInContextMenuWindow = true;
            flyoutItemBase.PointerPressed += (_, _) => PointerActionInContextMenuWindow = true;
        }

        frame.Loaded += (_, _) =>
        {
            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
            flyout.Hide();
        };
        window.Activated += (sender, args) =>
        {
            if (args.WindowActivationState == WindowActivationState.Deactivated)
            {
                IsContextMenuVisible = false;
                flyout.Hide();
                _ = WindowUtilities.HideWindow(handle);
                return;
            }

            if (ContextMenuWindow == null)
            {
                return;
            }

            IsContextMenuVisible = true;
            flyout.ShowAt(window.Content, new FlyoutShowOptions
            {
                ShowMode = FlyoutShowMode.Transient,
            });
        };

        ContextMenuWindow = window;
        ContextMenuWindowHandle = handle;
#if !HAS_UNO
        ContextMenuAppWindow = appWindow;
#endif
        ContextMenuFlyout = flyout;
    }

    private static Size MeasureFlyout(
        MenuFlyout flyout,
        Size availableSize,
        bool firstMeasure,
        bool pointerActionInContextMenuWindow,
        bool secondMeasureAfterPointerAction)
    {
        var width = 0.0;
        var height = 4.0; // top and bottom margin

        foreach (var item in flyout.Items)
        {
            item.Measure(availableSize);

            var additionalHeight = item is MenuFlyoutItem && firstMeasure
                ? 2.0
                : 0.0;
            // https://github.com/microsoft/microsoft-ui-xaml/issues/7374
            additionalHeight += item is MenuFlyoutItem &&
                (pointerActionInContextMenuWindow && !secondMeasureAfterPointerAction)
                ? -8.0
                : 0.0;
            width = Math.Max(width, item.DesiredSize.Width);
            height += item.DesiredSize.Height + additionalHeight;
        }

        var scale = flyout.XamlRoot?.RasterizationScale ?? 1.0;

        return new Size(
            width: Math.Round(scale * width
                + (firstMeasure ? 128.0 : 0.0) // ??
                + 4.0),  // borders
            height: Math.Round(scale * height + 4.0)); // borders
    }

#endregion
}
