using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;

namespace DivertR.SampleWebApp.Services;

public interface IBarService
{
    Task<Bar> CreateBarAsync(string name);
}