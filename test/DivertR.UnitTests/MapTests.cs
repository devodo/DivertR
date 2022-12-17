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
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        private readonly IFoo _proxy;

        public MapTests()
        {
            _proxy = _redirect.Proxy(new Foo());
        }

        [Fact]
        public void GivenStructMapVia_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _redirect
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Via<(Guid input, __)>(_ => Guid.NewGuid())
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
        public void GivenStructMapWithNoVia_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _redirect
                .To(x => x.EchoGeneric(Is<Guid>.Any))
                .Record<(Guid input, __)>()
                .Map(call => new { Input = call.Args.input, Returned = call.Returned?.Value ?? Guid.Empty });

            // ACT
            var outputs = inputs.Select(x => _proxy.EchoGeneric(x)).ToList();

            // ASSERT
            echoes.Count.ShouldBe(inputs.Count);
            echoes.Select(x => x.Input).ShouldBe(inputs);
            echoes.Select(x => x.Returned).ShouldBe(outputs);
            
            var i = 0;
            echoes.Verify(call =>
            {
                call.Input.ShouldBe(inputs[i]);
                call.Returned.ShouldBe(outputs[i++]);
            }).Count.ShouldBe(outputs.Count);
        }
        
        [Fact]
        public void GivenStructMapWithNoViaAndNoArgs_ShouldRecordAndMapCalls()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var echoes = _redirect
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
        public void GivenStructMapVia_WhenException_ShouldRecordAndMapException()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(_ => Guid.NewGuid())
                .ToList();

            var builder = _redirect.To(x => x.EchoGeneric(Is<Guid>.Any));
            var echoes = builder
                .Via<(Guid input, __)>(call => throw new Exception($"{call.Args.input}"))
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
        public void GivenMapVia_WhenOutParameterCall_ShouldRecordAndMap()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();

            var numberRedirect = new Redirect<INumber>();
            var numberProxy = numberRedirect.Proxy(new Number(x => x * 10));

            var results = numberRedirect
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Via<(int input, Ref<int> output)>(call => call.Relay.Next.OutNumber(call.Args.input, out call.Args.output.Value))
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
        public async Task GivenTaskMapVia_ShouldVerifyAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var calls = _redirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Via<(string input, __)>(call => Task.FromResult(call.Args.input + " diverted"))
                .Record()
                .Map(async (call, args) => new
                {
                    Input = args.input,
                    Result = await call.Returned!.Value!
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
            (await calls.VerifyAsync(call =>
            {
                call.Input.ShouldBe($"test{count}");
                call.Result.ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);

            count = 0;
            (await calls.Verify(async call =>
            {
                (await call).Input.ShouldBe($"test{count}");
                (await call).Result.ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public async Task GivenMapTypedFuncVia_ShouldVerifyAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var calls = _redirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Via<(string input, __)>(call => Task.FromResult(call.Args.input + " diverted"))
                .Record()
                .Map(async call => new
                {
                    Input = call.Args.input,
                    Result = await call.Returned!.Value!,
                    AsyncResult = call.Returned!.Value
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
            (await calls.VerifyAsync(async call =>
            {
                call.Input.ShouldBe($"test{count}");
                call.Result.ShouldBe($"test{count} diverted");
                (await call.AsyncResult).ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);

            count = 0;
            (await calls.Verify(async call =>
            {
                (await call).Input.ShouldBe($"test{count}");
                (await call).Result.ShouldBe($"test{count} diverted");
                (await (await call).AsyncResult).ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public async Task GivenMapFuncVia_ShouldVerifyAsync()
        {
            // ARRANGE
            var inputs = Enumerable
                .Range(0, 20).Select(i => i)
                .ToList();
            
            var calls = _redirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Via(call => Task.FromResult($"{call.Args[0]} diverted"))
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
                (await call.Result!).ShouldBe($"test{count++} diverted");
            })).Count.ShouldBe(inputs.Count);
        }
        
        [Fact]
        public void GivenMapTypedActionVia_ShouldVerifyAsync()
        {
            // ARRANGE
            var input = Guid.NewGuid().ToString();
            
            var calls = _redirect
                .To(x => x.SetName(Is<string>.Any))
                .Via<(string input, __)>((call, args) =>
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
        public void GivenMapActionVia_ShouldVerifyAsync()
        {
            // ARRANGE
            var input = Guid.NewGuid().ToString();
            
            var calls = _redirect
                .To(x => x.SetName(Is<string>.Any))
                .Via((call, args) =>
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

            var echoes = _redirect
                .To(new MatchCallConstraint<IFoo>(call => call.Method.Name == nameof(IFoo.Echo)))
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
        public async Task GivenClassViaBuilderWithNoArgs_ShouldMap()
        {
            // ARRANGE
            var calls = _redirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Record()
                .Map(async call => new
                {
                    Input = (string) call.Args[0],
                    Result = await call.Returned!.Value!
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