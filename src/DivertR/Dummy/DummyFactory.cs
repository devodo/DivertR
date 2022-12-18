using System;
using System.Linq.Expressions;
using DivertR.Dummy.Internal;
using DivertR.Internal;

namespace DivertR.Dummy
{
    public class DummyFactory : IDummyFactory
    {
        public DummyFactory()
        {
            var via = new Via(new DummyCallHandler());
            RedirectRepository = new RedirectRepository();
            RedirectRepository.InsertVia(via);
        }
        
        public DummyFactory(IRedirectRepository redirectRepository)
        {
            RedirectRepository = redirectRepository;
        }

        public TTarget Create<TTarget>(DiverterSettings diverterSettings) where TTarget : class?
        {
            var redirect = new Redirect<TTarget>(diverterSettings, RedirectRepository);

            return redirect.Proxy(false);
        }
        
        public IRedirectRepository RedirectRepository { get; }
        
        public IDummyBuilder<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression)
        {
            var viaBuilder = ViaBuilder.To(constraintExpression);

            return new DummyBuilder<TReturn>(RedirectRepository, viaBuilder);
        }

        public IDummyBuilder To(ICallConstraint? callConstraint = null)
        {
            var viaBuilder = ViaBuilder.To(callConstraint);

            return new DummyBuilder(RedirectRepository, viaBuilder);
        }
    }
}