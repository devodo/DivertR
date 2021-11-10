using System;

namespace DivertR
{
    public class RedirectOptions<TTarget> where TTarget : class
    {
        public int? OrderWeight { get; set; }

        public bool? DisableSatisfyStrict { get; set; }

        public Func<IVia<TTarget>, ICallHandler<TTarget>, ICallHandler<TTarget>>? CallHandlerDecorator { get; set; }
    }
}