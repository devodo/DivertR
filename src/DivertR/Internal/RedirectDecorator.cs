using System;

namespace DivertR.Internal
{
    internal class RedirectDecorator : IServiceDecorator
    {
        private readonly IRedirect _redirect;

        public RedirectDecorator(IRedirect redirect)
        {
            _redirect = redirect;
        }

        public Type ServiceType => _redirect.RedirectId.Type;
        
        public object Decorate(object input, IDiverter diverter, IServiceProvider provider)
        {
            return _redirect.RedirectSet.Settings.DiverterProxyFactory.CreateProxy(_redirect, input);
        }
    }
}