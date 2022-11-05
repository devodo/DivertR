using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface IFoo
    {
        string Name { get; set; }
        public object? LastAction { get; set; }

        Task<string> GetNameAsync();
        ValueTask<string> GetNameValueAsync();
        ValueTask<string> GetNameValueSync();

        string Echo(string input);

        T1 EchoGeneric<T1>(T1 i1);
        T1 EchoGenericAlt<T1>(T1 i1);
        (T1, T2) EchoGeneric<T1, T2>(T1 i1, T2 i2);
        (T1, T2, T3) EchoGeneric<T1, T2, T3>(T1 i1, T2 i2, T3 i3);
        (T1, T2, T3, T4) EchoGeneric<T1, T2, T3, T4>(T1 i1, T2 i2, T3 i3, T4 i4);
        (T1, T2, T3, T4, T5) EchoGeneric<T1, T2, T3, T4, T5>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5);
        (T1, T2, T3, T4, T5, T6) EchoGeneric<T1, T2, T3, T4, T5, T6>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6);
        (T1, T2, T3, T4, T5, T6, T7) EchoGeneric<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7);
        (T1, T2, T3, T4, T5, T6, T7, T8) EchoGeneric<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7, T8 i8);

        void GenericAction<T1>(T1 i1);
        void GenericAction<T1, T2>(T1 i1, T2 i2);
        void GenericAction<T1, T2, T3>(T1 i1, T2 i2, T3 i3);
        void GenericAction<T1, T2, T3, T4>(T1 i1, T2 i2, T3 i3, T4 i4);
        void GenericAction<T1, T2, T3, T4, T5>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5);
        void GenericAction<T1, T2, T3, T4, T5, T6>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6);
        void GenericAction<T1, T2, T3, T4, T5, T6, T7>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7);
        void GenericAction<T1, T2, T3, T4, T5, T6, T7, T8>(T1 i1, T2 i2, T3 i3, T4 i4, T5 i5, T6 i6, T7 i7, T8 i8);

        Task<string> EchoAsync(string input);
        T EchoGenericRef<T>(ref T input);
        ValueTask<string> EchoValueAsync(string input);
        ValueTask<string> EchoValueSync(string input);
        
        void SetName(string name);
        void SetName(Task<string> name);

        string SetName(Wrapper<string> input);

        IFoo GetFoo();
    }
}