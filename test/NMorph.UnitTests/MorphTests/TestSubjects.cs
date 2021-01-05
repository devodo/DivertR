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
        private readonly IInvocationContext<ITestSubject> _invocationContext;

        public SubstituteTest(string message, IInvocationContext<ITestSubject> invocationContext)
        {
            _message = message;
            _invocationContext = invocationContext;
        }

        public string Message => _invocationContext.Previous?.Message + _message;
    }
}