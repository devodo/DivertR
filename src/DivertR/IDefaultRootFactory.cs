namespace DivertR
{
    public interface IDefaultRootFactory
    {
        TTarget Create<TTarget>() where TTarget : class;
    }
}