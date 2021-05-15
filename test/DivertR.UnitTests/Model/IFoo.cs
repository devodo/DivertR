using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface IFoo
    {
        string Message { get; set; }

        Task<string> GetMessageAsync();

        string Echo(string input);

        T EchoGeneric<T>(T input);

        public Task<string> EchoAsync(string input);
        
        public ValueTask<string> EchoValueAsync(string input);
        
        string SetMessage(Wrapper<string> input);

        IFoo GetFoo();
    }
}