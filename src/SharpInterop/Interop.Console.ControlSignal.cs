// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using Windows.Win32;

namespace SharpInterop;

public static partial class Interop
{
    public abstract partial class Console
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool CtrlHandler(ControlSignal ctrlType);

        /// <summary>
        /// Specifies the type of control signals that can be sent by the console.
        /// </summary>
        public enum ControlSignal : uint
        {
            /// <summary>
            /// The CTRL+C signal.
            /// </summary>
            CTRL_C = PInvoke.CTRL_C_EVENT,

            /// <summary>
            /// The CTRL+BREAK signal.
            /// </summary>
            CTRL_BREAK = PInvoke.CTRL_BREAK_EVENT,

            /// <summary>
            /// A signal that the system sends to all processes attached to a console when the user closes the console.
            /// </summary>
            CLOSE = PInvoke.CTRL_CLOSE_EVENT,

            /// <summary>
            /// A signal that the system sends to all console processes when the user logs off.
            /// </summary>
            LOGOFF = PInvoke.CTRL_LOGOFF_EVENT,

            /// <summary>
            /// A signal that the system sends to all console processes when the system is shutting down.
            /// </summary>
            SHUTDOWN = PInvoke.CTRL_SHUTDOWN_EVENT,
        }
    }
}
