using System;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();

        [Fact]
        public void GivenNoRedirects_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenValidRootObjectType_WhenCreateProxyObject_ShouldCreateProxy()
        {
            // ARRANGE
            var original = new Foo();

            // ACT
            var proxy = (IFoo) ((IVia) _via).Proxy(original);

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenProxyWithNullRoot_WhenProxyMemberCalled_ShouldThrowException()
        {
            // ARRANGE
            var proxy = _via.Proxy(null);

            // ACT
            Func<object> testAction = () => proxy.Name;

            // ASSERT
            testAction.ShouldThrow<DiverterNullRootException>();
        }
        
        [Fact]
        public void GivenInvalidRootObjectType_WhenCreateProxyObject_ShouldThrowArgumentException()
        {
            // ARRANGE
            var invalidOriginal = new object();

            // ACT
            Func<object> testAction = () => _via.Proxy(invalidOriginal);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GivenStrictModeWithNoRedirect_ShouldThrowException()
        {
            // ARRANGE
            _via.Strict();
            var proxy = _via.Proxy(new Foo("hello foo"));

            // ACT
            Func<string> testAction = () => proxy.Name;

            // ASSERT
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
        
        [Fact]
        public void GivenStrictModeWithRedirect_ShouldNotThrowException()
        {
            // ARRANGE
            _via.Strict();
            _via.To(x => x.Name).Redirect("divert");
            var proxy = _via.Proxy(new Foo("hello foo"));

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("divert");
        }
    }
}