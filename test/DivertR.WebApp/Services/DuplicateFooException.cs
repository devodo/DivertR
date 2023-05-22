using DivertR.WebApp.Model;

namespace DivertR.WebApp.Services;

public class DuplicateFooException : Exception
{
    public Foo Foo { get; }

    public DuplicateFooException(Foo foo, string? message) : base(message)
    {
        Foo = foo;
    }
}