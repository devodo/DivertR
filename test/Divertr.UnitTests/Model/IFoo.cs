namespace DivertR.UnitTests.Model
{
    public interface IFoo
    {
        string Message { get; }

        string GetMessage(string input);
    }
}