// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.Win32;

namespace SharpInterop;

public static partial class Interop
{
    public partial class Console
    {
        /// <summary>
        /// Flags to control the behavior of attaching to or allocating a console.
        /// </summary>
        [Flags]
        public enum AttachFlags : ushort
        {
            /// <summary>
            /// No fallback are allowed, and exceptions are thrown on error.
            /// </summary>
            Default = 0x000,

            /// <summary>
            /// Allow fallback to an allocated console if attaching fails.
            /// </summary>
            AllowFallbackToAllocated = 0x001,

            /// <summary>
            /// Allow fallback to a system-assigned console if attaching fails.
            /// </summary>
            AllowFallbackToSystemAssigned = 0x0010,

            /// <summary>
            /// Allow fallback to any available console if attaching fails.
            /// </summary>
            AllowAllFallbacks = AllowFallbackToAllocated | AllowFallbackToSystemAssigned,

            /// <summary>
            /// Return a <see cref="Console.Invalid"/> instead of throwing an exception on error.
            /// </summary>
            DoNotThrowOnError = 0x0100,
        }

        /// <summary>
        /// Represents different modes for attaching to or allocating a console.
        /// </summary>
        public abstract record AttachOptions
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AttachOptions"/> class with default settings.
            /// </summary>
            protected AttachOptions()
                : this(AttachFlags.Default)
            {
            }

            /// <summary>
            /// Initializes a new instance of the <see cref="AttachOptions"/> class.
            /// </summary>
            /// <param name="flags">Flags to control attach behavior.</param>
            private protected AttachOptions(AttachFlags flags) => this.Flags = flags;

            internal AttachFlags Flags { get; }

            internal bool ThrowOnError => (this.Flags & AttachFlags.DoNotThrowOnError) == 0;

            internal bool AllowAllocate => (this.Flags & AttachFlags.AllowFallbackToAllocated) != 0;

            internal bool AllowSystemAssigned => (this.Flags & AttachFlags.AllowFallbackToSystemAssigned) != 0;

            internal bool AllowAnyFallback => (this.Flags & (AttachFlags.AllowFallbackToAllocated | AttachFlags.AllowFallbackToSystemAssigned)) != 0;

            /// <summary>
            /// Creates options to attach to a specific process or fail if unable to attach.
            /// </summary>
            /// <param name="processId">The ID of the process to attach to.</param>
            /// <param name="throwOnError">Whether to throw an exception on error.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions AttachToProcessOrFail(uint processId, bool throwOnError = true)
                => new AttachToProcessOrFailMode(processId, throwOnError);

            /// <summary>
            /// Creates options to attach to a specific process or fail if unable to attach.
            /// </summary>
            /// <param name="processId">The ID of the process to attach to.</param>
            /// <param name="throwOnError">Whether to throw an exception on error.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions AttachToProcessOrFail(int processId, bool throwOnError = true)
                => AttachToProcessOrFail(checked((uint)processId), throwOnError);

            /// <summary>
            /// Creates options to attach to the parent process or fail if unable to attach.
            /// </summary>
            /// <param name="throwOnError">Whether to throw an exception on error.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions AttachToParentOrFail(bool throwOnError = true)
                => AttachToProcessOrFail(PInvoke.ATTACH_PARENT_PROCESS, throwOnError);

            /// <summary>
            /// Creates options to allocate a new console or fail if unable to allocate.
            /// </summary>
            /// <param name="throwOnError">Whether to throw an exception on error.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions AllocateOrFail(bool throwOnError = true)
                => new AllocateOrFailMode(throwOnError);

            /// <summary>
            /// Creates options to get the current console or attach to a specific process, optionally allocating a new console if unable to attach.
            /// </summary>
            /// <param name="processId">The ID of the process to attach to.</param>
            /// <param name="flags">Flags to control attach behavior.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions GetOrAttachToProcess(uint processId, AttachFlags flags = AttachFlags.AllowAllFallbacks)
                => new GetOrAttachToProcessMode(processId, flags);

            /// <summary>
            /// Creates options to get the current console or attach to a specific process, optionally allocating a new console if unable to attach.
            /// </summary>
            /// <param name="processId">The ID of the process to attach to.</param>
            /// <param name="flags">Flags to control attach behavior.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions GetOrAttachToProcess(int processId, AttachFlags flags = AttachFlags.AllowAllFallbacks)
                => GetOrAttachToProcess(checked((uint)processId), flags);

            /// <summary>
            /// Creates options to get the current console or attach to the parent process, optionally allocating a new console if unable to attach.
            /// </summary>
            /// <param name="flags">Flags to control attach behavior.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions GetOrAttachToParent(AttachFlags flags = AttachFlags.AllowAllFallbacks)
                => GetOrAttachToProcess(PInvoke.ATTACH_PARENT_PROCESS, flags);

            /// <summary>
            /// Creates options to get the current console or allocate a new one if none exists.
            /// </summary>
            /// <param name="flags">Flags to control attach behavior.</param>
            /// <returns>A <see cref="AttachOptions"/> instance.</returns>
            public static AttachOptions GetOrAllocate(AttachFlags flags = AttachFlags.AllowAllFallbacks) => new GetOrAllocateMode(flags);

            internal record AttachToProcessOrFailMode(uint ProcessId, bool ThrowOnError)
                : AttachOptions(ThrowOnError ? AttachFlags.Default : AttachFlags.DoNotThrowOnError);

            internal record AllocateOrFailMode(bool ThrowOnError)
                : AttachOptions(ThrowOnError ? AttachFlags.Default : AttachFlags.Default);

            internal record GetOrAttachToProcessMode(uint ProcessId, AttachFlags Flags) : AttachOptions(Flags);

            internal record GetOrAllocateMode(AttachFlags Flags) : AttachOptions(Flags);
        }
    }
}
