using System;

namespace DivertR
{
    public class RedirectOptions : IRedirectOptions
    {
        public static readonly RedirectOptions Default = new RedirectOptions(0, false);

        public RedirectOptions(
            int? orderWeight = null,
            bool? disableSatisfyStrict = null)
        {
            OrderWeight = orderWeight ?? Default.OrderWeight;
            DisableSatisfyStrict = disableSatisfyStrict ?? Default.DisableSatisfyStrict;
        }

        public int? OrderWeight { get; }
        public bool? DisableSatisfyStrict { get; }
    }
    
    public class RedirectOptions<TTarget> : RedirectOptions, IRedirectOptions<TTarget> where TTarget : class
    {
        public new static readonly RedirectOptions<TTarget> Default = new RedirectOptions<TTarget>(0, false);

        public RedirectOptions(
            int? orderWeight = null,
            bool? disableSatisfyStrict = null,
            Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? callHandlerDecorator = null,
            Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? callConstraintDecorator = null)
            : base(orderWeight, disableSatisfyStrict)
        {
            CallHandlerDecorator = callHandlerDecorator;
            CallConstraintDecorator = callConstraintDecorator;
        }
        
        public Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? CallHandlerDecorator { get; }
        public Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? CallConstraintDecorator { get; }
    }
}