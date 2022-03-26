using System;

namespace DivertR
{
    public class RedirectOptions
    {
        public static readonly RedirectOptions Default = new RedirectOptions();
        
        public int? OrderWeight { get; set; }

        public bool? DisableSatisfyStrict { get; set; }

        public Func<ICallHandler, ICallHandler>? CallHandlerDecorator { get; set; }
        
        public Func<ICallConstraint, ICallConstraint>? CallConstraintDecorator { get; set; }
    }
}