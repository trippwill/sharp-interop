﻿// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using SharpInterop.Exceptions;
using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.Storage.FileSystem;
using Windows.Win32.System.Console;
using Windows.Win32.UI.WindowsAndMessaging;

namespace SharpInterop;

public static partial class Interop
{
    /// <summary>
    /// Provides methods to attach to or allocate a console.
    /// </summary>
    public abstract partial class Console : IDisposable
    {
        private static readonly object _lock = new();
        private static volatile uint _refCount = 0;
        private static Console? _instance;
        private static SafeFileHandle? _inputHandle;
        private static SafeFileHandle? _outputHandle;

        /// <summary>
        /// Gets the current console instance without any safety checks.
        /// Prefer using <see cref="Attach(AttachOptions)"/> instead.
        /// </summary>
        public static Console? DangerousInstance => _instance;

        /// <summary>
        /// Gets the reference count of the console.
        /// </summary>
        public static uint RefCount => _refCount;

        /// <summary>
        /// Gets the status of the console.
        /// </summary>
        public abstract Status ConsoleStatus { get; }

        /// <summary>
        /// Gets the handle to the console input buffer.
        /// </summary>
        public virtual SafeFileHandle InputHandle => _inputHandle ??= GetConsoleInputHandle();

        /// <summary>
        /// Gets the handle to the console output buffer.
        /// </summary>
        public virtual SafeFileHandle OutputHandle => _outputHandle ??= GetConsoleOutputHandle();

        /// <summary>
        /// Forces the current process to detach from its console.
        /// Should succeed whether or not the process is attached to a console.
        /// If called after <see cref="Attach(AttachOptions)"/>,
        /// any instances of <see cref="Console"/> will be invalidated,
        /// and could cause unexpected behavior.
        /// </summary>
        /// <exception cref="Win32Exception">Thrown when the console cannot be detached. Indicates a serious interop error.</exception>
        public static void ForceDetach()
        {
            lock (_lock)
            {
                _refCount = 0;
                ReleaseConsoleResources(throwOnError: true);
            }
        }

        /// <summary>
        /// Attaches to or allocates a console based on the specified options.
        /// A process can only be attached to one console at a time. All valid instances of <see cref="Console"/> share the same console.
        /// A new console can only be attached or allocated if all valid instances of <see cref="Console"/> are disposed.
        /// </summary>
        /// <param name="options">The options specifying how to attach to or allocate the console.</param>
        /// <returns>An instance of <see cref="Console"/> representing the attached or allocated console.</returns>
        /// <remarks>
        ///     <para>
        ///         When using options that allow fallbacks or do not throw on error, check the value of <see cref="Console.Status"/> to determine the actual status of the console.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when options is null.</exception>
        /// <exception cref="ArgumentException">Thrown when options is invalid.</exception>
        /// <exception cref="ConsoleAttachException">When options allow throwing on error, thrown when the console cannot be attached or allocated.</exception>
        public static Console Attach(AttachOptions options)
        {
            ArgumentNullException.ThrowIfNull(options, nameof(options));

            lock (_lock)
            {
                Console local = options switch
                {
                    AttachOptions.AttachToProcessOrFailMode attachToProcessOrFail => AttachToProcessOrFail(attachToProcessOrFail),
                    AttachOptions.AllocateOrFailMode allocateOrFail => AllocateOrFail(allocateOrFail),
                    AttachOptions.GetOrAttachToProcessMode getOrAttachToProcess => GetOrAttachToProcess(getOrAttachToProcess),
                    AttachOptions.GetOrAllocateMode getOrAllocate => GetOrAllocate(getOrAllocate),
                    _ => throw new ArgumentException("Invalid attach mode.", nameof(options)),
                };

                if (local is Invalid invalid)
                {
                    return options.ThrowOnError
                        ? throw invalid.Exception ?? ConsoleAttachException.UnknownError()
                        : (Console)invalid;
                }

                _refCount++;
                return _instance = local;
            }

            //// *** Local functions ***

            static Console AttachToProcessOrFail(AttachOptions.AttachToProcessOrFailMode attachMode)
            {
                return _instance switch
                {
                    Attached attached when attached.ProcessId == attachMode.ProcessId => _instance,
                    null => Attach(attachMode.ProcessId, attachMode),
                    _ => new Invalid(ConsoleAttachException.AlreadyAttached),
                };
            }

            static Console AllocateOrFail(AttachOptions.AllocateOrFailMode attachMode)
            {
                return _instance switch
                {
                    Owned => _instance,
                    null => Allocate(attachMode),
                    _ => new Invalid(ConsoleAttachException.AlreadyAttached),
                };
            }

            static Console GetOrAttachToProcess(AttachOptions.GetOrAttachToProcessMode attachMode)
            {
                return _instance switch
                {
                    Attached attached when attached.ProcessId == attachMode.ProcessId => _instance,
                    SystemAssigned when attachMode.AllowSystemAssigned => _instance,
                    Owned when attachMode.AllowAllocate => _instance,
                    null => Attach(attachMode.ProcessId, attachMode),
                    _ => new Invalid(ConsoleAttachException.AlreadyAttached),
                };
            }

            static Console GetOrAllocate(AttachOptions.GetOrAllocateMode attachMode)
            {
                return _instance switch
                {
                    SystemAssigned when attachMode.AllowSystemAssigned => _instance,
                    Owned => _instance,
                    null => Allocate(attachMode),
                    _ => new Invalid(ConsoleAttachException.AlreadyAttached),
                };
            }
        }

