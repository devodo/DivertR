using System;
using System.Linq.Expressions;

namespace DivertR
{
    /// <summary>
    /// A Diverter Redirect builder interface for configuring <see cref="IRedirect{TTarget}"/> behaviour.
    /// By default all <see cref="IVia"/> instances added by this builder are configured to be persistent and to not satisfy strict checks.
    /// </summary>
    /// <typeparam name="TTarget">The <see cref="IRedirect{TTarget}"/> target type.</typeparam>
    public interface IDiverterRedirectBuilder<TTarget> where TTarget : class?
    {
        /// <summary>
        /// The underlying Redirect configured by the builder
        /// </summary>
        IRedirect<TTarget> Redirect { get; }
        
        /// <summary>
        /// Insert a <see cref="IVia"/> into the Redirect.
        /// </summary>
        /// <param name="via">The Via instance to insert.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Via(IVia via, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Add an <see cref="IRedirect{TReturn}"/> and proxy all calls with return type <typeparamref name="TReturn"/> via the added redirect.
        /// The added <see cref="IRedirect{TReturn}"/> has default <see cref="RedirectId.Name" />.
        /// </summary>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>The parent builder.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and <see cref="RedirectId.Name" />.</exception>
        IDiverterBuilder ViaRedirect<TReturn>(Action<IViaOptionsBuilder>? optionsAction = null) where TReturn : class?;

        /// <summary>
        /// Add an <see cref="IRedirect{TReturn}"/> and proxy all calls with return type <typeparamref name="TReturn"/> via the added redirect.
        /// </summary>
        /// <param name="name">The <see cref="RedirectId.Name" /> of the added <see cref="IRedirect"/>.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type of calls to redirect.</typeparam>
        /// <returns>The parent builder.</returns>
        /// <exception cref="DiverterException">Thrown if a nested <see cref="IRedirect{TReturn}"/> has already been registered on the parent with matching <typeparamref name="TReturn"/> type and <see cref="RedirectId.Name" />.</exception>
        IDiverterBuilder ViaRedirect<TReturn>(string? name, Action<IViaOptionsBuilder>? optionsAction = null) where TReturn : class?;

        /// <summary>
        /// Add a decorator that will be applied to all call returns of type <typeparamref name="TReturn"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type of calls to decorate.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate<TReturn>(Func<TReturn, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);

        /// <summary>
        /// Add a decorator that will be applied to all call returns of type <typeparamref name="TReturn"/>.
        /// </summary>
        /// <param name="decorator">The decorator function.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <typeparam name="TReturn">The return type of calls to decorate.</typeparam>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Decorate<TReturn>(Func<TReturn, IDiverter, TReturn> decorator, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Retarget all <see cref="IRedirect{TTarget}"/> calls to the given <paramref name="target"/> instance.
        /// </summary>
        /// <param name="target">The target instance to retarget calls to.</param>
        /// <param name="optionsAction">Optional <see cref="IViaOptionsBuilder"/> action.</param>
        /// <returns>The parent builder.</returns>
        IDiverterBuilder Retarget(TTarget target, Action<IViaOptionsBuilder>? optionsAction = null);
        
        /// <summary>
        /// Creates and returns a Diverter Redirect builder with a filter allowing calls matching the given <paramref name="callConstraint"/>.
        /// </summary>
        /// <param name="callConstraint">Optional call constraint <see cref="ICallConstraint"/>.</param>
        /// <returns>The updater instance.</returns>
        IDiverterRedirectToBuilder<TTarget> To(ICallConstraint? callConstraint = null);
        
        /// <summary>
        /// Creates and returns a Diverter Redirect builder with a filter allowing calls matching the given <paramref name="constraintExpression"/> for class types.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="_">Ignore, this is a discard parameter to coerce C# into allowing method overloads on the generic constraints.</param>
        /// <typeparam name="TReturn">The constraint expression return type.</typeparam>
        /// <returns>The created child builder.</returns>
        IDiverterRedirectToFuncBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : class?;
        
        /// <summary>
        /// Creates and returns a Diverter Redirect builder with a filter allowing calls matching the given <paramref name="constraintExpression"/> for struct types.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <param name="_">Ignore, this is a discard parameter to coerce C# into allowing method overloads on the generic constraints.</param>
        /// <typeparam name="TReturn">The constraint expression return type.</typeparam>
        /// <returns>The created child builder.</returns>
        IDiverterRedirectToFuncBuilder<TTarget, TReturn> To<TReturn>(Expression<Func<TTarget, TReturn>> constraintExpression, TReturn? _ = null) where TReturn : struct;
        
        /// <summary>
        /// Creates and returns a Diverter Redirect builder with a filter allowing void returning calls matching the given <paramref name="constraintExpression"/>.
        /// </summary>
        /// <param name="constraintExpression">The call constraint expression.</param>
        /// <returns>The created child builder.</returns>
        IDiverterRedirectToActionBuilder<TTarget> To(Expression<Action<TTarget>> constraintExpression);
        
        /// <summary>
        /// Creates and returns a Diverter Redirect builder with a filter allowing property setter calls matching the given <paramref name="memberExpression"/> and <paramref name="constraintExpression"/>.
        /// </summary>
        /// <param name="memberExpression">The expression for matching the property setter member.</param>
        /// <param name="constraintExpression">Optional constraint expression on the setter input argument. If null, the constraint defaults to match any value</param>
        /// <typeparam name="TProperty">The member type of the property setter.</typeparam>
        /// <returns>The created child builder.</returns>
        IDiverterRedirectToActionBuilder<TTarget> ToSet<TProperty>(Expression<Func<TTarget, TProperty>> memberExpression, Expression<Func<TProperty>>? constraintExpression = null);
    }
}