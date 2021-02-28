using System;

namespace DivertR.UnitTests.Model
{
    public class Number : INumber
    {
        private readonly Func<int, int> _numberFactory;

        public Number() : this(i => i)
        {
        }

        public Number(Func<int, int> numberFactory)
        {
            _numberFactory = numberFactory;
        }

        public int GetNumber(int input)
        {
            return _numberFactory.Invoke(input);
        }
    }
}