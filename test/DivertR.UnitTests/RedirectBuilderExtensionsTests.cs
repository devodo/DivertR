using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectBuilderExtensionsTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();

        [Fact]
        public void GivenRepeatExtension_ShouldRedirectNumberOfTimes()
        {
            // ARRANGE
            const int Count = 5;
            _via
                .To(x => x.Name)
                .Redirect("hello")
                .Redirect(() => $"{_via.Relay.Next.Name} first", options => options.Repeat(1))
                .Redirect(() => $"{_via.Relay.Next.Name} diverted", options => options.Repeat(Count + 1));

            var proxy = _via.Proxy();

            // ACT
            var results = Enumerable.Range(0, Count + 2).Select(_ => proxy.Name).ToList();

            // ASSERT
            results.First().ShouldBe("hello first diverted");
            results.Last().ShouldBe("hello");
            results.Skip(1).Take(Count).ShouldAllBe(x => x == "hello diverted");
        }
        
        [Fact]
        public void GivenSkipExtension_ShouldRedirectAfterNumberOfTimes()
        {
            // ARRANGE
            const int Count = 5;
            _via
                .To(x => x.Name)
                .Redirect("hello")
                .Redirect(() => $"{_via.Relay.Next.Name} after", options => options.Skip(1))
                .Redirect(() => $"{_via.Relay.Next.Name} diverted", options => options.Skip(Count));
            
            var proxy = _via.Proxy();

            // ACT
            var results = Enumerable.Range(0, Count + 1).Select(_ => proxy.Name).ToList();

            // ASSERT
            results.First().ShouldBe("hello");
            results.Last().ShouldBe("hello after diverted");
            results.Skip(1).Take(Count - 1).ShouldAllBe(x => x == "hello after");
        }
        
        [Fact]
        public void GivenSkipAndRepeatExtensions_ShouldRedirectAfterAndNumberOfTimes()
        {
            // ARRANGE
            const int Count = 5;
            _via
                .To(x => x.Name)
                .Redirect("hello")
                .Redirect(() => $"{_via.Relay.Next.Name} after", options => options.Repeat(1).Skip(1))
                .Redirect(() => $"{_via.Relay.Next.Name} diverted", options => options.Skip(Count));
            
            var proxy = _via.Proxy();

            // ACT
            var results = Enumerable.Range(0, Count + 1).Select(_ => proxy.Name).ToList();

            // ASSERT
            results.First().ShouldBe("hello");
            results.Skip(1).First().ShouldBe("hello after");
            results.Skip(2).Take(Count - 2).ShouldAllBe(x => x == "hello");
            results.Last().ShouldBe("hello diverted");
        }
    }
}