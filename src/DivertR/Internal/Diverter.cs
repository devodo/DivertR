using System;
using System.Collections.Generic;

namespace DivertR.Internal
{
    internal class Diverter : IDiverter
    {
        private readonly Func<string?, IEnumerable<IServiceDecorator>> _getDecorators;
        
        public Diverter(IRedirectSet redirectSet, Func<string?, IEnumerable<IServiceDecorator>> getDecorators)
        {
            RedirectSet = redirectSet;
            _getDecorators = getDecorators;
        }

        public IRedirectSet RedirectSet { get; }

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
            var redirect = RedirectSet.Get(redirectId);
            
            if (redirect == null)
            {
                throw new DiverterException($"Redirect not registered for {redirectId}");
            }
            
            return redirect;
        }
        
        public IDiverter StrictAll()
        {
            RedirectSet.StrictAll();
            
            return this;
        }
        
        public IDiverter Strict(string? name = null)
        {
            RedirectSet.Strict(name);

            return this;
        }
        
        public IDiverter ResetAll()
        {
            RedirectSet.ResetAll();
            
            return this;
        }
        
        public IDiverter Reset(string? name = null)
        {
            RedirectSet.Reset(name);

            return this;
        }
        
        public IEnumerable<IServiceDecorator> GetDecorators(string? name = null)
        {
            return _getDecorators(name);
        }
    }
}