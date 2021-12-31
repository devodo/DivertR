using System;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace DivertR.UnitTests
{
    public class ValueTupleValidationTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly ITestOutputHelper _output;

        public ValueTupleValidationTests(ITestOutputHelper output)
        {
            _output = output;
        }
        
        [Fact]
        public void GivenInvalidArgumentType_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect<(int input, __)>(call => call.Args.input.ToString());

            // ASSERT
            _output.WriteLine($"{testAction.ShouldThrow<DiverterValidationException>()}");
        }
        
        [Fact]
        public void GivenTooManyArgumentTypes_ShouldThrowException()
        {
            // ARRANGE
            var builder = _via.To(x => x.Echo(Is<string>.Any));

            // ACT
            Action testAction = () => builder.Redirect<(string input, int i)>(call => call.Args.input);

            // ASSERT
            _output.WriteLine($"{testAction.ShouldThrow<DiverterValidationException>()}");
        }
        
        [Fact]
        public void GivenLessArgumentTypesThanParameters_ShouldNotThrowException()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<int>.Any, Is<int>.Any))
                .Redirect<(int input, __)>((_, args) => (args.input, 0));

            // ACT
            var result = _via.Proxy().EchoGeneric(10, 100);

            // ASSERT
            result.ShouldBe((10, 0));
        }
        
        [Fact]
        public void GivenNonRefTypeForOutParameter_ShouldThrowException()
        {
            // ARRANGE
            var builder = new Via<INumber>().To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any));

            // ACT
            Action testAction = () => builder.Redirect<(int input, int output)>(_ => { });

            // ASSERT
            _output.WriteLine($"{testAction.ShouldThrow<DiverterValidationException>()}");
        }
    }
}