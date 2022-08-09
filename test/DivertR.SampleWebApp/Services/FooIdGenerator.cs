using System;

namespace DivertR.SampleWebApp.Services;

public class FooIdGenerator : IFooIdGenerator
{
    public Guid Create()
    {
        return Guid.NewGuid();
    }
}