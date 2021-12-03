using System;

namespace DivertR
{
    public interface IViaSet
    {
        DiverterSettings Settings { get; }
        IVia<TTarget> Via<TTarget>(string? name = null) where TTarget : class;
        IVia Via(Type targetType, string? name = null);
        IVia? Reset(ViaId viaId);
        IViaSet Reset(string? name = null);
        IViaSet ResetAll();
        IViaSet Strict(string? name = null);
        IViaSet StrictAll();
    }
}