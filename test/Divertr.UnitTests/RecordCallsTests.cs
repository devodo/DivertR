using System;
using System.Linq;
using DivertR.Redirects;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecordCallsTests
    {
        private readonly Via<IFoo> _via = new();
        private readonly ICallsRecord<IFoo> _callsRecord;

        public RecordCallsTests()
        {
            _callsRecord = _via.Record();
        }
        
        [Fact]
        public void GivenProxyCalls_ShouldCapture()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(x => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .Redirect(x => x.Echo(Is<string>.Any))
                .To(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ACT
            var calls = _callsRecord.All;

            // ASSERT
            calls.Select(x => (string) x.CallInfo.Arguments[0]).ShouldBe(inputs);
            calls.Select(x => (string) x.ReturnObject).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenGetCallConstraint_ShouldFilter()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select((i, x) => $"{i}")
                .ToList();
            
            _via
                .Redirect(x => x.Echo(Is<string>.Any))
                .To(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ACT
            var calls = _callsRecord.Matching(x => x.Echo(inputs[0]));

            // ASSERT
            calls.Count.ShouldBe(1);
            calls[0].CallInfo.Arguments.Count.ShouldBe(1);
            calls[0].CallInfo.Arguments[0].ShouldBe(inputs[0]);
            calls[0].ReturnValue.ShouldBe(outputs[0]);
            calls[0].CallInfo.ViaProxy.ShouldBeSameAs(fooProxy);
        }
    }
}