using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface IAsyncFoo
    {
        Task<string> MessageAsync { get; }
    }
}