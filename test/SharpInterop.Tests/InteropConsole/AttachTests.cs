// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics;
using SharpInterop;
using SharpInterop.Exceptions;
using Xunit;

using AttachOptions = SharpInterop.Interop.Console.AttachOptions;

namespace InteropConsole;

public class AttachTests
{
    private const string ProcessName = "conhost";

    [Fact]
    public void Attach_WithNullOptions_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Interop.Console.Attach(null!));
    }

    [Fact]
    public void Attach_WithInvalidOptions_ThrowsArgumentException()
    {
        InvalidAttachOptions invalidOptions = new();
        Assert.Throws<ArgumentException>(() => Interop.Console.Attach(invalidOptions));
    }

    [Fact]
    public void Attach_AttachToProcessOrFailMode_ReturnsConsoleInstance()
    {
        Interop.Console.ForceDetach();

        using Process process = Process.Start(ProcessName);
        try
        {
            AttachOptions options = AttachOptions.AttachToProcessOrFail(process.Id);
            using Interop.Console console = Interop.Console.Attach(options);
            Assert.NotNull(console);
        }
        finally
        {
            process.Kill();
        }
    }

    [Fact]
    public void Attach_AttachToProcessOrFailMode_ThrowsConsoleException()
    {
        Interop.Console.ForceDetach();

        using Process process = Process.Start(ProcessName);
        try
        {
            AttachOptions options = AttachOptions.AttachToProcessOrFail(process.Id);
            Interop.Console.Attach(options);
            Assert.Throws<ConsoleAttachException>(() => Interop.Console.Attach(options));
        }
        finally
        {
            process.Kill();
        }
    }

    [Fact]
    public void Attach_AllocateOrFailMode_ReturnsConsoleInstance()
    {
        Interop.Console.ForceDetach();

        AttachOptions options = AttachOptions.AllocateOrFail();
        using Interop.Console console = Interop.Console.Attach(options);
        Assert.NotNull(console);
    }

    [Fact(Skip = "TODO: Investigate")]
    public void Attach_AllocateOrFailMode_ThrowsConsoleException()
    {
        Interop.Console.ForceDetach();

        using Process process = Process.Start(ProcessName);
        try
        {
            AttachOptions options = AttachOptions.GetOrAttachToProcess(process.Id, Interop.Console.AttachFlags.Default);
            using (Interop.Console c = Interop.Console.Attach(options))
            {
                options = AttachOptions.AllocateOrFail();
                Assert.Throws<ConsoleAttachException>(() => Interop.Console.Attach(options));
            }
        }
        finally
        {
            process.Kill();
        }
    }

    [Fact]
    public void Attach_GetOrAttachToProcessMode_ReturnsConsoleInstance()
    {
        Interop.Console.ForceDetach();
        using Process process = Process.Start(ProcessName);
        try
        {
            AttachOptions options = AttachOptions.GetOrAttachToProcess(process.Id);
            using Interop.Console console = Interop.Console.Attach(options);
            Assert.NotNull(console);
        }
        finally
        {
            process.Kill();
        }
    }

    [Fact(Skip = "TODO: Investigate")]
    public void Attach_GetOrAttachToProcessMode_ThrowsConsoleException()
    {
        Interop.Console.ForceDetach();

        using Process process = Process.Start(ProcessName);
        try
        {
            AttachOptions options = AttachOptions.GetOrAttachToProcess(process.Id);
            using (Interop.Console c = Interop.Console.Attach(options))
            {
                options = AttachOptions.GetOrAttachToProcess(-2);
                Assert.Throws<ConsoleAttachException>(() => Interop.Console.Attach(options));
            }
        }
        finally
        {
            process.Kill();
        }
    }

    [Fact]
    public void Attach_GetOrAttachToProcessMode_ReturnsSameConsoleInstance()
    {
        Interop.Console.ForceDetach();
        using Process process = Process.Start(ProcessName);
        try
        {
            AttachOptions options = AttachOptions.GetOrAttachToProcess(process.Id);
            using Interop.Console console1 = Interop.Console.Attach(options);
            using Interop.Console console2 = Interop.Console.Attach(options);
            Assert.Same(console1, console2);
        }
        finally
        {
            process.Kill();
        }
    }

    [Fact]
    public void Attach_GetOrAllocateMode_ReturnsConsoleInstance()
    {
        Interop.Console.ForceDetach();

        AttachOptions options = AttachOptions.GetOrAllocate();
        using Interop.Console console = Interop.Console.Attach(options);
        Assert.NotNull(console);
    }

    [Fact]
    public void Attach_GetOrAllocateMode_ReturnsSameConsoleInstance()
    {
        Interop.Console.ForceDetach();

        AttachOptions options = AttachOptions.GetOrAllocate();
        using Interop.Console console1 = Interop.Console.Attach(options);
        using Interop.Console console2 = Interop.Console.Attach(options);
        Assert.Same(console1, console2);
    }

    [Fact]
    public void Attach_GetOrAllocateMode_ReturnsSystemAssignedInstance()
    {
        AttachOptions options = AttachOptions.GetOrAllocate();
        using Interop.Console console = Interop.Console.Attach(options);
        Assert.IsType<Interop.Console.SystemAssigned>(console);
    }

    [Fact]
    public void ForceDetach_SetsRefCountToZero()
    {
        Interop.Console.ForceDetach();
        Assert.Equal(0u, Interop.Console.RefCount);
    }

    [Fact]
    public void ForceDetach_InvalidatesInstance()
    {
        Interop.Console.ForceDetach();
        Assert.Null(Interop.Console.DangerousInstance);
    }

    private record InvalidAttachOptions : AttachOptions;
}
