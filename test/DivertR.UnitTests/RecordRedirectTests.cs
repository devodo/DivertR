using System;
using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecordRedirectTests
    {
        private readonly IVia<IFoo> _fooVia = new Via<IFoo>();
        private readonly IFoo _proxy;
        private readonly IFoo _foo = new Foo();

        public RecordRedirectTests()
        {
            _proxy = _fooVia.Proxy(_foo);
        }

        [Fact]
        public void GivenRecordGetParamRedirect_WhenGet_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.Name)
                .Record();

            // ACT
            var result = _proxy.Name;

            // ASSERT
            calls.Verify(call =>
            {
                result.ShouldBe(_foo.Name);
                call.Args.Count.ShouldBe(0);
                call.Returned?.Value.ShouldBe(_foo.Name);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordRedirectWithArgs_WhenCalled_ThenRecordsCallsWithArgs()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Record();

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => _foo.Echo(input)));
            calls.Select(call => call.Args[0]).ShouldBe(inputs);
            
            calls.Verify(call =>
            {
                call.Args.Count.ShouldBe(1);
                call.Returned?.Value.ShouldBe(_foo.Echo((string) call.Args[0]));
            }).Count.ShouldBe(inputs.Length);
            
            calls.Verify((call, args) =>
            {
                args.Count.ShouldBe(1);
                call.Returned?.Value.ShouldBe(_foo.Echo((string) args[0]));
            }).Count.ShouldBe(inputs.Length);
            
            calls.WithArgs<(string input, __)>().Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                call.Returned?.Value.ShouldBe(_foo.Echo(call.Args.input));
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenFuncCallLogWithRelayRootRedirect_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>((call, args) => $"{call.Root.Echo(args.input)} redirected")
                .Record();

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} redirected"));

            var count = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count]);
                call.Returned?.Value.ShouldBe(results[count++]);
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify((call, args) =>
            {
                args.input.ShouldBe(inputs[count2]);
                call.Returned?.Value.ShouldBe(results[count2++]);
            }).Count.ShouldBe(inputs.Length);

            calls.Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                call.Returned?.Value.ShouldBe(results[i]);
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenFuncCallLogWithRelayNextRedirect_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>((call, args) => $"{call.Next.Echo(args.input)} redirected")
                .Record();

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} redirected"));

            var count = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count]);
                call.Returned?.Value.ShouldBe(results[count++]);
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify((call, args) =>
            {
                args.input.ShouldBe(inputs[count2]);
                call.Returned?.Value.ShouldBe(results[count2++]);
            }).Count.ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenActionCallLogWithRelayRootRedirect_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooVia
                .To(x => x.SetName(Is<string>.Any))
                .Redirect<(string input, __)>((call, args) => call.Root.SetName($"{args.input} redirected"))
                .Record();

            // ACT
            var results = inputs.Select(input =>
            {
                _proxy.SetName(input);
                return _foo.Name;
            }).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{input} redirected"));

            var count = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count++]);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify((call, args) =>
            {
                args.input.ShouldBe(inputs[count2++]);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenActionCallLogWithRelayNextRedirect_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooVia
                .To(x => x.SetName(Is<string>.Any))
                .Redirect<(string input, __)>((call, args) => call.Next.SetName($"{args.input} redirected"))
                .Record();

            // ACT
            var results = inputs.Select(input =>
            {
                _proxy.SetName(input);
                return _foo.Name;
            }).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{input} redirected"));

            var count = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count++]);
                call.Returned.ShouldNotBeNull();
                call.Returned!.Exception.ShouldBeNull();
                call.Returned!.Value.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify((call, args) =>
            {
                args.input.ShouldBe(inputs[count2++]);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenRecordBeforeRelayRootRedirect_WhenCalled_ThenDoesNotRecord()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var redirectBuilder = _fooVia.To(x => x.Echo(Is<string>.Any));
            var calls = redirectBuilder.Record();
            redirectBuilder
                .Redirect<(string input, __)>((call, args) => $"{call.Root.Echo(args.input)} redirected");

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} redirected"));
            calls.Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenRecordBeforeRelayNextRedirect_WhenCalled_ThenRecordsCallAfterRedirect()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var redirectBuilder = _fooVia.To(x => x.Echo(Is<string>.Any));
            var calls = redirectBuilder.Record<(string input, __)>();
            redirectBuilder
                .Redirect<(string input, __)>((call, args) =>
                {
                    var modifiedInput = $"{args.input} modified";
                    return $"{call.Next.Echo(modifiedInput)} redirected";
                });

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} modified redirected"));
            calls.Verify().Select((call, i) =>
            {
                call.Args.input.ShouldBe($"{inputs[i]} modified");
                call.Returned?.Value.ShouldBe($"{_foo.Echo(inputs[i])} modified");
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenOrderFirstRecordBeforeRelayRootRedirect_WhenCalled_ThenRecordsCall()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var redirectBuilder = _fooVia.To(x => x.Echo(Is<string>.Any));
            var recordedCalls = redirectBuilder.Record<(string input, __)>(opt => opt.OrderFirst());
            redirectBuilder
                .Redirect<(string input, __)>((call, args) =>
                {
                    var modifiedInput = $"{args.input} modified";
                    return $"{call.Next.Echo(modifiedInput)} redirected";
                });

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} modified redirected"));
            recordedCalls.Verify().Select((call, i) =>
            {
                call.Args.input.ShouldBe($"{inputs[i]}");
                call.Returned?.Value.ShouldBe($"{results[i]}");
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenOrderLastRedirectAfterRecord_WhenCalled_ThenRecordsCall()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var redirectBuilder = _fooVia.To(x => x.Echo(Is<string>.Any));
            var recordedCalls = redirectBuilder.Record<(string input, __)>();
            redirectBuilder
                .Redirect<(string input, __)>((call, args) =>
                {
                    var modifiedInput = $"{args.input} modified";
                    return $"{call.Next.Echo(modifiedInput)} redirected";
                }, opt => opt.OrderLast());

            // ACT
            var result = inputs.Select(input => _proxy.Echo(input)).Count();

            // ASSERT
            recordedCalls.Count.ShouldBe(result);
        }
    }
}