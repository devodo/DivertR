namespace DivertR
{
    /// <summary>
    /// A factory used by Diverter to create service decorators from <see cref="IRedirect"/> proxies.
    /// </summary>
    public interface IDecoratorFactory
    {
        /// <summary>
        /// Creates a service decorator.
        /// </summary>
        /// <param name="redirect">The redirect registered for the <paramref name="service"/> type.</param>
        /// <param name="service">The service instance to be decorated.</param>
        /// <returns>The decorate service.</returns>
        object? CreateDecorator(IRedirect redirect, object? service);
    }
}