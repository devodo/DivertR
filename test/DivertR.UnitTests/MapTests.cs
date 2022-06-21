using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class MapTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IFoo _proxy;

        public MapTests()
        {
            _proxy = _via.Proxy(new Foo());
        }

        [Fact]
        public void GivenStructMapRedirect_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _via
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Redirect<(Guid input, __)>(_ => Guid.NewGuid())
                .Record()
                .Map(call => new { Input = call.Args.input, Returned = call.Returned?.Value ?? Guid.Empty });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenStructMapWithNoRedirect_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _via
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Record<(Guid input, __)>()
                .Map(call => new { Input = call.Args.input, Returned = call.Returned?.Value ?? Guid.Empty });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
            echoes.Verify().Select((call, i) =>
            {
                call.Input.ShouldBe(inputs[i]);
                call.Returned.ShouldBe(outputs[i]);
                return call;
            }).Count().ShouldBe(outputs.Count);
        }
        
        [Fact]
        public void GivenStructMapWithNoRedirectAndNoArgs_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _via
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Record()
                .Map(call => new { Input = (Guid) call.Args[0], Returned = call.Returned?.Value ?? Guid.Empty });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
        }
        
        [Fact]
        public void GivenStructMapRedirect_WhenException_ShouldRecordAndMapException()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var builder = _via.To(x => x.EchoGeneric(Is<Guid>.Any));
            var echoes = builder
                .Redirect<(Guid input, __)>(call => throw new Exception($"{call.Args.input}"))
                .Record()
                .Map(call => new
                {
                    Input = call.Args.input,
                    call.Returned,
                    call.Returned?.Exception
                });

            // ACT
            var outputs = inputs.Select(x =>
            {
                try
                {
                    _proxy.EchoGeneric(x);
                    return null;
                }
                catch (Exception ex)
                {
                    return ex;
                }
            }).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Exception).ShouldBe(outputs);
            echoes.Verify(call =>
            {
                Should.Throw<DiverterException>(() => call.Returned!.Value);
            });
        }
        
        [Fact]
        public void GivenMapRedirect_WhenOutCall_ShouldRecordAndMap()
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
                .Record()
                .Map((_, args) => new { args.input, output = args.output.Value });

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
        public async Task GivenTaskMapRedirect_ShouldVerifyAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var calls = _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect<(string input, __)>(call => Task.FromResult(call.Args.input + " diverted"))
                .Record()
                .Map(async (call, args) => new
                {
                    Input = args.input,
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
            calls.Count.ShouldBe(inputs.Count);

            var count = 0;

            count = 0;
            (await calls.Verify(async call =>
            {
                (await call).Input.ShouldBe($"test{count}");
                (await call).Result.ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public async Task GivenMapTypedFuncRedirect_ShouldVerifyAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var calls = _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect<(string input, __)>(call => Task.FromResult(call.Args.input + " diverted"))
                .Record()
                .Map(call => new
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
            calls.Count.ShouldBe(inputs.Count);

            var count = 0;
            (await calls.Verify(async call =>
            {
                call.Input.ShouldBe($"test{count}");
                (await call.Result).ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public async Task GivenMapFuncRedirect_ShouldVerifyAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var calls = _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect(call => Task.FromResult($"{call.Args[0]} diverted"))
                .Record()
                .Map((call, args) => new
                {
                    Input = (string) args[0],
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
            calls.Count.ShouldBe(inputs.Count);

            var count = 0;
            (await calls.Verify(async call =>
            {
                call.Input.ShouldBe($"test{count}");
                (await call.Result).ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenMapTypedActionRedirect_ShouldVerifyAsync()
        {
            // ARRANGE
            var input = Guid.NewGuid().ToString();
            
            var calls = _via
                .To(x => x.SetName(Is<string>.Any))
                .Redirect<(string input, __)>((call, args) =>
                {
                    call.Next.SetName($"{args.input} diverted");
                })
                .Record();

            // ACT
            _proxy.SetName($"test {input}");

            // ASSERT
            _proxy.Name.ShouldBe($"test {input} diverted");
            calls.Count.ShouldBe(1);
            
            calls.Map(call => new
            {
                Input = call.Args.input,
                Result = call.Returned!.Value
            }).Verify(map =>
            {
                map.Input.ShouldBe($"test {input}");
                map.Result.ShouldBeNull();
            });
            
            calls.Map((call, args) => new
            {
                Input = args.input,
                Result = call.Returned!.Value
            }).Verify(map =>
            {
                map.Input.ShouldBe($"test {input}");
                map.Result.ShouldBeNull();
            });
        }
        
        [Fact]
        public void GivenMapActionRedirect_ShouldVerifyAsync()
        {
            // ARRANGE
            var input = Guid.NewGuid().ToString();
            
            var calls = _via
                .To(x => x.SetName(Is<string>.Any))
                .Redirect((call, args) =>
                {
                    call.Next.SetName($"{args[0]} diverted");
                })
                .Record();

            // ACT
            _proxy.SetName($"test {input}");

            // ASSERT
            _proxy.Name.ShouldBe($"test {input} diverted");
            calls.Count.ShouldBe(1);

            calls.Verify(call =>
            {
                call.Args[0].ShouldBe($"test {input}");
                call.Returned!.Value.ShouldBeNull();
            });
            
            calls.Verify((call, args) =>
            {
                args[0].ShouldBe($"test {input}");
                call.Returned!.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Map(call => new
            {
                Input = (string) call.Args[0],
                Result = call.Returned!.Value
            }).Verify(map =>
            {
                map.Input.ShouldBe($"test {input}");
                map.Result.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Map((call, args) => new
            {
                Input = (string) args[0],
                Result = call.Returned!.Value
            }).Verify(map =>
            {
                map.Input.ShouldBe($"test {input}");
                map.Result.ShouldBeNull();
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenCallConstraintMap_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid().ToString())
                .ToList();

            var echoes = _via
                .To(new CallConstraint<IFoo>(call => call.Method.Name == nameof(IFoo.Echo)))
                .Record()
                .Map((_, args) => new { Input = args[0] });

            // ACT
            var outputs = inputs.Select(x => _proxy.Echo(x)).ToList();

            // ASSERT
            outputs.ShouldBe(inputs.Select(x => $"{_proxy.Name}: {x}"));
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            
            var snapshot = echoes.Verify();
            snapshot.Count.ShouldBe(inputs.Count);
            for (var i = 0; i < snapshot.Count; i++)
            {
                snapshot[i].Input.ShouldBe(inputs[i]);
            }
        }
        
        [Fact]
        public async Task GivenClassRedirectBuilderWithNoArgs_ShouldMap()
        {
            // ARRANGE
            var calls = _via
                .To(x => x.EchoAsync(Is<string>.Any))
                .Record()
                .Map(async call => new
                {
                    Input = (string) call.Args[0],
                    Result = await call.Returned!.Value
                });

            // ACT
            var result = await _proxy.EchoAsync("test");
            
            // ASSERT
            (await calls.Verify(async call =>
            {
                result.ShouldBe("original: test");
                (await call).Input.ShouldBe("test");
                (await call).Result.ShouldBe(result);
            })).Count.ShouldBe(1);
        }
    }
}