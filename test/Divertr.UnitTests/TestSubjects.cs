namespace Divertr.UnitTests
{
    public interface IFoo
    {
        string Message { get; }
    }

    public class Foo : IFoo
    {
        public Foo() {}
        public Foo(string message)
        {
            Message = message;
        }
        
        public string Message { get; init; }
    }

    public class SubstituteTest : IFoo
    {
        private readonly string _message;
        private readonly IFoo _replacedSubject;

        public SubstituteTest(string message, IFoo replacedSubject = null)
        {
            _message = message;
            _replacedSubject = replacedSubject;
        }

        public string Message => _replacedSubject?.Message + _message;
    }
}