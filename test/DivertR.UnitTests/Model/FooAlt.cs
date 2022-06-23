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

        public object LastAction { get; set; }

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
            throw new NotImplementedException();
        }

        public T1 EchoGenericAlt<T1>(T1 i1)
        {
            throw new NotImplementedException();
        }

        public (T1, T2) EchoGeneric<T1, T2>(T1 i1, T2 i2)
        {
            throw new NotImplementedException();
        }

        public (T1, T2, T3) EchoGeneric<T1, T2, T3>(T1 i1, T2 i2, T3 i3)
        {
            throw new NotImplementedException();
        }

        public (T1, T2, T3, T4) EchoGeneric<T1, T2, T3, T4>(T1 i1, T2 i2, T3 i3, T4 i4)
        {
            throw new NotImplementedException();
        }

        public (T1, T2, T3, T4, T5) EchoGeneric<T1, T2, T3, T4, T5>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5)
        {
            throw new NotImplementedException();
        }

        public (T1, T2, T3, T4, T5, T6) EchoGeneric<T1, T2, T3, T4, T5, T6>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6)
        {
            throw new NotImplementedException();
        }

        public (T1, T2, T3, T4, T5, T6, T7) EchoGeneric<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7)
        {
            throw new NotImplementedException();
        }

        public (T1, T2, T3, T4, T5, T6, T7, T8) EchoGeneric<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6,
            T7 i7, T8 i8)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1>(T1 i1)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1, T2>(T1 i1, T2 i2)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1, T2, T3>(T1 i1, T2 i2, T3 i3)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1, T2, T3, T4>(T1 i1, T2 i2, T3 i3, T4 i4)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1, T2, T3, T4, T5>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1, T2, T3, T4, T5, T6>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7)
        {
            throw new NotImplementedException();
        }

        public void GenericAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7, T8 i8)
        {
            throw new NotImplementedException();
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

        public void SetName(Task<string> name)
        {
            throw new NotImplementedException();
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