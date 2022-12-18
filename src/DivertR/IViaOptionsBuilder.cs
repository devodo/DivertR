using System;

namespace DivertR
{
    public interface IViaOptionsBuilder
    {
        IViaOptionsBuilder OrderWeight(int orderWeight);
        IViaOptionsBuilder OrderFirst();
        IViaOptionsBuilder OrderLast();
        IViaOptionsBuilder DisableSatisfyStrict(bool disableStrict = true);
        IViaOptionsBuilder Persist(bool isPersistent = true);
        
        IViaOptionsBuilder Decorate(Func<IVia, IVia> decorator);
        IViaOptionsBuilder Repeat(int repeatCount);
        IViaOptionsBuilder Skip(int skipCount);
    }
}