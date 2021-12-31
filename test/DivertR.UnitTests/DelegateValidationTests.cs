using System;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class DelegateValidationTests
    {
        private readonly ITestOutputHelper _output;
        private readonly IVia<IFoo> _via = new Via<IFoo>();

        public DelegateValidationTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void GivenTooManyRedirectParameters_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new Func<string, int, string>((input, _) => input));

            // ASSERT
            _output.WriteLine(testAction.ShouldThrow<DiverterValidationException>().Message);
        }
        
        [Fact]
        public void GivenTooFewRedirectParameters_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.EchoGeneric(Is<string>.Any, Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new Func<string, (string, string)>(input => (input, "")));

            // ASSERT
            _output.WriteLine(testAction.ShouldThrow<DiverterValidationException>().Message);
        }
        
        [Fact]
        public void GivenDelegateWithInvalidParameterType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new Func<int, string>(input => input.ToString()));

            // ASSERT
            _output.WriteLine(testAction.ShouldThrow<DiverterValidationException>().Message);
        }
        
        [Fact]
        public void GivenDelegateWithInvalidReturnType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new Func<string, int>(_ => 0));

            // ASSERT
            _output.WriteLine(testAction.ShouldThrow<DiverterValidationException>().Message);
        }
        
        [Fact]
        public void GivenPropertyRedirect_WhenDelegateWithInvalidReturnType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Name);

            // ACT
            Action testAction = () => builder.Redirect(new Func<int>(() => 0));

            // ASSERT
            _output.WriteLine(testAction.ShouldThrow<DiverterValidationException>().Message);
        }
        
        delegate void RefDelegate(int input, ref int output);
        
        [Fact]
        public void GivenDelegateWithInvalidRefTypeParameter_ShouldThrowException()
        {
            // ARRANGE
            var via = new Via<INumber>();
            var builder = via.To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any));

            // ACT
            Action testAction = () => builder.Redirect(new RefDelegate((int _, ref int _) => { }));
            
            // ASSERT
            _output.WriteLine(testAction.ShouldThrow<DiverterValidationException>().Message);
        }
    }
}