        /// <summary>
        /// Gets a <see cref="StreamReader"/> for reading from the console output buffer.
        /// </summary>
        /// <returns>A <see cref="StreamReader"/> for reading from the console output buffer.</returns>
        public virtual StreamReader GetReader() => new(new FileStream(this.InputHandle, FileAccess.Read), leaveOpen: true);

        /// <summary>
        /// Gets a <see cref="StreamWriter"/> for writing to the console input buffer.
        /// </summary>
        /// <returns>A <see cref="StreamWriter"/> for writing to the console input buffer.</returns>
        public virtual StreamWriter GetWriter()
        {
            return new(new FileStream(this.OutputHandle, FileAccess.Write), leaveOpen: true)
            {
                AutoFlush = true,
            };
        }

        /// <summary>
        /// Enables VT100 support for the console.
        /// </summary>
        public virtual void EnableVT100Support()
        {
            PInvoke.GetConsoleMode(this.OutputHandle, out CONSOLE_MODE outMode);
            PInvoke.GetConsoleMode(this.InputHandle, out CONSOLE_MODE inMode);

            PInvoke.SetConsoleMode(this.OutputHandle, outMode | CONSOLE_MODE.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            PInvoke.SetConsoleMode(this.InputHandle, inMode | CONSOLE_MODE.ENABLE_VIRTUAL_TERMINAL_INPUT);
        }

        /// <summary>
        /// Disables VT100 support for the console.
        /// </summary>
        public virtual void DisableVT100Support()
        {
            PInvoke.GetConsoleMode(this.OutputHandle, out CONSOLE_MODE outMode);
            PInvoke.GetConsoleMode(this.InputHandle, out CONSOLE_MODE inMode);

            PInvoke.SetConsoleMode(this.OutputHandle, outMode & ~CONSOLE_MODE.ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            PInvoke.SetConsoleMode(this.InputHandle, inMode & ~CONSOLE_MODE.ENABLE_VIRTUAL_TERMINAL_INPUT);
        }

        /// <summary>
        /// Replaces the process's standard input, output, and error handles with the console's handles.
        /// Any cached references to the standard handles will be invalidated.
        /// </summary>
        public virtual void ReplaceStandardHandles()
        {
            ReplaceStandardHandle(STD_HANDLE.STD_INPUT_HANDLE, this.InputHandle);
            ReplaceStandardHandle(STD_HANDLE.STD_OUTPUT_HANDLE, this.OutputHandle);
            ReplaceStandardHandle(STD_HANDLE.STD_ERROR_HANDLE, this.OutputHandle);

            static bool ReplaceStandardHandle(STD_HANDLE handle, SafeFileHandle hHandle)
            {
                HANDLE currentHandle = PInvoke.GetStdHandle(handle);
                if (!PInvoke.SetStdHandle(handle, hHandle))
                {
                    return false;
                }

                return PInvoke.CloseHandle(currentHandle);
            }
        }

        /// <summary>
        /// Shows the console window.
        /// </summary>
        /// <returns>True if the console window was shown; otherwise, false.</returns>
        public virtual bool ShowConsoleWindow()
        {
            HWND consoleWindow = GetConsoleWindow();
            if (consoleWindow == HWND.Null)
                return false;

            PInvoke.ShowWindow(consoleWindow, SHOW_WINDOW_CMD.SW_SHOW);
            return true;
        }

        /// <summary>
        /// Hides the console window.
        /// </summary>
        /// <returns>True if the console window was hidden; otherwise, false.</returns>
        public virtual bool HideConsoleWindow()
        {
            HWND consoleWindow = GetConsoleWindow();
            if (consoleWindow == HWND.Null)
                return false;

            PInvoke.ShowWindow(consoleWindow, SHOW_WINDOW_CMD.SW_HIDE);
            return true;
        }

        /// <summary>
        /// Positions the console window at the specified coordinates and optionally resizes it.
        /// </summary>
        /// <param name="x">The x-coordinate of the console window's new position.</param>
        /// <param name="y">The y-coordinate of the console window's new position.</param>
        /// <param name="width">The new width of the console window. If null, the current width is retained.</param>
        /// <param name="height">The new height of the console window. If null, the current height is retained.</param>
        /// <param name="moveToTop">If true, moves the console window to the top of the Z order.</param>
        /// <param name="activate">If true, activates the console window.</param>
        /// <returns>True if the console window was successfully positioned; otherwise, false.</returns>
        public virtual bool PositionConsoleWindow(int x, int y, int? width = default, int? height = default, bool moveToTop = false, bool activate = false)
        {
            HWND consoleWindow = GetConsoleWindow();
            if (consoleWindow == HWND.Null)
                return false;

            if (!PInvoke.GetWindowRect(consoleWindow, out RECT rect))
                return false;

            int newWidth = width ?? rect.right - rect.left;
            int newHeight = height ?? rect.bottom - rect.top;

            SET_WINDOW_POS_FLAGS flags = activate ? SET_WINDOW_POS_FLAGS.SWP_SHOWWINDOW : SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE;
            if (!moveToTop)
                flags |= SET_WINDOW_POS_FLAGS.SWP_NOZORDER;

            HWND hWndInsertAfter = moveToTop ? HWND.HWND_TOPMOST : HWND.Null;

            return PInvoke.SetWindowPos(consoleWindow, hWndInsertAfter, x, y, newWidth, newHeight, flags);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_lock)
            {
                if (--_refCount == 0)
                {
                    ReleaseConsoleResources(throwOnError: false);
                }
            }

            GC.SuppressFinalize(this);
        }

        private static Console Attach(uint processId, AttachOptions attachMode)
        {
            if (!PInvoke.AttachConsole(processId))
            {
                WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastPInvokeError();

                // Unrecoverable errors
                if (error == WIN32_ERROR.ERROR_ACCESS_DENIED || !attachMode.AllowAnyFallback)
                {
                    return attachMode.ThrowOnError
                        ? throw ConsoleAttachException.FromPInvokeError(error)
                        : new Invalid(ConsoleAttachException.FromPInvokeError(error));
                }

                return Allocate(attachMode);
            }

            return new Attached(processId);
        }

        private static Console Allocate(AttachOptions options)
        {
            if (!PInvoke.AllocConsole())
            {
                WIN32_ERROR error = (WIN32_ERROR)Marshal.GetLastPInvokeError();

                if (error == WIN32_ERROR.ERROR_ACCESS_DENIED && options.AllowSystemAssigned)
                    return new SystemAssigned();

                return options.ThrowOnError
                    ? throw ConsoleAttachException.FromPInvokeError(error)
                    : new Invalid(ConsoleAttachException.FromPInvokeError(error));
            }

            return new Owned();
        }

        private static void ReleaseConsoleResources(bool throwOnError)
        {
            _instance = null;
            _inputHandle?.Close();
            _inputHandle = null;
            _outputHandle?.Close();
            _outputHandle = null;

            if (!PInvoke.FreeConsole() && throwOnError)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private static SafeFileHandle GetConsoleInputHandle()
        {
            SafeFileHandle handle = PInvoke.CreateFile(
                "CONIN$",
                (uint)(FILE_ACCESS_RIGHTS.FILE_GENERIC_READ | FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE),
                FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                lpSecurityAttributes: null,
                FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                new SafeFileHandle(IntPtr.Zero, ownsHandle: true));

            return handle.IsInvalid ? throw new Win32Exception(Marshal.GetLastPInvokeError()) : handle;
        }

        private static SafeFileHandle GetConsoleOutputHandle()
        {
            SafeFileHandle handle = PInvoke.CreateFile(
                "CONOUT$",
                (uint)(FILE_ACCESS_RIGHTS.FILE_GENERIC_READ | FILE_ACCESS_RIGHTS.FILE_GENERIC_WRITE),
                FILE_SHARE_MODE.FILE_SHARE_READ | FILE_SHARE_MODE.FILE_SHARE_WRITE,
                lpSecurityAttributes: null,
                FILE_CREATION_DISPOSITION.OPEN_EXISTING,
                FILE_FLAGS_AND_ATTRIBUTES.FILE_ATTRIBUTE_NORMAL,
                new SafeFileHandle(IntPtr.Zero, ownsHandle: true));

            return handle.IsInvalid ? throw new Win32Exception(Marshal.GetLastPInvokeError()) : handle;
        }

        private static HWND GetConsoleWindow() => PInvoke.GetConsoleWindow();

        /// <summary>
        /// Represents a console attached to another process.
        /// </summary>
        /// <param name="processId">The ID of the process to which the console is attached.</param>
        public class Attached(uint processId) : Console
        {
            /// <summary>
            /// Gets a value indicating whether the console is attached to the parent process.
            /// </summary>
            public bool IsAttachedToParent => processId == PInvoke.ATTACH_PARENT_PROCESS;

            /// <summary>
            /// Gets the ID of the process to which the console is attached.
            /// </summary>
            public uint ProcessId => processId;

            /// <inheritdoc/>
            public override Status ConsoleStatus => Status.Attached;
        }

        /// <summary>
        /// Represents a console owned by the current process.
        /// </summary>
        public class Owned : Console
        {
            /// <inheritdoc/>
            public override Status ConsoleStatus => Status.Owned;
        }

        /// <summary>
        /// Represents the default system assigned console.
        /// </summary>
        public class SystemAssigned : Owned
        {
            /// <inheritdoc/>
            public override Status ConsoleStatus => Status.SystemAssigned;
        }
    }
}