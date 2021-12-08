using System;

namespace DivertR
{
    public class RedirectOptions<TTarget> where TTarget : class
    {
        public static readonly RedirectOptions<TTarget> Default = new RedirectOptions<TTarget>();
        
        public int? OrderWeight { get; set; }

        public bool? DisableSatisfyStrict { get; set; }

        public Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? CallHandlerDecorator { get; set; }
        
        public Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? CallConstraintDecorator { get; set; }
    }
}