namespace DivertR.WebApp.Services;

public interface IBarServiceFactory
{
    Task<IBarService> CreateBarService();
}