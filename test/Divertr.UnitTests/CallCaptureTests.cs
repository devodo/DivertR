using System;
using System.Linq;
using DivertR.Core;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class CallCaptureTests
    {
        private readonly Via<IFoo> _via = new();
        private readonly ICallCapture<IFoo> _callCapture;

        public CallCaptureTests()
        {
            _callCapture = _via.CaptureCalls();
        }
        
        [Fact]
        public void GivenProxyCalls_ShouldCapture()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(x => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .Redirect(x => x.GetMessage(Is<string>.Any))
                .To(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();
            var outputs = inputs.Select(x => fooProxy.GetMessage(x)).ToList();

            // ACT
            var calls = _callCapture.Calls();

            // ASSERT
            calls.Select(x => (string) x.CallInfo.Arguments[0]).ShouldBe(inputs);
            calls.Select(x => (string) x.ReturnValue).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenGetCallConstraint_ShouldFilter()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(x => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .Redirect(x => x.GetMessage(Is<string>.Any))
                .To(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();
            var outputs = inputs.Select(x => fooProxy.GetMessage(x)).ToList();

            // ACT
            var calls = _callCapture.Calls(x => x.GetMessage(inputs[0]));

            // ASSERT
            calls.Count.ShouldBe(1);
            calls[0].CallInfo.Arguments.Count.ShouldBe(1);
            calls[0].CallInfo.Arguments[0].ShouldBe(inputs[0]);
            calls[0].ReturnValue.ShouldBe(outputs[0]);
            calls[0].CallInfo.Proxy.ShouldBeSameAs(fooProxy);
        }
    }
}