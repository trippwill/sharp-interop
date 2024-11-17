// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.Foundation;

namespace SharpInterop.Exceptions;

/// <summary>
/// Exception thrown when there is an error attaching to a console.
/// </summary>
public class ConsoleAttachException : InvalidOperationException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleAttachException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    internal ConsoleAttachException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }

    internal static ConsoleAttachException AlreadyAttached => new("The current process is already attached to a console.");

    internal static ConsoleAttachException UnknownError(Exception? inner = null) => new("An unknown error occurred while attaching to the console.", inner);

    /// <summary>
    /// Creates a new instance of the <see cref="ConsoleAttachException"/> class based on a PInvoke error.
    /// </summary>
    /// <param name="error">The PInvoke error code.</param>
    /// <returns>A new instance of the <see cref="ConsoleAttachException"/> class.</returns>
    internal static ConsoleAttachException FromPInvokeError(WIN32_ERROR error)
    {
        Win32Exception inner = new((int)error, Marshal.GetLastPInvokeErrorMessage());

        return error switch
        {
            WIN32_ERROR.ERROR_ACCESS_DENIED => new ConsoleAttachException(
                "The current process is already attached to a console.",
                inner),
            WIN32_ERROR.ERROR_INVALID_HANDLE => new ConsoleAttachException(
                "There is no console to attach to.",
                inner),
            WIN32_ERROR.ERROR_INVALID_PARAMETER => new ConsoleAttachException(
                "The specified process ID is not valid.",
                inner),
            _ => new ConsoleAttachException(
                "An unknown error occurred while attaching to the console.",
                inner),
        };
    }
}
