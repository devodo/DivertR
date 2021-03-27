using System.Collections.Immutable;
using DivertR.Core;

namespace DivertR.Internal
{
    internal class ViaState<T> where T : class
    {
        public ImmutableArray<IRedirect<T>> Redirects { get; }
        
        public ViaState(ImmutableArray<IRedirect<T>> redirects)
        {
            Redirects = redirects;
        }
    }
}