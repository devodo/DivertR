using System;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class ValueTupleValidationTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        private readonly ITestOutputHelper _output;

        public ValueTupleValidationTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void GivenInvalidArgumentType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _redirect.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Via<(int input, __)>(call => call.Args.input.ToString());

            // ASSERT
            _output.WriteLine($"{testAction.ShouldThrow<DiverterValidationException>()}");
        }
        
        [Fact]
        public void GivenInvalidRefArgumentType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _redirect.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Via<(Ref<int> input, __)>(call => call.Args.input.Value.ToString());

            // ASSERT
            _output.WriteLine($"{testAction.ShouldThrow<DiverterValidationException>()}");
        }
        
        [Fact]
        public void GivenTooManyArgumentTypes_ShouldThrowException()
        {
            // ARRANGE
            var builder = _redirect.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Via<(string input, int i)>(call => call.Args.input);

            // ASSERT
            _output.WriteLine($"{testAction.ShouldThrow<DiverterValidationException>()}");
        }
        
        [Fact]
        public void GivenLessArgumentTypesThanParameters_ShouldNotThrowException()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<int>.Any, Is<int>.Any))
                .Via<(int input, __)>(call => (call.Args.input, 0));

            // ACT
            var result = _redirect.Proxy().EchoGeneric(10, 100);

            // ASSERT
            result.ShouldBe((10, 0));
        }
        
        [Fact]
        public void GivenNonRefTypeForOutParameter_ShouldThrowException()
        {
            // ARRANGE
            var builder = new Redirect<INumber>().To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any));

            // ACT
            Action testAction = () => builder.Via<(int input, int output, __)>(_ => { });

            // ASSERT
            _output.WriteLine($"{testAction.ShouldThrow<DiverterValidationException>()}");
        }
    }
}