using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface IFoo
    {
        string Name { get; set; }
        
        string NameVirtual { get; set; }

        Task<string> GetNameAsync();
        ValueTask<string> GetNameValueAsync();
        ValueTask<string> GetNameValueSync();

        string Echo(string input);

        string EchoGeneric<T>(T input);

        Task<string> EchoAsync(string input);
        
        ValueTask<string> EchoValueAsync(string input);
        ValueTask<string> EchoValueSync(string input);
        
        void SetName(string name);
        
        string SetName(Wrapper<string> input);

        IFoo GetFoo();
    }
}