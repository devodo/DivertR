using System;
using DivertR.SampleWebApp.Model;

namespace DivertR.SampleWebApp.Services;

public class DuplicateFooException : Exception
{
    public Foo Foo { get; }

    public DuplicateFooException(Foo foo, string? message) : base(message)
    {
        Foo = foo;
    }
}