using System;

namespace DivertR
{
    public interface IViaOptions
    {
        int OrderWeight { get; }
        bool DisableSatisfyStrict { get; }  
        bool IsPersistent { get; }
        Func<IVia, IVia>? ViaDecorator { get; }
    }
}