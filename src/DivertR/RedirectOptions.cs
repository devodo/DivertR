using System;

namespace DivertR
{
    public class RedirectOptions<TTarget> : IRedirectOptions<TTarget> where TTarget : class
    {
        public static readonly RedirectOptions<TTarget> Default = new RedirectOptions<TTarget>();

        public RedirectOptions(
            int? orderWeight = 0,
            bool? disableSatisfyStrict = false,
            Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? callHandlerDecorator = null,
            Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? callConstraintDecorator = null)
        {
            OrderWeight = orderWeight;
            DisableSatisfyStrict = disableSatisfyStrict;
            CallHandlerDecorator = callHandlerDecorator;
            CallConstraintDecorator = callConstraintDecorator;
        }

        public int? OrderWeight { get; }
        public bool? DisableSatisfyStrict { get; }
        public Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? CallHandlerDecorator { get; }
        public Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? CallConstraintDecorator { get; }
    }
}