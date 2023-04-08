using System;
using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;

namespace DivertR.SampleWebApp.Services;

public class BarService : IBarService
{
    public Task<Bar> CreateBarAsync(string name)
    {
        throw new NotImplementedException();
    }
}