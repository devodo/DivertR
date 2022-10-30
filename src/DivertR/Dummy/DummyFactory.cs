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
            var redirect = new Redirect(new DummyCallHandler());
            RedirectRepository = new RedirectRepository();
            RedirectRepository.InsertRedirect(redirect);
        }
        
        public DummyFactory(IRedirectRepository redirectRepository)
        {
            RedirectRepository = redirectRepository;
        }

        public TTarget Create<TTarget>(DiverterSettings diverterSettings) where TTarget : class?
        {
            var via = new Via<TTarget>(diverterSettings, RedirectRepository);

            return via.Proxy(false);
        }
        
        public IRedirectRepository RedirectRepository { get; }
        
        public IDummyBuilder<TReturn> To<TReturn>(Expression<Func<TReturn>> constraintExpression)
        {
            var redirectBuilder = RedirectBuilder.To(constraintExpression);

            return new DummyBuilder<TReturn>(RedirectRepository, redirectBuilder);
        }

        public IDummyBuilder To(ICallConstraint? callConstraint = null)
        {
            var redirectBuilder = RedirectBuilder.To(callConstraint);

            return new DummyBuilder(RedirectRepository, redirectBuilder);
        }
    }
}