﻿using DivertR.Core;

namespace DivertR.Setup
{
    public interface IDiverterSettings
    {
        IProxyFactory ProxyFactory { get; }
    }
}