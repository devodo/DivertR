using System.Threading.Tasks;

namespace DivertR.SampleWebApp.Services;

public interface IBarServiceFactory
{
    Task<IBarService> CreateBarService();
}