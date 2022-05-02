namespace DivertR.Default
{
    public interface IDummyFactory
    {
        TTarget Create<TTarget>(DiverterSettings diverterSettings) where TTarget : class;
    }
}