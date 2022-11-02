using System.Diagnostics.CodeAnalysis;

namespace DivertR
{
    public interface IViaProxyDecorator
    {
        [return: NotNull]
        TTarget Decorate<TTarget>(IVia via, [DisallowNull] TTarget proxy) where TTarget : class?;
    }
}