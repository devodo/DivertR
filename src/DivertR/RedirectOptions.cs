using System;

namespace DivertR
{
    public class RedirectOptions : IRedirectOptions
    {
        public static readonly RedirectOptions Default = new RedirectOptions();
        
        public int? OrderWeight { get; set; }

        public bool? DisableSatisfyStrict { get; set; }

        public Func<ICallHandler, ICallHandler>? CallHandlerDecorator { get; set; }
        
        public Func<ICallConstraint, ICallConstraint>? CallConstraintDecorator { get; set; }
    }
    
    public class RedirectOptions<TTarget> : IRedirectOptions<TTarget> where TTarget : class
    {
        public static readonly RedirectOptions<TTarget> Default = new RedirectOptions<TTarget>();
        private Func<ICallHandler, ICallHandler>? _callHandlerDecorator;
        private Func<ICallConstraint, ICallConstraint>? _callConstraintDecorator;

        public int? OrderWeight { get; set; }
        public bool? DisableSatisfyStrict { get; set; }

        Func<ICallHandler, ICallHandler>? IRedirectOptions.CallHandlerDecorator => CallHandlerDecorator;

        Func<ICallConstraint, ICallConstraint>? IRedirectOptions.CallConstraintDecorator => _callConstraintDecorator;

        public Func<ICallHandler<TTarget>, ICallHandler<TTarget>>? CallHandlerDecorator { get; set; }
        
        public Func<ICallConstraint<TTarget>, ICallConstraint<TTarget>>? CallConstraintDecorator { get; set; }
    }
}