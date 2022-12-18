using System.Runtime.CompilerServices;

namespace DivertR.Internal
{
    internal class ConfiguredVia : IConfiguredVia
    {
        public ConfiguredVia(IVia via, IViaOptions viaOptions)
        {
            Via = viaOptions.ViaDecorator?.Invoke(via) ?? via;
            Options = viaOptions;
        }
        
        public IVia Via
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }

        public IViaOptions Options
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get;
        }
    }
}