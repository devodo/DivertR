using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaOptionsBuilderTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();

        [Fact]
        public void GivenRepeatExtension_ShouldViaNumberOfTimes()
        {
            // ARRANGE
            const int Count = 5;
            _redirect
                .To(x => x.Name)
                .Via("hello")
                .Via(() => $"{_redirect.Relay.Next.Name} first", options => options.Repeat(1))
                .Via(() => $"{_redirect.Relay.Next.Name} diverted", options => options.Repeat(Count + 1));

            var proxy = _redirect.Proxy();

            // ACT
            var results = Enumerable.Range(0, Count + 2).Select(_ => proxy.Name).ToList();

            // ASSERT
            results.First().ShouldBe("hello first diverted");
            results.Last().ShouldBe("hello");
            results.Skip(1).Take(Count).ShouldAllBe(x => x == "hello diverted");
        }
        
        [Fact]
        public void GivenSkipExtension_ShouldViaAfterNumberOfTimes()
        {
            // ARRANGE
            const int Count = 5;
            _redirect
                .To(x => x.Name)
                .Via("hello")
                .Via(() => $"{_redirect.Relay.Next.Name} after", options => options.Skip(1))
                .Via(() => $"{_redirect.Relay.Next.Name} diverted", options => options.Skip(Count));
            
            var proxy = _redirect.Proxy();

            // ACT
            var results = Enumerable.Range(0, Count + 1).Select(_ => proxy.Name).ToList();

            // ASSERT
            results.First().ShouldBe("hello");
            results.Last().ShouldBe("hello after diverted");
            results.Skip(1).Take(Count - 1).ShouldAllBe(x => x == "hello after");
        }
        
        [Fact]
        public void GivenSkipAndRepeatExtensions_ShouldViaAfterAndNumberOfTimes()
        {
            // ARRANGE
            const int Count = 5;
            _redirect
                .To(x => x.Name)
                .Via("hello")
                .Via(() => $"{_redirect.Relay.Next.Name} after", options => options.Repeat(1).Skip(1))
                .Via(() => $"{_redirect.Relay.Next.Name} diverted", options => options.Skip(Count));
            
            var proxy = _redirect.Proxy();

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