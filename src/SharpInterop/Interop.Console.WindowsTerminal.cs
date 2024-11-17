// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace SharpInterop;

public static partial class Interop
{
    public abstract partial class Console
    {
        private const string WindowsTerminalClassName = "CASCADIA_HOSTING_WINDOW_CLASS";

        /// <summary>
        /// Determines if the console window is an instance of Windows Terminal.
        /// </summary>
        /// <returns>True if the console window is an instance of Windows Terminal; otherwise, false.</returns>
        internal static unsafe bool IsWindowsTerminal(HWND consoleWindow)
        {
            if (consoleWindow == HWND.Null)
                return false;

            char* className = stackalloc char[256];

            if (PInvoke.GetClassName(consoleWindow, className, 256) == 0)
                return false;

            string? classNameString = Marshal.PtrToStringAuto((IntPtr)className, 256);

            return classNameString == WindowsTerminalClassName;
        }
    }
}
