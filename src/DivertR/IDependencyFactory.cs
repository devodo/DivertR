namespace DivertR
{
    public interface IDependencyFactory
    {
        object? Create(IVia via, object? original);
    }
}