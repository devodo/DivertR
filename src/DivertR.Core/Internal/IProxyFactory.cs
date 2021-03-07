using System;

namespace DivertR.Core.Internal
{
    internal interface IProxyFactory
    {
        T CreateDiverterProxy<T>(T? original, Func<IViaState<T>?> getViaState) where T : class;
        T CreateRedirectTargetProxy<T>(IRelayState<T> relayState) where T : class;
        T CreateOriginalTargetProxy<T>(IRelayState<T> relayState) where T : class;
    }
}