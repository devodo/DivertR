namespace DivertR.WebApp.Services;

public class BarServiceFactory : IBarServiceFactory
{
    public Task<IBarService> CreateBarService()
    {
        return Task.FromResult<IBarService>(new BarService());
    }
}