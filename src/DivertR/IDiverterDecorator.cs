using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    public interface IDiverterDecorator
    {
        Type ServiceType { get; }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object Decorate(object input, IDiverter diverter, IServiceProvider provider);
    }
}