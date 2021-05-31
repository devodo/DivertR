using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface IFoo
    {
        string Name { get; set; }

        Task<string> GetNameAsync();

        string Echo(string input);

        string EchoGeneric<T>(T input);

        public Task<string> EchoAsync(string input);
        
        public ValueTask<string> EchoValueAsync(string input);
        
        string SetName(Wrapper<string> input);

        IFoo GetFoo();
    }
}