namespace H.NotifyIcon;

[DependencyProperty<bool>("NoLeftClickDelay",
    Description = "Set to true to make left clicks work without delay.", Category = CategoryName)]
[DependencyProperty<ICommand>("DoubleClickCommand",
    Description = "A command that is being executed if the tray icon is being double-clicked.", Category = CategoryName)]
[DependencyProperty<object>("DoubleClickCommandParameter",
    Description = "Parameter to submit to the DoubleClickCommand when the user double clicks on the NotifyIcon.", Category = CategoryName)]
[DependencyProperty<ICommand>("LeftClickCommand",
    Description = "A command that is being executed if the tray icon is being left-clicked.", Category = CategoryName)]
[DependencyProperty<object>("LeftClickCommandParameter",
    Description = "The target of the command that is fired if the notify icon is clicked with the left mouse button.", Category = CategoryName)]
[DependencyProperty<ICommand>("RightClickCommand",
    Description = "A command that is being executed if the tray icon is being right-clicked.", Category = CategoryName)]
[DependencyProperty<object>("RightClickCommandParameter",
    Description = "The target of the command that is fired if the notify icon is clicked with the right mouse button.", Category = CategoryName)]
[DependencyProperty<ICommand>("MiddleClickCommand",
    Description = "A command that is being executed if the tray icon is being middle-clicked.", Category = CategoryName)]
[DependencyProperty<object>("MiddleClickCommandParameter",
    Description = "The target of the command that is fired if the notify icon is clicked with the middle mouse button.", Category = CategoryName)]
#if HAS_WPF
[DependencyProperty<IInputElement>("DoubleClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is double clicked.", Category = CategoryName)]
[DependencyProperty<IInputElement>("LeftClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is clicked with the left mouse button.", Category = CategoryName)]
[DependencyProperty<IInputElement>("RightClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is clicked with the right mouse button.", Category = CategoryName)]
[DependencyProperty<IInputElement>("MiddleClickCommandTarget",
    Description = "The target of the command that is fired if the notify icon is clicked with the middle mouse button.", Category = CategoryName)]
#endif
public partial class TaskbarIcon
{
}
