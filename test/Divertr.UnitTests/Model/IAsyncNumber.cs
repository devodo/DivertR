using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface IAsyncNumber
    {
        Task<int> GetNumber(int input);
    }
}