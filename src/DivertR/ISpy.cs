using System;
using DivertR.Record;

namespace DivertR
{
    /// <summary>
    /// The <see cref="ISpy"/> interface is an extension of the <see cref="IRedirect"/> interface that emulates classic mocking capability.
    /// This is done by providing:
    ///     1. A <see cref="ISpy.Mock"/> object property that is an <see cref="IRedirect"/> proxy instance.
    ///     2. A <see cref="ISpy.Calls"/> property that is a <see cref="IRecordStream"/> of all calls to the Mock object.
    ///
    /// <see cref="ISpy"/> inherits from <see cref="IRedirect"/> and therefore its Mock behaviour can be configured using the same Redirect fluent interface e.g. to add one or more <see cref="IVia"/> instances or reset.
    /// The Spy is preconfigured with a Via that records the mock object calls readable from the <see cref="ISpy.Calls"/> property.
    /// As with a normal Redirect, Spy reset removes all Vias, however it also adds a new Mock call record Via and the <see cref="ISpy.Calls"/> property is replaced.
    /// </summary>
    public interface ISpy : IRedirect
    {
        /// <summary>
        /// The mock proxy object of this <see cref="ISpy"/>.
        /// </summary>
        object Mock { get; }
        
        /// <summary>
        /// Record of all calls to the <see cref="ISpy.Mock"/> proxy.
        /// </summary>
        IRecordStream Calls { get; }
        
        /// <summary>
        /// Insert a <see cref="IVia"/> into this Spy.
        /// </summary>
        /// <param name="via">The Via instance to insert.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This Spy instance.</returns>
        new ISpy Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Reset the Spy's <see cref="IRedirectRepository" /> to its initial state.
        /// </summary>
        /// <returns>This Spy instance.</returns>
        new ISpy Reset();
        
        /// <summary>
        /// Set strict mode on the Spy.
        /// If strict is enabled and a call to its proxies does not hit a configured <see cref="IVia"/> then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This Spy instance.</returns>
        new ISpy Strict(bool? isStrict = true);
    }
    
    /// <summary>
    /// Strongly typed generic extension of the <see cref="ISpy"/> interface mock target type defined.
    /// </summary>
    /// <typeparam name="TTarget">The mock target type.</typeparam>
    public interface ISpy<TTarget> : IRedirect<TTarget>, ISpy where TTarget : class?
    {
        /// <summary>
        /// The mock TTarget proxy object of this <see cref="ISpy{TTarget}"/>.
        /// </summary>
        new TTarget Mock { get; }
        
        /// <summary>
        /// Record of all calls to the <see cref="ISpy{TTarget}.Mock"/> proxy.
        /// </summary>
        new IRecordStream<TTarget> Calls { get; }
        
        /// <summary>
        /// Insert a <see cref="IVia"/> into this Spy.
        /// </summary>
        /// <param name="via">The Via instance to insert.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This Spy instance.</returns>
        new ISpy<TTarget> Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Reset the Spy's <see cref="IRedirectRepository" /> to its initial state.
        /// </summary>
        /// <returns>This Spy instance.</returns>
        new ISpy<TTarget> Reset();
        
        /// <summary>
        /// Set strict mode on the Spy.
        /// If strict is enabled and a call to its proxies does not hit a configured <see cref="IVia"/> then a <see cref="StrictNotSatisfiedException"/> is thrown.
        /// </summary>
        /// <param name="isStrict">Optional bool to specify enable/disable of strict mode.</param>
        /// <returns>This Spy instance.</returns>
        new ISpy<TTarget> Strict(bool? isStrict = true);
        
        /// <summary>
        /// Inserts a retarget <see cref="IVia"/> with no call constraints (therefore all calls will be matched and retargeted).
        /// </summary>
        /// <param name="target">The target instance to retarget calls to.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>This Spy instance.</returns>
        new ISpy<TTarget> Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
    }
}