using System.Threading.Tasks;
using DivertR.UnitTests.Model;

namespace DivertR.UnitTests
{
    public class NumberIn : INumberIn
    {
        public int GetNumber(in int input)
        {
            return input;
        }
    }
}