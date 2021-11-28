using System;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ValueTupleValidationTests
    {
        private readonly IVia<IFoo> _via = Via.For<IFoo>();
        
        [Fact]
        public void GivenInvalidRedirectParameterType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect<(int input, __)>(call => call.Args.input.ToString());

            // ASSERT
            testAction.ShouldThrow<InvalidRedirectException>();
        }
        
        [Fact]
        public void GivenTooManyRedirectParameters_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect<(string input, int i)>(call => call.Args.input);

            // ASSERT
            testAction.ShouldThrow<InvalidRedirectException>();
        }
    }
}