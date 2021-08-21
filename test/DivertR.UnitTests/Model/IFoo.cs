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

        T1 EchoGeneric<T1>(T1 i1);
        (T1, T2) EchoGeneric<T1, T2>(T1 i1, T2 i2);

        Task<string> EchoAsync(string input);
        
        ValueTask<string> EchoValueAsync(string input);
        ValueTask<string> EchoValueSync(string input);
        
        void SetName(string name);
        
        string SetName(Wrapper<string> input);

        IFoo GetFoo();
    }
}