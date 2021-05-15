using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface IValueTaskFoo
    {
        ValueTask<string> MessageAsync { get; }
    }
}