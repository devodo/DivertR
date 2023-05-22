using DivertR.WebApp.Model;

namespace DivertR.WebApp.Services;

public interface IBarService
{
    Task<Bar> CreateBarAsync(string name);
}