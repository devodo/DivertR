namespace DivertR.UnitTests.Model;

public class Bar : IBar
{
    public Bar(string name)
    {
        Name = name;
    }
    
    public string Name { get; }
}