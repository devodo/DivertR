using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class Foo : IFoo
    {
        private string _name;
        
        public Foo() : this("original")
        {
        }

        public Foo(string name)
        {
            _name = name;
        }

        public virtual string Name
        {
            get => _name;
            set => _name = value;
        }

        public async Task<string> GetNameAsync()
        {
            await Task.Yield();
            return _name;
        }

        public string Echo(string input)
        {
            return input;
        }

        public string EchoGeneric<T>(T input)
        {
            return $"{input}";
        }

        public async Task<string> EchoAsync(string input)
        {
            await Task.Yield();
            return input;
        }

        public async ValueTask<string> EchoValueAsync(string input)
        {
            await Task.Yield();
            return input;
        }

        public string SetName(Wrapper<string> input)
        {
            _name = input.Item;

            return _name;
        }

        public IFoo GetFoo()
        {
            return this;
        }
    }
}