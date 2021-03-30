namespace DivertR.UnitTests.Model
{
    public interface IFoo
    {
        string Message { get; set; }

        string GetMessage(string input);
        
        string SetMessage(Wrapper<string> input);

        IFoo GetFoo();
    }
}