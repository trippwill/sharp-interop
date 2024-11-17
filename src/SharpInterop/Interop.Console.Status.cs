// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SharpInterop;

public static partial class Interop
{
public abstract partial class Console
    {
        /// <summary>
        /// Specifies the status of the console.
        /// </summary>
        public enum Status
        {
            /// <summary>
            /// The console is invalid.
            /// </summary>
            Invalid = 0,

            /// <summary>
            /// The console is the default system assigned console.
            /// </summary>
            SystemAssigned,

            /// <summary>
            /// The console is attached to a different process.
            /// </summary>
            Attached,

            /// <summary>
            /// The console is owned by the current process.
            /// </summary>
            Owned,
        }
    }
}
