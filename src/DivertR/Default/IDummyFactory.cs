namespace DivertR.Default
{
    public interface IDummyFactory
    {
        TTarget Create<TTarget>(DiverterSettings settings) where TTarget : class;
    }
}