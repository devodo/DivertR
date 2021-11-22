using System;
using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class SpyTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IFoo _proxy;

        public SpyTests()
        {
            _proxy = _via.Proxy();
        }

        [Fact]
        public void GivenSpyRedirect_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _via
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Redirect<(Guid input, __)>(_ => Guid.NewGuid())
                .Spy(call => new { Input = call.Args.input, Returned = call.Returned?.IsValue == true ? call.Returned.Value : Guid.Empty });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenSpyRedirect_WhenException_ShouldRecordAndMapException()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var builder = _via.To(x => x.EchoGeneric(Is<Guid>.Any));
            var echoes = builder
                .Redirect<(Guid input, __)>(call => throw new Exception($"{call.Args.input}"))
                .Spy(call => new
                {
                    Input = call.Args.input,
                    Returned = call.Returned?.IsValue == true ? call.Returned.Value : Guid.Empty,
                    Exception = call.Returned?.IsException == true ? call.Returned.Exception : null
                });

            builder.Redirect(call =>
            {
                try
                {
                    return call.Relay.CallNext();
                }
                catch (Exception)
                {
                    return Guid.Empty;
                }
            });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Count(x => x.Exception.Message == $"{x.Input}").ShouldBe(inputs.Count);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenSpyRedirect_WhenOutCall_ShouldRecordAndMap()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();

            var numberVia = new Via<INumber>();
            var numberProxy = numberVia.Proxy(new Number(x => x * 10));

            var results = numberVia
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Redirect<(int input, Ref<int> output)>(call => call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value))
                .Spy(call => new { call.Args.input, output = call.Args.output.Value });

            // ACT
            var outputs = inputs.Select(i =>
            {
                numberProxy.OutNumber(i, out var output);
                return output;
            }).ToList();

            // ASSERT
            results.Select(x => x.input).ShouldBe(inputs);
            results.Select(x => x.output).ShouldBe(outputs);
        }
    }
}