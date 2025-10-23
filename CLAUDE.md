# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

H.NotifyIcon is a cross-platform system tray icon library for .NET. It's a continuation of the hardcodet/wpf-notifyicon project, extended to support WinUI, Uno Platform, MAUI, and Console applications. The library provides native Windows tray icon functionality without relying on Windows Forms.

## Key Architecture Concepts

### Multi-Platform Structure

The codebase uses a **shared code pattern** with platform-specific implementations:

- **H.NotifyIcon.Shared/**: Contains shared source files compiled into each platform-specific library
  - Files are organized by feature: `TaskbarIcon.cs`, `TaskbarIcon.Properties.cs`, `TaskbarIcon.ContextMenu.cs`, etc.
  - Platform-specific implementations use naming conventions: `.Wpf.cs`, `.WinUI.cs`, `.WinRT.cs`
  - Conditional compilation with `#if HAS_WPF`, `#if HAS_WINUI`, `#if HAS_UNO`, `#if HAS_MAUI`

- **Platform Libraries**:
  - `H.NotifyIcon`: Core library with Windows interop via CsWin32, supports Console apps (targets net4.6.2, netstandard2.0, net8.0, net9.0)
  - `H.NotifyIcon.Wpf`: WPF wrapper with XAML integration (targets net4.6.2, net8.0-windows, net9.0-windows)
  - `H.NotifyIcon.WinUI`: WinUI 3 wrapper with WindowsAppSDK (targets net8.0-windows, net9.0-windows)
  - `H.NotifyIcon.Uno`: Uno Platform wrapper (targets net9.0, net9.0-android, net9.0-ios, net9.0-maccatalyst)
  - `H.NotifyIcon.Uno.WinUI`: Uno Platform with WinUI syntax (targets net9.0-windows10.0.19041.0)
  - `H.NotifyIcon.Maui`: MAUI wrapper (targets net9.0-android, net9.0-ios, net9.0-maccatalyst, net9.0-windows10.0.19041.0)

- **Icon Generation Libraries**:
  - `H.GeneratedIcons.System.Drawing`: Dynamic icon generation using System.Drawing
  - `H.GeneratedIcons.SkiaSharp`: Dynamic icon generation using SkiaSharp (alternative)

### Graphics Library Configuration

The `GraphicsLibrary` property in `src/libs/Directory.Build.props` controls which graphics backend is used:
- `System.Drawing` (default): Uses System.Drawing with `HAS_SYSTEM_DRAWING` define
- `SkiaSharp`: Uses SkiaSharp with `HAS_SKIA_SHARP` define

### Core Components

**TaskbarIcon** (main class split across multiple files):
- `TaskbarIcon.cs`: Core initialization and TrayIcon management
- `TaskbarIcon.Properties.cs`: Dependency properties using DependencyPropertyGenerator
- `TaskbarIcon.ContextMenu.*.cs`: Platform-specific context menu implementations
- `TaskbarIcon.MouseEvents.cs`, `TaskbarIcon.KeyboardEvents.cs`: Input handling
- `TaskbarIcon.Notifications.cs`: Balloon notifications
- `TaskbarIcon.IconSource.cs`: Icon source binding and conversion

**TrayIcon** (in H.NotifyIcon core):
- Low-level Windows Shell_NotifyIcon API wrapper via CsWin32
- Manages NOTIFYICONDATA structure and message window

**GeneratedIconSource**:
- Allows dynamic icon generation from text, emojis, or graphics
- Cross-platform implementations for WPF and WinRT

### Source Generators

The project heavily uses source generators:
- **DependencyPropertyGenerator**: Generates WPF/WinUI dependency properties from attributes
- **EventGenerator**: Generates event infrastructure
- **CsWin32**: Generates P/Invoke code from Windows metadata

## Build and Development

### Prerequisites

- .NET 9 SDK
- MAUI workload: `dotnet workload install maui`
- Tizen workload (Windows): Run the workload-install.ps1 script from Samsung Tizen.NET repo

### Building

Build all libraries:
```powershell
Get-ChildItem -Path src/libs -Recurse -Filter *.csproj | ForEach-Object { dotnet build $_.FullName --configuration Release }
```

Build specific platform library:
```bash
dotnet build src/libs/H.NotifyIcon.Wpf/H.NotifyIcon.Wpf.csproj --configuration Release
```

Build core library only:
```bash
dotnet build src/libs/H.NotifyIcon/H.NotifyIcon.csproj --configuration Release
```

### Testing

Run integration tests (Windows only):
```bash
dotnet test src/tests/H.NotifyIcon.IntegrationTests/H.NotifyIcon.IntegrationTests.csproj
```

Tests target both net4.8 and net9.0-windows frameworks.

### Running Sample Apps

Sample applications demonstrate platform-specific usage:

WPF app:
```bash
dotnet run --project src/apps/H.NotifyIcon.Apps.Wpf/H.NotifyIcon.Apps.Wpf.csproj
```

WinUI app (requires x64/x86/arm64 RID):
```bash
dotnet run --project src/apps/H.NotifyIcon.Apps.WinUI/H.NotifyIcon.Apps.WinUI.csproj
```

Console app:
```bash
dotnet run --project src/apps/H.NotifyIcon.Apps.Console/H.NotifyIcon.Apps.Console.csproj
```

MAUI app:
```bash
dotnet build src/apps/H.NotifyIcon.Apps.Maui/H.NotifyIcon.Apps.Maui.csproj -f net9.0-windows10.0.19041.0
```

## Code Patterns and Conventions

### Conditional Compilation

Platform-specific code uses consistent defines:
- `HAS_WPF`: WPF-specific code
- `HAS_WINUI`: WinUI 3-specific code
- `HAS_UNO`: Uno Platform-specific code
- `HAS_MAUI`: MAUI-specific code
- `HAS_SYSTEM_DRAWING`: System.Drawing graphics backend
- `HAS_SKIA_SHARP`: SkiaSharp graphics backend

### Assembly Signing

All library projects are signed with `src/libs/key.snk` (except Uno which has `SignAssembly>false`).

### NuGet Packaging

- Packages are automatically generated on Release builds
- Versioning uses MinVer with git tags (prefix: `v`)
- All packages include `assets/nuget_icon.png` and `README.md`

### Trimming and AOT

Libraries are trim-compatible and AOT-compatible (net7.0+):
- `IsTrimmable>true`
- `IsAotCompatible>true` (net7.0+)
- Use `[DynamicDependency]` attributes for reflection code (see recent commit about ToggleMenuFlyoutItem)

### File Exclusion Patterns

Platform-specific files are excluded from builds using MSBuild conditions in project files:
- `*.ios.cs`, `ios/**/*.cs` - iOS-specific
- `*.macos.cs`, `macos/**/*.cs` - macOS-specific
- `*.android.cs`, `android/**/*.cs` - Android-specific
- `*.windows.cs`, `windows/**/*.cs` - Windows-specific
- `*.net.cs` - Modern .NET only (net6.0, net8.0, etc.)

## Context Menu Behavior

### WPF
Uses native WPF ContextMenu with full feature support.

### WinUI/Uno
Three context menu modes (controlled by `ContextMenuMode` property):
1. **PopupMenu** (default): Converts MenuFlyout to Win32 popup menu, calls commands on selection
2. **ActiveWindow**: Renders menu in corner of active window
3. **SecondWindow**: Creates transparent secondary window for native menu rendering (preview, requires WindowsAppSDK 1.1.0+)

Packaged vs unpackaged apps have different capabilities for transparency, borderless windows, and animations.

## Important Implementation Details

### GUID Management
Each TrayIcon uses a GUID to persist settings across sessions. Windows associates the GUID with the executable path. When the executable path changes, Windows displays the old icon. Solutions:
- Use fixed GUID via `CustomName` property
- Use Authenticode code signing (Windows associates GUID with certificate)

### Efficiency Mode Integration
The library provides `EfficiencyModeUtilities.SetEfficiencyMode(bool)` for Windows 11 Efficiency Mode (EcoQoS). WindowExtensions automatically manage efficiency mode on Show/Hide.

### Design-Time Support
TrayIcons appear in the Windows system tray even at design time. Pin the designer icon in "Taskbar Settings → Other system tray icons" for easier viewing.

## Directory Structure

```
src/
├── libs/              # Library projects
│   ├── H.NotifyIcon.Shared/         # Shared source files
│   ├── H.NotifyIcon/                # Core library (Windows interop)
│   ├── H.NotifyIcon.Wpf/            # WPF wrapper
│   ├── H.NotifyIcon.WinUI/          # WinUI 3 wrapper
│   ├── H.NotifyIcon.Uno/            # Uno Platform wrapper
│   ├── H.NotifyIcon.Uno.WinUI/      # Uno Platform WinUI syntax
│   ├── H.NotifyIcon.Maui/           # MAUI wrapper
│   ├── H.GeneratedIcons.System.Drawing/
│   └── H.GeneratedIcons.SkiaSharp/
├── apps/              # Sample applications
│   ├── H.NotifyIcon.Apps.Wpf/
│   ├── H.NotifyIcon.Apps.WinUI/
│   ├── H.NotifyIcon.Apps.Console/
│   ├── H.NotifyIcon.Apps.Maui/
│   └── TrimmingHelper/              # AOT/trimming validation
└── tests/
    └── H.NotifyIcon.IntegrationTests/
```

## CI/CD

GitHub Actions workflow (`.github/workflows/dotnet.yml`):
- Runs on: Windows (due to WinUI/WPF dependencies)
- Triggers: Push to master, tags matching `v**`
- Steps: Install MAUI workload, install Tizen workload, build all libraries, publish to NuGet

## Notes for Code Changes

- When adding new dependency properties, use `[DependencyProperty<T>]` attribute in partial class
- When adding platform-specific code, use appropriate file naming (`.Wpf.cs`, `.WinUI.cs`, etc.) and place in Shared folder
- For trimming-safe reflection, add `[DynamicDependency]` attributes
- When modifying icon generation, consider both System.Drawing and SkiaSharp implementations
- Test on multiple target frameworks (net4.6.2, netstandard2.0, net8.0, net9.0)
