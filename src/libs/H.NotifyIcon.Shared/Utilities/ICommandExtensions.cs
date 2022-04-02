namespace H.NotifyIcon;

internal static class ICommandExtensions
{
    #region execute command

    public static void TryExecute(
        this ICommand command,
        object? commandParameter)
    {
        if (command.CanExecute(commandParameter))
        {
            command.Execute(commandParameter);
        }
    }

    #endregion

#if HAS_WPF

    public static void TryExecute(
        this ICommand command,
        object? commandParameter,
        IInputElement target)
    {
        if (command is RoutedCommand routedCommand)
        {
            if (routedCommand.CanExecute(commandParameter, target))
            {
                routedCommand.Execute(commandParameter, target);
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
