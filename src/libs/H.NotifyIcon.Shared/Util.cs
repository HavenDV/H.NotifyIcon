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

    #region execute command

    /// <summary>
    /// Executes a given command if its <see cref="ICommand.CanExecute"/> method
    /// indicates it can run.
    /// </summary>
    /// <param name="command">The command to be executed, or a null reference.</param>
    /// <param name="commandParameter">An optional parameter that is associated with
    /// the command.</param>
    public static void ExecuteIfEnabled(this ICommand command, object? commandParameter)
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
    public static void ExecuteIfEnabled(this ICommand command, object? commandParameter, IInputElement target)
    {
        if (command == null) return;

        var rc = command as RoutedCommand;
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
}
