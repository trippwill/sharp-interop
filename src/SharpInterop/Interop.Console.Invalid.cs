// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;

namespace SharpInterop;

public static partial class Interop
{
    public abstract partial class Console
    {
        /// <summary>
        /// Represents an invalid console.
        /// </summary>
        public class Invalid(Exception? exception) : Console
        {
            /// <inheritdoc/>
            public override Status ConsoleStatus => Status.Invalid;

            /// <inheritdoc/>
            public override SafeFileHandle InputHandle => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override SafeFileHandle OutputHandle => throw this.InvalidConsoleException();

            /// <summary>
            /// Gets the exception that caused the console to be invalid.
            /// </summary>
            public Exception? Exception => exception;

            /// <inheritdoc/>
            public override StreamReader GetReader() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override StreamWriter GetWriter() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override void Write(string value) => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override void WriteLine(string value) => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override int WaitForReadKey() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override void EnableVT100Support() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override void DisableVT100Support() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override void RestoreVT100Support() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override bool HideConsoleWindow() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override bool ShowConsoleWindow() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override bool PositionConsoleWindow(int x, int y, int? width = null, int? height = null, bool moveToTop = false, bool activate = false)
            {
                throw this.InvalidConsoleException();
            }

            /// <inheritdoc/>
            public override nint AddControlHandler(ControlSignal signal, Action handler)
            {
                throw this.InvalidConsoleException();
            }

            /// <inheritdoc/>
            public override void RemoveControlHandler(nint controlHandler)
            {
                throw this.InvalidConsoleException();
            }

            /// <inheritdoc/>
            public override void IgnoreCtrlC() => throw this.InvalidConsoleException();

            /// <inheritdoc/>
            public override void RestoreCtrlC() => throw this.InvalidConsoleException();

            private InvalidOperationException InvalidConsoleException() => new("The console is invalid.", this.Exception);
        }
    }
}
