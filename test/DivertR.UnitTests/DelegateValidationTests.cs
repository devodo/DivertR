using System;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DelegateValidationTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();

        [Fact]
        public void GivenTooManyRedirectParameters_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new Func<string, int, string>((input, i) => input));

            // ASSERT
            testAction.ShouldThrow<InvalidRedirectException>();
        }
        
        [Fact]
        public void GivenDelegateWithInvalidParameterType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new Func<int, string>(input => input.ToString()));

            // ASSERT
            testAction.ShouldThrow<InvalidRedirectException>();
        }
        
        [Fact]
        public void GivenDelegateWithInvalidReturnType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new Func<string, int>(input => 0));

            // ASSERT
            testAction.ShouldThrow<InvalidRedirectException>();
        }
    }
}