﻿namespace H.NotifyIcon.Core;

/// <summary>
/// Possible keyboard events.
/// </summary>
public enum KeyboardEvent
{
    /// <summary>
    /// The icon was selected with the keyboard
    /// and Shift-F10 was pressed.
    /// </summary>
    ContextMenu,

    /// <summary>
    /// The icon was selected with the keyboard
    /// and activated with Spacebar or Enter.
    /// </summary>
    KeySelect,

    /// <summary>
    /// The icon was selected with the mouse
    /// and activated with Enter.
    /// </summary>
    Select,
}