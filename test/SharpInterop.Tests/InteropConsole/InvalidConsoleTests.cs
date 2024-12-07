// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SharpInterop;
using Xunit;

namespace InteropConsole;

public class InvalidConsoleTests
{
    [Fact]
    public void InvalidConsole_Constructor_SetsException()
    {
        InvalidOperationException exception = new InvalidOperationException("Test exception");
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(exception);

        Assert.Equal(exception, invalidConsole.Exception);
    }

    [Fact]
    public void InvalidConsole_ConsoleStatus_IsInvalid()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Equal(Interop.Console.Status.Invalid, invalidConsole.ConsoleStatus);
    }

    [Fact]
    public void InvalidConsole_InputHandle_ThrowsInvalidOperationException()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Throws<InvalidOperationException>(() => invalidConsole.InputHandle);
    }

    [Fact]
    public void InvalidConsole_OutputHandle_ThrowsInvalidOperationException()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Throws<InvalidOperationException>(() => invalidConsole.OutputHandle);
    }

    [Fact]
    public void InvalidConsole_GetReader_ThrowsInvalidOperationException()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Throws<InvalidOperationException>(() => invalidConsole.GetReader());
    }

    [Fact]
    public void InvalidConsole_GetWriter_ThrowsInvalidOperationException()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Throws<InvalidOperationException>(() => invalidConsole.GetWriter());
    }

    [Fact]
    public void InvalidConsole_EnableVT100Support_ThrowsInvalidOperationException()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Throws<InvalidOperationException>(() => invalidConsole.EnableVT100Support());
    }

    [Fact]
    public void InvalidConsole_HideConsoleWindow_ThrowsInvalidOperationException()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Throws<InvalidOperationException>(() => invalidConsole.HideConsoleWindow());
    }

    [Fact]
    public void InvalidConsole_ShowConsoleWindow_ThrowsInvalidOperationException()
    {
        Interop.Console.Invalid invalidConsole = new Interop.Console.Invalid(null);

        Assert.Throws<InvalidOperationException>(() => invalidConsole.ShowConsoleWindow());
    }
}
