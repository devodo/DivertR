﻿using System;
using System.Linq;
using System.Threading.Tasks;
using DivertR.Redirects;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecordCallsTests
    {
        private readonly Via<IFoo> _via = new();
        private readonly ICallStream<IFoo> _callStream;

        public RecordCallsTests()
        {
            _callStream = _via.Record();
        }
        
        [Fact]
        public void GivenProxyCalls_ShouldCapture()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();
            

            // ACT
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ASSERT
            _callStream.Select(x => (string) x.CallInfo.Arguments[0]).ShouldBe(inputs);
            _callStream.Select(x => (string) x.Returned?.Value).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenGetCallConstraint_ShouldFilter()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select((i, _) => $"{i}")
                .ToList();
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => Guid.NewGuid().ToString());

            var fooProxy = _via.Proxy();
            var outputs = inputs.Select(x => fooProxy.Echo(x)).ToList();

            // ACT
            var calls = _callStream.To(x => x.Echo(inputs[0]));

            // ASSERT
            calls.Count.ShouldBe(1);
            calls.Single().CallInfo.Arguments.Count.ShouldBe(1);
            calls.Single().CallInfo.ViaProxy.ShouldBeSameAs(fooProxy);
            
            calls.Visit<string>((input, callReturn) =>
            {
                input.ShouldBe(inputs[0]);
                callReturn.Value.ShouldBe(outputs[0]);
            });
        }
        
        [Fact]
        public void GivenRecordCalls_WhenException_ShouldRecordException()
        {
            // ARRANGE
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(() => throw new Exception("test"));

            // ACT
            Exception caughtException = null;
            try
            {
                _via.Proxy().Echo("test");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // ASSERT
            var call = _callStream.To(x => x.Echo("test")).Single();
            caughtException.ShouldNotBeNull();
            call.Returned!.Exception.ShouldBeSameAs(caughtException);
        }
        
        [Fact]
        public async Task GivenRecordCalls_WhenAsyncException_ShouldRecordReturnTask()
        {
            // ARRANGE
            _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect(async () =>
                {
                    await Task.Yield();
                    throw new Exception("test");
                });

            // ACT
            Exception caughtException = null;
            try
            {
                await _via.Proxy().EchoAsync("test");
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            // ASSERT
            var call = _callStream.To(x => x.EchoAsync("test")).Single();
            caughtException.ShouldNotBeNull();
            call.Returned!.Exception.ShouldBeNull();
            
            Exception returnedException = null;
            try
            {
                await call.Returned!.Value;
            }
            catch (Exception ex)
            {
                returnedException = ex;
            }
            
            returnedException.ShouldBeSameAs(caughtException);
        }
    }
}