namespace H.NotifyIcon;

/// <summary>
/// Util and extension methods.
/// </summary>
internal static class Util
{
    public static readonly object SyncRoot = new object();

    #region IsDesignMode

    private static readonly bool isDesignMode = false;

    /// <summary>
    /// Checks whether the application is currently in design mode.
    /// </summary>
    public static bool IsDesignMode
    {
        get { return isDesignMode; }
    }

    #endregion

    #region construction

    static Util()
    {
#if HAS_WPF
        isDesignMode =
            (bool)
                DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
                    typeof (FrameworkElement))
                    .Metadata.DefaultValue;
#endif
    }

    #endregion

    #region CreateHelperWindow

    /// <summary>
    /// Creates an transparent window without dimension that
    /// can be used to temporarily obtain focus and/or
    /// be used as a window message sink.
    /// </summary>
    /// <returns>Empty window.</returns>
    public static Window CreateHelperWindow()
    {
        return new Window
        {
#if HAS_WPF
            Width = 0,
            Height = 0,
            ShowInTaskbar = false,
            WindowStyle = WindowStyle.None,
            AllowsTransparency = true,
            Opacity = 0
#endif
        };
    }

    #endregion

    #region ImageSource to Icon

#if HAS_WPF

    /// <summary>
    /// Reads a given image resource into a WinForms icon.
    /// </summary>
    /// <param name="imageSource">Image source pointing to
    /// an icon file (*.ico).</param>
    /// <returns>An icon object that can be used with the
    /// taskbar area.</returns>
    public static Icon ToIcon(this ImageSource imageSource)
    {
        if (imageSource == null) return null;

        Uri uri = new Uri(imageSource.ToString());
        StreamResourceInfo streamInfo = Application.GetResourceStream(uri);

        if (streamInfo == null)
        {
            string msg = "The supplied image source '{0}' could not be resolved.";
            msg = string.Format(msg, imageSource);
            throw new ArgumentException(msg);
        }

        return new Icon(streamInfo.Stream);
    }

#else

    /// <summary>
    /// Reads a given image resource into a WinForms icon.
    /// </summary>
    /// <param name="imageSource">Image source pointing to
    /// an icon file (*.ico).</param>
    /// <returns>An icon object that can be used with the
    /// taskbar area.</returns>
    public static async Task<Icon> ToIconAsync(this ImageSource imageSource)
    {
        var image = (BitmapImage)imageSource;
        var test = await StorageFile.GetFileFromApplicationUriAsync(image.UriSource);
        var stream = await test.OpenStreamForReadAsync();

        return new Icon(stream);
    }

#endif

    #endregion

    #region execute command

    /// <summary>
    /// Executes a given command if its <see cref="ICommand.CanExecute"/> method
    /// indicates it can run.
    /// </summary>
    /// <param name="command">The command to be executed, or a null reference.</param>
    /// <param name="commandParameter">An optional parameter that is associated with
    /// the command.</param>
    public static void ExecuteIfEnabled(this ICommand command, object commandParameter)
    {
        if (command == null) return;
        
        if (command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    #endregion

#if HAS_WPF

    /// <summary>
    /// Executes a given command if its <see cref="ICommand.CanExecute"/> method
    /// indicates it can run.
    /// </summary>
    /// <param name="command">The command to be executed, or a null reference.</param>
    /// <param name="commandParameter">An optional parameter that is associated with
    /// the command.</param>
    /// <param name="target">The target element on which to raise the command.</param>
    public static void ExecuteIfEnabled(this ICommand command, object commandParameter, IInputElement target)
    {
        if (command == null) return;

        RoutedCommand rc = command as RoutedCommand;
        if (rc != null)
        {
            //routed commands work on a target
            if (rc.CanExecute(commandParameter, target)) rc.Execute(commandParameter, target);
        }
        else if (command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    /// <summary>
    /// Returns a dispatcher for multi-threaded scenarios
    /// </summary>
    /// <returns>Dispatcher</returns>
    internal static Dispatcher GetDispatcher(this DispatcherObject source)
    {
        //use the application's dispatcher by default
        if (Application.Current != null) return Application.Current.Dispatcher;

        //fallback for WinForms environments
        if (source.Dispatcher != null) return source.Dispatcher;

        // ultimately use the thread's dispatcher
        return Dispatcher.CurrentDispatcher;
    }

#endif

    /// <summary>
    /// Checks whether the DataContextProperty
    ///  is bound or not.
    /// </summary>
    /// <param name="element">The element to be checked.</param>
    /// <returns>True if the data context property is being managed by a
    /// binding expression.</returns>
    /// <exception cref="ArgumentNullException">If <paramref name="element"/>
    /// is a null reference.</exception>
    public static bool IsDataContextDataBound(this FrameworkElement element)
    {
        if (element == null) throw new ArgumentNullException("element");
        return element.GetBindingExpression(FrameworkElement.DataContextProperty) != null;
    }
}
