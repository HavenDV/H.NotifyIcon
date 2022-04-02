namespace H.NotifyIcon;

internal static class Util
{
    #region IsDesignMode

    public static bool IsDesignMode { get; }

    #endregion

    #region Constructors

    static Util()
    {
#if HAS_WPF
        IsDesignMode = (bool)DependencyPropertyDescriptor.FromProperty(
            DesignerProperties.IsInDesignModeProperty,
            typeof (FrameworkElement)).Metadata.DefaultValue;
#endif
    }

    #endregion

    #region execute command

    public static void ExecuteIfEnabled(this ICommand command, object? commandParameter)
    {
        if (command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    #endregion

#if HAS_WPF

    public static void ExecuteIfEnabled(this ICommand command, object? commandParameter, IInputElement target)
    {
        var rc = command as RoutedCommand;
        if (rc != null)
        {
            //routed commands work on a target
            if (rc.CanExecute(commandParameter, target))
            {
                rc.Execute(commandParameter, target);
            }
        }
        else if (command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    internal static Dispatcher GetDispatcher(this DispatcherObject source)
    {
        if (Application.Current != null)
        {
            return Application.Current.Dispatcher;
        }
        if (source.Dispatcher != null)
        {
            return source.Dispatcher;
        }

        return Dispatcher.CurrentDispatcher;
    }

#endif
}
