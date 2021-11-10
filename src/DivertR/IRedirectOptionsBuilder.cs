using System;
using DivertR.Internal;

namespace DivertR
{
    public interface IRedirectOptionsBuilder<TTarget> where TTarget : class
    {
        IRedirectOptionsBuilder<TTarget> OrderWeight(int orderWeight);
        IRedirectOptionsBuilder<TTarget> DisableSatisfyStrict(bool disableStrict = true);
        IRedirectOptionsBuilder<TTarget> ChainCallHandler(Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>> chainLink);
        IRedirectOptionsBuilder<TTarget> Repeat(int repeatCount);
        IRedirectOptionsBuilder<TTarget> Skip(int skipCount);
    }
}