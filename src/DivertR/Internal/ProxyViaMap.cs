using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ProxyViaMap
    {
        private readonly ConditionalWeakTable<object, IVia> _viaMap = new();

        public void AddVia(object proxy, IVia via)
        {
            _viaMap.Add(proxy, via);
        }

        public IVia<TTarget> GetVia<TTarget>(TTarget proxy) where TTarget : class
        {
            if (!_viaMap.TryGetValue(proxy, out var via))
            {
                throw new DiverterException("Via not found");
            }

            if (via is not IVia<TTarget> viaOf)
            {
                throw new DiverterException($"Via target type: {via.ViaId.Type} does not match proxy type: {typeof(TTarget)}");
            }

            return viaOf;
        }
    }
}