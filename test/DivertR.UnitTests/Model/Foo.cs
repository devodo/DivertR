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

        public string Name
        {
            get => _name;
            set => _name = value;
        }
        
        public virtual string NameVirtual
        {
            get => _name;
            set => _name = value;
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

        public T1 EchoGeneric<T1>(T1 i1)
        {
            return i1;
        }

        public T1 EchoGenericAlt<T1>(T1 i1)
        {
            throw new System.NotImplementedException();
        }

        public (T1, T2) EchoGeneric<T1, T2>(T1 i1, T2 i2)
        {
            return (i1, i2);
        }

        public async Task<string> EchoAsync(string input)
        {
            await Task.Yield();
            return Echo(input);
        }

        public async ValueTask<string> EchoValueAsync(string input)
        {
            await Task.Yield();
            return await EchoAsync(input);
        }
        
        public ValueTask<string> EchoValueSync(string input)
        {
            return new(Echo(input));
        }
        
        public void SetName(string name)
        {
            _name = name;
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