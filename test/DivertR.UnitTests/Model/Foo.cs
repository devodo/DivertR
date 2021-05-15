using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class Foo : IFoo
    {
        private string _message;
        
        public Foo() : this("original")
        {
        }

        public Foo(string message)
        {
            _message = message;
        }

        public virtual string Message
        {
            get => _message;
            set => _message = value;
        }

        public async Task<string> GetMessageAsync()
        {
            await Task.Yield();
            return _message;
        }

        public string Echo(string input)
        {
            return input;
        }

        public T EchoGeneric<T>(T input)
        {
            return input;
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

        public string SetMessage(Wrapper<string> input)
        {
            _message = input.Item;

            return _message;
        }

        public IFoo GetFoo()
        {
            return this;
        }
    }
}