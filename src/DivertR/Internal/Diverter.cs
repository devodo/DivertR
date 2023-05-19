using System;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class Diverter : IDiverter
    {
        private readonly Func<string?, IEnumerable<IServiceDecorator>> _getDecorators;
        private readonly IRedirectSet _redirectSet;
        
        public Diverter(IRedirectSet redirectSet, Func<string?, IEnumerable<IServiceDecorator>> getDecorators)
        {
            _redirectSet = redirectSet;
            _getDecorators = getDecorators;
        }

        public IRedirect<TTarget> Redirect<TTarget>(string? name = null) where TTarget : class?
        {
            return (IRedirect<TTarget>) Redirect(RedirectId.From<TTarget>(name));
        }
        
        public IRedirect Redirect(Type type, string? name = null)
        {
            return Redirect(RedirectId.From(type, name));
        }
        
        public IRedirect Redirect(RedirectId redirectId)
        {
            var redirect = _redirectSet.Get(redirectId);
            
            if (redirect == null)
            {
                throw new DiverterException($"Redirect not registered for {redirectId}");
            }
            
            return redirect;
        }
        
        public IDiverter StrictAll()
        {
            _redirectSet.StrictAll();
            
            return this;
        }
        
        public IDiverter Strict(string? name = null)
        {
            _redirectSet.Strict(name);

            return this;
        }
        
        public IDiverter ResetAll()
        {
            _redirectSet.ResetAll();
            
            return this;
        }
        
        public IDiverter Reset(string? name = null)
        {
            _redirectSet.Reset(name);

            return this;
        }
        
        public IEnumerable<IServiceDecorator> GetDecorators(string? name = null)
        {
            return _getDecorators(name);
        }
    }
}