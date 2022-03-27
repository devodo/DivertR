namespace DivertR.Default
{
    public interface IDefaultRootFactory
    {
        TTarget CreateRoot<TTarget>() where TTarget : class;
    }
}