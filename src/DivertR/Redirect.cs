﻿using System;

namespace DivertR
{
    public class Redirect<TTarget> where TTarget : class
    {
        public Redirect(ICallHandler<TTarget> callHandler, ICallConstraint<TTarget>? callConstraint = null, int? orderWeight = null, bool? noStrictSatisfy = null)
        {
            CallHandler = callHandler ?? throw new ArgumentNullException(nameof(callHandler));
            CallConstraint = callConstraint ?? TrueCallConstraint<TTarget>.Instance;
            OrderWeight = orderWeight ?? 0;
            NoStrictSatisfy = noStrictSatisfy ?? false;
        }
        
        public ICallHandler<TTarget> CallHandler { get; }
        
        public ICallConstraint<TTarget> CallConstraint { get; }
        
        public int OrderWeight { get; }

        public bool NoStrictSatisfy { get; }
    }
}