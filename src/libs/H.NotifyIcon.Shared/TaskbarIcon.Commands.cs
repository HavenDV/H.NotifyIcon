namespace H.NotifyIcon;

[DependencyProperty<ICommand>("DoubleClickCommand",
    Description = "A command that is being executed if the tray icon is being double-clicked.", Category = CategoryName)]
[DependencyProperty<object>("DoubleClickCommandParameter",
    Description = "Parameter to submit to the DoubleClickCommand when the user double clicks on the NotifyIcon.", Category = CategoryName)]
[DependencyProperty<ICommand>("LeftClickCommand",
    Description = "A command that is being executed if the tray icon is being left-clicked.", Category = CategoryName)]
[DependencyProperty<object>("LeftClickCommandParameter",
    Description = "The target of the command that is fired if the notify icon is clicked with the left mouse button.", Category = CategoryName)]
[DependencyProperty<bool>("NoLeftClickDelay",
    Description = "Set to true to make left clicks work without delay.", Category = CategoryName)]
#if HAS_WPF
[DependencyProperty<IInputElement>("DoubleClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is double clicked.", Category = CategoryName)]
[DependencyProperty<IInputElement>("LeftClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is clicked with the left mouse button.", Category = CategoryName)]
#endif
public partial class TaskbarIcon
{
}
