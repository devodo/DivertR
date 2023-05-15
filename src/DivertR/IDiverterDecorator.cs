using System;
using System.Runtime.CompilerServices;

namespace DivertR
{
    /// <summary>
    /// A <see cref="IDiverter"/> service decorator interface intended to be used to decorate a dependency injection service.
    /// </summary>
    public interface IDiverterDecorator
    {
        /// <summary>
        /// The service type.
        /// </summary>
        Type ServiceType { get; }
        
        /// <summary>
        /// The decorate method.
        /// </summary>
        /// <param name="input">The original instance to be decorated.</param>
        /// <param name="diverter">The <see cref="IDiverter"/> instance where this decorator is registered.</param>
        /// <param name="provider">The service provider over the dependency injection container where this decorator is embedded.</param>
        /// <returns>The decorated instance.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        object Decorate(object input, IDiverter diverter, IServiceProvider provider);
    }
}