using System.Threading;

namespace DivertR.UnitTests.Model;

public class BarFactory : IBarFactory
{
    private int _barCount;
    
    public IBar Create(string name)
    {
        var barNumber = Interlocked.Increment(ref _barCount);

        return new Bar($"Bar {barNumber}");
    }
}