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
            calls.Replay(call =>
            {
                result.ShouldBe(_foo.Name);
                call.Args.Count.ShouldBe(0);
                call.Returned?.Value.ShouldBe(_foo.Name);
            }).ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordRedirectWithArgs_WhenCalled_ThenRecordsCallWithArgs()
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
            
            calls.Replay(call =>
            {
                call.Args.Count.ShouldBe(1);
                call.Returned?.Value.ShouldBe(_foo.Echo((string) call.Args[0]));
            }).ShouldBe(inputs.Length);
            
            calls.Replay((call, args) =>
            {
                args.Count.ShouldBe(1);
                call.Returned?.Value.ShouldBe(_foo.Echo((string) args[0]));
            }).ShouldBe(inputs.Length);
            
            calls.WithArgs<(string input, __)>().Replay((call, args, i) =>
            {
                args.input.ShouldBe(inputs[i]);
                call.Returned?.Value.ShouldBe(_foo.Echo(args.input));
            }).ShouldBe(inputs.Length);
        }
    }
}