﻿using DivertR.DispatchProxy;
using DivertR.Dummy;
using DivertR.Internal;

namespace DivertR
{
    public class DiverterSettings
    {
        private static readonly object GlobalLock = new object();
        private static readonly ICallInvoker DefaultCallInvoker = new LambdaExpressionCallInvoker();
        private static DiverterSettings GlobalSettings = new DiverterSettings();
        
        
        public IProxyFactory ProxyFactory { get; }

        public bool DefaultWithDummyRoot { get; }

        public IDummyFactory DummyFactory { get; }
        
        public ICallInvoker CallInvoker { get; }


        public static DiverterSettings Global
        {
            get
            {
                lock (GlobalLock)
                {
                    return GlobalSettings;
                }
            }

            set
            {
                lock (GlobalLock)
                {
                    GlobalSettings = value;
                }
            }
        }

        public DiverterSettings(IProxyFactory? proxyFactory = null,
            bool defaultWithDummyRoot = true,
            IDummyFactory? dummyFactory = null,
            ICallInvoker? callInvoker = null)
        {
            ProxyFactory = proxyFactory ?? new DispatchProxyFactory();
            DefaultWithDummyRoot = defaultWithDummyRoot;
            DummyFactory = dummyFactory ?? new DummyFactory();
            CallInvoker = callInvoker ?? DefaultCallInvoker;
        }
    }
}