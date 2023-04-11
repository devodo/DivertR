using System;
using System.Collections.Generic;
using System.Linq;

namespace DivertR.Internal
{
    internal class ViaBuilder : IViaBuilder
    {
        protected readonly List<ICallConstraint> CallConstraints;

        public ViaBuilder(ICallConstraint? callConstraint = null)
        {
            CallConstraints = new List<ICallConstraint>();
            
            if (callConstraint != null)
            {
                CallConstraints.Add(callConstraint);
            }
        }

        protected ViaBuilder(List<ICallConstraint> callConstraints)
        {
            CallConstraints = callConstraints;
        }

        public IViaBuilder Filter(ICallConstraint callConstraint)
        {
            CallConstraints.Add(callConstraint);

            return this;
        }

        public IVia Build(Func<object?> viaDelegate)
        {
            return Build(_ => viaDelegate.Invoke());
        }
        
        public IVia Build(Func<IRedirectCall, object?> viaDelegate)
        {
            var callHandler = new DelegateCallHandler(viaDelegate);

            return Build(callHandler);
        }

        public IVia Build<TTarget>(Func<IRedirectCall<TTarget>, object?> viaDelegate) where TTarget : class?
        {
            var callHandler = new DelegateCallHandler<TTarget>(viaDelegate);

            return Build(callHandler);
        }

        public IVia Build(ICallHandler callHandler)
        {
            return new Via(callHandler, BuildCallConstraint());
        }

        private ICallConstraint BuildCallConstraint()
        {
            if (CallConstraints.Count == 0)
            {
                return TrueCallConstraint.Instance;
            }
            
            if (CallConstraints.Count == 1)
            {
                return CallConstraints.First();
            }

            return new CompositeCallConstraint(CallConstraints);
        }
    }
}