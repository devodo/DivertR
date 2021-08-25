using System.Linq;
using DivertR.Extensions;
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
            var proxy = _via
                .To(x => x.Name).Redirect("hello")
                .To(x => x.Name).Repeat(1).Redirect(() => $"{_via.Relay.Next.Name} first")
                .To(x => x.Name).Repeat(Count + 1).Redirect(() => $"{_via.Relay.Next.Name} diverted")
                .Proxy();

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
            var proxy = _via
                .To(x => x.Name).Redirect("hello")
                .To(x => x.Name).Skip(1).Redirect(() => $"{_via.Relay.Next.Name} after")
                .To(x => x.Name).Skip(Count).Redirect(() => $"{_via.Relay.Next.Name} diverted")
                .Proxy();

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
            var proxy = _via
                .To(x => x.Name).Redirect("hello")
                .To(x => x.Name).Repeat(1).Skip(1).Redirect(() => $"{_via.Relay.Next.Name} after")
                .To(x => x.Name).Skip(Count).Redirect(() => $"{_via.Relay.Next.Name} diverted")
                .Proxy();

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