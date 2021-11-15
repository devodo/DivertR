using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Record;
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
        public void GivenProxyCalls_ShouldRecord()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var spiedCalls = _via
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Redirect<(Guid input, __)>(_ => Guid.NewGuid())
                .Spy(call => new { Input = call.Args.input, Returned = call.Returned!.Value });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            spiedCalls.Count.ShouldBe(inputs.Count);
            spiedCalls.Select(x => x.Input).ShouldBe(inputs);
            spiedCalls.Select(x => x.Returned).ShouldBe(outputs);
        }
    }
}