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
        private readonly IVia<IFoo> _via = Via.For<IFoo>();
        private readonly IFoo _proxy;

        public SpyTests()
        {
            _proxy = _via.Proxy(new Foo());
        }

        [Fact]
        public void GivenStructSpyRedirect_ShouldRecordAndMapCalls()
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
        public void GivenStructSpyWithNoRedirect_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _via
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .WithArgs<(Guid input, __)>()
                .Spy(call => new { Input = call.Args.input, Returned = call.Returned?.IsValue == true ? call.Returned.Value : Guid.Empty });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenStructSpyWithNoRedirectAndNoArgs_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _via
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Spy(call => new { Input = (Guid) call.Args[0], Returned = call.Returned?.IsValue == true ? call.Returned.Value : Guid.Empty });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenStructSpyRedirect_WhenException_ShouldRecordAndMapException()
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

            var numberVia = Via.For<INumber>();
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
            inputs.Select(x => x * 10).ShouldBe(outputs);
            results.Select(x => x.input).ShouldBe(inputs);
            results.Select(x => x.output).ShouldBe(outputs);
        }
        
        [Fact]
        public async Task GivenTaskSpyRedirect_ShouldScanAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var spy = _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect<(string input, __)>(call => Task.FromResult(call.Args.input + " diverted"))
                .Spy(async call => new
                {
                    Input = call.Args.input,
                    Result = await call.Returned!.Value
                });

            // ACT
            var results = new List<string>(inputs.Count);
            foreach (var input in inputs)
            {
                results.Add(await _proxy.EchoAsync("test" + input));
            }
            
            // ASSERT
            results.ShouldBe(inputs.Select(x => $"test{x} diverted"));
            spy.Count.ShouldBe(inputs.Count);

            var count = 0;
            (await spy.ScanAsync(call =>
            {
                call.Input.ShouldBe($"test{count}");
                call.Result.ShouldBe($"test{count++} diverted");
            })).ShouldBe(inputs.Count);
            
            (await spy.ScanAsync((call, i) =>
            {
                call.Input.ShouldBe($"test{i}");
                call.Result.ShouldBe($"test{i} diverted");
            })).ShouldBe(inputs.Count);
        }
        
        [Fact]
        public async Task GivenSpyRedirect_ShouldScanAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var spy = _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect<(string input, __)>(call => Task.FromResult(call.Args.input + " diverted"))
                .Spy(call => new
                {
                    Input = call.Args.input,
                    Result = call.Returned!.Value
                });

            // ACT
            var results = new List<string>(inputs.Count);
            foreach (var input in inputs)
            {
                results.Add(await _proxy.EchoAsync("test" + input));
            }
            
            // ASSERT
            results.ShouldBe(inputs.Select(x => $"test{x} diverted"));
            spy.Count.ShouldBe(inputs.Count);

            var count = 0;
            (await spy.ScanAsync(async call =>
            {
                call.Input.ShouldBe($"test{count}");
                (await call.Result).ShouldBe($"test{count++} diverted");
            })).ShouldBe(inputs.Count);
            
            (await spy.ScanAsync(async (call, i) =>
            {
                call.Input.ShouldBe($"test{i}");
                (await call.Result).ShouldBe($"test{i} diverted");
            })).ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenCallConstraintSpy_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();

            var echoes = _via
                .To(new CallConstraint<IFoo>(call => call.Method.Name == nameof(IFoo.Echo)))
                .Spy((_, args) => new { Input = args[0] });

            // ACT
            var outputs = inputs.Select(x => _proxy.Echo(x)).ToList();

            // ASSERT
            outputs.ShouldBe(inputs.Select(x => $"{_proxy.Name}: {x}"));
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
        }
        
        [Fact]
        public async Task GivenClassRedirectBuilderWithNoArgs_ShouldSpy()
        {
            // ARRANGE
            var spy = _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Spy(async call => new
                {
                    Input = (string) call.Args[0],
                    Result = await call.Returned!.Value
                });

            // ACT
            var result = await _proxy.EchoAsync("test");
            
            // ASSERT
            await spy.ScanAsync(call =>
            {
                result.ShouldBe("original: test");
                call.Input.ShouldBe("test");
                call.Result.ShouldBe(result);
            });
        }
    }
}