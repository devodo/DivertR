using System;
using DivertR.Internal;

namespace DivertR.Default
{
    public class DummyVia : Via, IDummyVia
    {
        private readonly IProxyFactory _proxyFactory;
        
        public DummyVia(string? name = null) : this(ViaId.From<DummyVia>(name), new ViaSet())
        {
            ((ViaSet) ViaSet).AddVia(this);
        }
        
        public DummyVia(DiverterSettings? diverterSettings) : this(ViaId.From<DummyVia>(), new ViaSet(diverterSettings))
        {
            ((ViaSet) ViaSet).AddVia(this);
        }
        
        public DummyVia(string? name, DiverterSettings? diverterSettings) : this(ViaId.From<DummyVia>(name), new ViaSet(diverterSettings))
        {
            ((ViaSet) ViaSet).AddVia(this);
        }

        private DummyVia(ViaId viaId, IViaSet viaSet) : base(viaId, viaSet, new Relay())
        {
            _proxyFactory = viaSet.Settings.ProxyFactory;
        }

        public override object ProxyObject(object? root)
        {
            throw new System.NotImplementedException();
        }

        public override object ProxyObject()
        {
            throw new System.NotImplementedException();
        }

        public TTarget Proxy<TTarget>(TTarget? root = null) where TTarget : class
        {
            _proxyFactory.ValidateProxyTarget<TTarget>();
            
            return _proxyFactory.CreateProxy<TTarget>(GetProxyCall, root);
        }
        
        public IDummyVia Redirect<TReturn>(Func<IFuncRedirectCall<TReturn>, TReturn> redirectDelegate, Action<IRedirectOptionsBuilder>? optionsAction = null)
        {
            var callHandler = new FuncRedirectCallHandler<TReturn>(redirectDelegate);
            var callConstraint = new ReturnTypeCallConstraint<TReturn>();
            var redirect = new Redirect(callHandler, callConstraint, optionsAction.Create());
            InsertRedirect(redirect);

            return this;
        }
    }
}