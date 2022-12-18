using System.Diagnostics.CodeAnalysis;

namespace DivertR
{
    public interface IRedirectProxyDecorator
    {
        [return: NotNull]
        TTarget Decorate<TTarget>(IRedirect redirect, [DisallowNull] TTarget proxy) where TTarget : class?;
    }
}