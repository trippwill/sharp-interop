// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SharpInterop;

/// <summary>
/// Provides VT100 escape codes for text formatting, cursor control, and screen control.
/// The class is partial to allow for additional escape codes to be added in other files.
/// </summary>
public static partial class VT100Code
{
    // ** Text colors **

    /// <summary>Sets text color to red.</summary>
    public const string Red = "\u001b[31m";

    /// <summary>Sets text color to green.</summary>
    public const string Green = "\u001b[32m";

    /// <summary>Sets text color to yellow.</summary>
    public const string Yellow = "\u001b[33m";

    /// <summary>Sets text color to blue.</summary>
    public const string Blue = "\u001b[34m";

    /// <summary>Sets text color to magenta.</summary>
    public const string Magenta = "\u001b[35m";

    /// <summary>Sets text color to cyan.</summary>
    public const string Cyan = "\u001b[36m";

    /// <summary>Sets text color to white.</summary>
    public const string White = "\u001b[37m";

    // ** Text styles **

    /// <summary>Resets all text attributes.</summary>
    public const string Reset = "\u001b[0m";

    /// <summary>Sets text to bold.</summary>
    public const string Bold = "\u001b[1m";

    /// <summary>Sets text to underline.</summary>
    public const string Underline = "\u001b[4m";

    /// <summary>Reverses the foreground and background colors.</summary>
    public const string Reversed = "\u001b[7m";

    /// <summary>Sets text to blink.</summary>
    public const string Blink = "\u001b[5m";

    // ** Screen control **

    /// <summary>Clears the entire screen.</summary>
    public const string ClearScreen = "\u001b[2J";

    /// <summary>Clears the current line.</summary>
    public const string ClearLine = "\u001b[2K";

    // ** Cursor control **

    /// <summary>Moves the cursor up by the specified number of lines.</summary>
    /// <param name="n">The number of lines to move the cursor up.</param>
    /// <returns>The escape code to move the cursor up.</returns>
    public static string MoveCursorUp(int n) => $"\u001b[{n}A";

    /// <summary>Moves the cursor down by the specified number of lines.</summary>
    /// <param name="n">The number of lines to move the cursor down.</param>
    /// <returns>The escape code to move the cursor down.</returns>
    public static string MoveCursorDown(int n) => $"\u001b[{n}B";

    /// <summary>Moves the cursor forward by the specified number of columns.</summary>
    /// <param name="n">The number of columns to move the cursor forward.</param>
    /// <returns>The escape code to move the cursor forward.</returns>
    public static string MoveCursorForward(int n) => $"\u001b[{n}C";

    /// <summary>Moves the cursor backward by the specified number of columns.</summary>
    /// <param name="n">The number of columns to move the cursor backward.</param>
    /// <returns>The escape code to move the cursor backward.</returns>
    public static string MoveCursorBackward(int n) => $"\u001b[{n}D";

    /// <summary>Moves the cursor to the specified row and column.</summary>
    /// <param name="row">The row to move the cursor to.</param>
    /// <param name="col">The column to move the cursor to.</param>
    /// <returns>The escape code to move the cursor to the specified position.</returns>
    public static string MoveCursorTo(int row, int col) => $"\u001b[{row};{col}H";

#pragma warning disable SA1201 // Elements should appear in the correct order

    /// <summary>Hides the cursor.</summary>
    public const string HideCursor = "\u001b[?25l";

    /// <summary>Shows the cursor.</summary>
    public const string ShowCursor = "\u001b[?25h";

    /// <summary>Saves the current cursor position.</summary>
    public const string SaveCursorPosition = "\u001b[s";

    /// <summary>Restores the cursor to the last saved position.</summary>
    public const string RestoreCursorPosition = "\u001b[u";

#pragma warning restore SA1201 // Elements should appear in the correct order

    // ** OSC (Operating System Command) control **

    /// <summary>Sets the terminal window title.</summary>
    /// <param name="title">The title to set.</param>
    /// <returns>The escape code to set the terminal window title.</returns>
    public static string SetTitle(string title) => $"\u001b]0;{title}\u0007";
}
