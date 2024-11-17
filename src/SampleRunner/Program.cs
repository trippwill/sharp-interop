// Copyright (c) Contributors to the SharpInterop project. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using SharpInterop;

Console.WriteLine("Hello, World!");

Interop.Console.ForceDetach();

using (Interop.Console console = Interop.Console.Attach(Interop.Console.AttachOptions.GetOrAttachToParent()))
{
    console.EnableVT100Support();

    using (StreamWriter writer = console.GetWriter())
    {
        writer.Write(VT100Code.Green);
        writer.Write(VT100Code.Bold);
        writer.Write(VT100Code.Reversed);
        writer.WriteLine("Hello, World!");
        writer.Write(VT100Code.Reset);

        writer.WriteLine("Any key to continue...");
    }

    using (StreamReader reader = console.GetReader())
    {
        while (reader.Read() == -1)
        {
        }
    }

    using (StreamWriter writer = console.GetWriter())
    {
        writer.Write(VT100Code.Red);
        writer.Write(VT100Code.Bold);
        writer.Write(VT100Code.Reversed);
        writer.WriteLine("Hello, World!");
        writer.Write(VT100Code.Reset);

        writer.WriteLine("Any key to continue...");
    }

    using (StreamReader reader = console.GetReader())
    {
        while (reader.Read() == -1)
        {
        }
    }
}

using (Interop.Console console = Interop.Console.Attach(Interop.Console.AttachOptions.GetOrAllocate()))
{
    console.DisableVT100Support();

    using (StreamWriter writer = console.GetWriter())
    {
        writer.Write(VT100Code.Blue);
        writer.Write(VT100Code.Bold);
        writer.Write(VT100Code.Reversed);
        writer.WriteLine("Hello, World!");
        writer.Write(VT100Code.Reset);

        writer.WriteLine("Any key to continue...");
    }

    using (StreamReader reader = console.GetReader())
    {
        while (reader.Read() == -1)
        {
        }
    }

    console.EnableVT100Support();

    using (StreamWriter writer = console.GetWriter())
    {
        writer.Write(VT100Code.SetTitle("Runner!!!"));
        writer.Write(VT100Code.Blue);
        writer.Write(VT100Code.Bold);
        writer.Write(VT100Code.Blink);
        writer.Write(VT100Code.Reversed);
        writer.WriteLine("Hello, World!");
        writer.Write(VT100Code.Reset);

        writer.WriteLine("Any key to continue...");
    }

    using (StreamReader reader = console.GetReader())
    {
        while (reader.Read() == -1)
        {
        }
    }
}
