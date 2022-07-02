namespace DivertR.Dummy
{
    public interface IDummyFactory
    {
        TTarget Create<TTarget>(DiverterSettings diverterSettings) where TTarget : class;
    }
}