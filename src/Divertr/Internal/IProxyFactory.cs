using System;

namespace DivertR.Internal
{
    internal interface IProxyFactory
    {
        T CreateDiverterProxy<T>(T? original, Func<ViaWay<T>?> getViaWay) where T : class;
        T CreateRedirectTargetProxy<T>(Relay<T> relay) where T : class;
        T CreateOriginalTargetProxy<T>(Relay<T> relay) where T : class;
    }
}