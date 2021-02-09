namespace Divertr.UnitTests.Model
{
    public class FooSubstitute : IFoo
    {
        private readonly string _message;
        private readonly IFoo _replacedSubject;

        public FooSubstitute(string message, IFoo replacedSubject = null)
        {
            _message = message;
            _replacedSubject = replacedSubject;
        }

        public string Message => _replacedSubject?.Message + _message;
    }
}