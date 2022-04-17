## H.NotifyIcon

** This project is a continuation of the inactive base project to 
other platforms (WinUI/Uno.Skia.Wpf/Console) - ❤️ https://github.com/hardcodet/wpf-notifyicon ❤️ **

This is an implementation of a NotifyIcon (aka system tray icon or taskbar icon)
for .Net Core 3.1/.Net 5/.Net 6 WPF/WinUI/Uno.Skia.WPF/Console platforms.
It does not just rely on the Windows Forms NotifyIcon component, 
but is a purely independent control which leverages several features of the WPF/WinUI framework 
in order to display rich tooltips, popups, context menus, and balloon messages. 
It can be used directly in code or embedded in any XAML file.

### Features

- Notifications
- Context menus
- ICommand support
- Design-time access
- Dynamic icon generation

### Nuget

[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.Wpf.svg?style=flat-square&label=H.NotifyIcon.Wpf)](https://www.nuget.org/packages/H.NotifyIcon.Wpf/)
[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.WinUI.svg?style=flat-square&label=H.NotifyIcon.WinUI)](https://www.nuget.org/packages/H.NotifyIcon.WinUI/)
[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.Uno.svg?style=flat-square&label=H.NotifyIcon.Uno)](https://www.nuget.org/packages/H.NotifyIcon.Uno/)
[![Nuget](https://img.shields.io/nuget/dt/H.NotifyIcon.Uno.WinUI.svg?style=flat-square&label=H.NotifyIcon.Uno.WinUI)](https://www.nuget.org/packages/H.NotifyIcon.Uno.WinUI/)

```
Install-Package H.NotifyIcon.Wpf
Install-Package H.NotifyIcon.WinUI
Install-Package H.NotifyIcon.Uno
Install-Package H.NotifyIcon.Uno.WinUI
// If you need other platforms, you can use this Core library - 
// it allows you to make NotifyIcon even in a console application.
Install-Package H.NotifyIcon
```

### Usage

```xml
<Window
    xmlns:tb="http://www.hardcodet.net/taskbar" // WPF
    xmlns:tb="using:H.NotifyIcon" // WinUI
    >
    <tb:TaskbarIcon
        ToolTipText="ToolTip"
        IconSource="/Images/TrayIcons/Logo.ico"
        ContextMenu="{StaticResource TrayMenu}"
        MenuActivation="LeftOrRightClick"
        TrayPopup="{StaticResoure TrayStatusPopup}"
        PopupActivation="DoubleClick"
        TrayToolTip="{StaticResource TrayToolTip}"
        />
</Window>
```

### Generated icons
Simple example: <img width="24" alt="image" src="https://user-images.githubusercontent.com/3002068/163700914-ad61c54c-444c-485c-92df-73be5412109d.png">
```xml
<tb:TaskbarIcon GeneratedIconText="❤️" GeneratedIconForeground="Red">
```
Advanced example: <img width="19" alt="image" src="https://user-images.githubusercontent.com/3002068/163702368-da5031c6-efa7-4c83-85b5-a0a3eaf8a0ca.png">
```xml
<tb:TaskbarIcon
    IconSource="/Icons/Error.ico"
    GeneratedIconText="5"
    GeneratedIconForeground="Black"
    GeneratedIconFontSize="9"
    GeneratedIconFontWeight="Bold"
    >
```


### Design-Time Access
It is recommended to pin the designer icon for easy viewing. To do this, go to Taskbar Settings -> Other system tray icons and enable this icon:
<img width="412" alt="image" src="https://user-images.githubusercontent.com/3002068/163700588-eb2ad5f2-45d0-4b6f-ad39-c66f96202cb5.png">


### [Sample Apps](https://github.com/HavenDV/H.NotifyIcon/tree/master/src/apps)
The minimum supported version of the .Net Framework is 4.5.1.  
So in some cases to build the project you will need to install this -  
https://dotnet.microsoft.com/en-us/download/dotnet-framework/thank-you/net451-developer-pack-offline-installer

### Contacts
* [mail](mailto:havendv@gmail.com)
* Discord: Haven#5924
