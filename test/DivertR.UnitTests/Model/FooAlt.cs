using System;
using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class FooAlt : IFoo
    {
        private Func<string> _messageFactory;
        
        public FooAlt() : this("alternate")
        {
        }

        public FooAlt(string message)
        {
            _messageFactory = () => message;
        }
        
        public FooAlt(Func<string> messageFactory)
        {
            _messageFactory = messageFactory;
        }

        public string Name
        {
            get => _messageFactory.Invoke();
            set => _messageFactory = () => value;
        }

        public virtual string NameVirtual
        {
            get => _messageFactory.Invoke();
            set => _messageFactory = () => value;
        }

        public async Task<string> GetNameAsync()
        {
            await Task.Yield();
            return Name;
        }
        
        public async ValueTask<string> GetNameValueAsync()
        {
            return await GetNameAsync();
        }
        
        public ValueTask<string> GetNameValueSync()
        {
            return new(Name);
        }

        public string Echo(string input)
        {
            return $"{Name}: {input}";
        }

        public string EchoGeneric<T>(T input)
        {
            return Echo($"{input}");
        }

        public async Task<string> EchoAsync(string input)
        {
            await Task.Yield();
            return Echo(input);
        }

        public async ValueTask<string> EchoValueAsync(string input)
        {
            return await EchoAsync(input);
        }

        public ValueTask<string> EchoValueSync(string input)
        {
            return new(Echo(input));
        }

        public void SetName(string name)
        {
            _messageFactory = () => name;
        }

        public string SetName(Wrapper<string> input)
        {
            _messageFactory = () => input.Item;

            return _messageFactory.Invoke();
        }

        public IFoo GetFoo()
        {
            return this;
        }
    }
}