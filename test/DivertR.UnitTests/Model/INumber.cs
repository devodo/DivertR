using System.Threading.Tasks;

namespace DivertR.UnitTests.Model
{
    public interface INumber
    {
        int GetNumber(int input);
        string GenericNumber<T>(T arg, int input);
        void ArrayNumber(int[] inputs);
        void RefArrayNumber(ref int[] inputs);
        int RefNumber(ref int input);
        void OutNumber(int input, out int output);
        int RefOutNumber(ref int input, out int output);
        
        Task<int> GetNumberAsync(int input);
        ValueTask<int> GetNumberValueAsync(int input);
    }
}