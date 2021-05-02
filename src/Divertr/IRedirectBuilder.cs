namespace DivertR
{
    public interface IRedirectBuilder<TTarget> where TTarget : class
    {
        ICallConstraint<TTarget> CallConstraint { get; }
        IRedirectBuilder<TTarget> AddConstraint(ICallConstraint<TTarget> callConstraint);
        IRedirectBuilder<TTarget> WithOrderWeight(int orderWeight);
        IRedirect<TTarget> Build(TTarget target);
        IVia<TTarget> To(TTarget target);
    }
}