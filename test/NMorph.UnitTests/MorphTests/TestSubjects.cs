namespace NMorph.UnitTests.MorphTests
{
    public interface ITestSubject
    {
        string Message { get; }
    }
    
    public class TestA : ITestSubject
    {
        public string Message { get; }

        public TestA(string message)
        {
            Message = message;
        }
    }
    
    public class SubstituteTest : ITestSubject
    {
        private readonly string _message;
        private readonly ICallContext<ITestSubject> _callContext;

        public SubstituteTest(string message, ICallContext<ITestSubject> callContext = null)
        {
            _message = message;
            _callContext = callContext;
        }

        public string Message => _callContext.Previous?.Message + _message;
    }
}