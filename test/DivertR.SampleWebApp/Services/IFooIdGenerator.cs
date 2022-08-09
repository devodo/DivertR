using System;

namespace DivertR.SampleWebApp.Services;

public interface IFooIdGenerator
{
    Guid Create();
}