using System;
using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public class Foo : IFoo
    {
        public Foo() : this("original")
        {
        }

        public Foo(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public object? LastAction { get; set; }

        public virtual string NameVirtual
        {
            get => Name;
            set => Name = value;
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
            throw new NotImplementedException();
        }

        public (T1, T2) EchoGeneric<T1, T2>(T1 i1, T2 i2)
        {
            return (i1, i2);
        }

        public (T1, T2, T3) EchoGeneric<T1, T2, T3>(T1 i1, T2 i2, T3 i3)
        {
            return (i1, i2, i3);
        }

        public (T1, T2, T3, T4) EchoGeneric<T1, T2, T3, T4>(T1 i1, T2 i2, T3 i3, T4 i4)
        {
            return (i1, i2, i3, i4);
        }

        public (T1, T2, T3, T4, T5) EchoGeneric<T1, T2, T3, T4, T5>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5)
        {
            return (i1, i2, i3, i4, i5);
        }

        public (T1, T2, T3, T4, T5, T6) EchoGeneric<T1, T2, T3, T4, T5, T6>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6)
        {
            return (i1, i2, i3, i4, i5, i6);
        }

        public (T1, T2, T3, T4, T5, T6, T7) EchoGeneric<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7)
        {
            return (i1, i2, i3, i4, i5, i6, i7);
        }

        public (T1, T2, T3, T4, T5, T6, T7, T8) EchoGeneric<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6,
            T7 i7, T8 i8)
        {
            return (i1, i2, i3, i4, i5, i6, i7, i8);
        }

        public void GenericAction<T1>(T1 i1)
        {
            LastAction = i1;
        }

        public void GenericAction<T1, T2>(T1 i1, T2 i2)
        {
            LastAction = (i1, i2);
        }

        public void GenericAction<T1, T2, T3>(T1 i1, T2 i2, T3 i3)
        {
            LastAction = (i1, i2, i3);
        }

        public void GenericAction<T1, T2, T3, T4>(T1 i1, T2 i2, T3 i3, T4 i4)
        {
            LastAction = (i1, i2, i3, i4);
        }

        public void GenericAction<T1, T2, T3, T4, T5>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5)
        {
            LastAction = (i1, i2, i3, i4, i5);
        }

        public void GenericAction<T1, T2, T3, T4, T5, T6>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6)
        {
            LastAction = (i1, i2, i3, i4, i5, i6);
        }

        public void GenericAction<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7)
        {
            LastAction = (i1, i2, i3, i4, i5, i6, i7);
        }

        public void GenericAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7, T8 i8)
        {
            LastAction = (i1, i2, i3, i4, i5, i6, i7, i8);
        }

        public async Task<string> EchoAsync(string input)
        {
            await Task.Yield();
            return Echo(input);
        }

        public T EchoGenericRef<T>(ref T input)
        {
            return input;
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
            Name = name;
        }
        
        public async void SetName(Task<string> name)
        {
            Name = await name;
        }

        public string SetName(Wrapper<string> input)
        {
            Name = input.Item;

            return Name;
        }

        public IFoo GetFoo()
        {
            return this;
        }
    }
}