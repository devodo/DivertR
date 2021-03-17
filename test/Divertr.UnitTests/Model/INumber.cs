namespace DivertR.UnitTests.Model
{
    public interface INumber
    {
        int GetNumber(int input);

        void RefNumber(ref int input);

        string GenericNumber<T>(T arg, int input);
        
        void ArrayNumber(int[] inputs);
        void RefArrayNumber(ref int[] inputs);
    }
}