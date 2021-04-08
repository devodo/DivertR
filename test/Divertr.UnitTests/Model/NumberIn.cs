namespace DivertR.UnitTests.Model
{
    public class NumberIn : INumberIn
    {
        public int GetNumber(in int input)
        {
            return input;
        }
    }
}