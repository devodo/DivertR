﻿using System;

namespace DivertR
{
    public interface IProxyFactory
    {
        TTarget CreateProxy<TTarget>(TTarget? root, Func<IProxyCall<TTarget>?> getProxyCall) where TTarget : class;
        TTarget CreateProxy<TTarget>(IProxyCall<TTarget> proxyCall) where TTarget : class;
        void ValidateProxyTarget<TTarget>();
    }
}