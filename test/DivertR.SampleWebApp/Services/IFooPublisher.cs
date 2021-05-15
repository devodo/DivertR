using System.Threading.Tasks;
using DivertR.SampleWebApp.Model;

namespace DivertR.SampleWebApp.Services
{
    public interface IFooPublisher
    {
        Task Publish(FooEvent fooEvent);
    }
}