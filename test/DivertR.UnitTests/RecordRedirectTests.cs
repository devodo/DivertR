using System;
using System.Linq;
using System.Threading.Tasks;
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
        public void GivenRecordFuncCallStream_WhenProxyNotCalled_ThenRecordsNoCalls()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Record();

            // ACT

            // ASSERT
            calls.Count.ShouldBe(0);
            calls.Verify().Count.ShouldBe(0);
            calls.Verify<(string input, __)>().Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenRecordActionCallStream_WhenProxyNotCalled_ThenRecordsNoCalls()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.SetName(Is<string>.Any))
                .Record();

            // ACT

            // ASSERT
            calls.Count.ShouldBe(0);
            calls.Verify().Count.ShouldBe(0);
            calls.Verify<(string input, __)>().Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenRecordFuncCallStream_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Record();

            var input = Guid.NewGuid().ToString();

            // ACT
            var result = _proxy.Echo(input);

            // ASSERT
            result.ShouldBe(_foo.Echo(input));
            
            calls.Verify(call =>
            {
                call.Args[0].ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>((call, args) =>
            {
                args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenTypedRecordFuncCallStream_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Record<(string input, __)>();

            var input = Guid.NewGuid().ToString();

            // ACT
            var result = _proxy.Echo(input);

            // ASSERT
            result.ShouldBe(_foo.Echo(input));
            
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>((call, args) =>
            {
                args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>((call, args) =>
            {
                args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Args<(object input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>().Select(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
                return call;
            }).Count().ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecordFuncCallStreamWithTaskParam_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.EchoGeneric(Is<Task<string>>.Any))
                .Record();

            var input = Guid.NewGuid().ToString();

            // ACT
            var result = _proxy.EchoGeneric(Task.FromResult(input));

            // ASSERT
            (await result).ShouldBe(input);
            
            (await calls.Verify(async call =>
            {
                (await (Task<string>) call.Args[0]).ShouldBe(input);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify(async (call, args) =>
            {
                (await (Task<string>) args[0]).ShouldBe(input);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async call =>
            {
                (await call.Args.input).ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async (call, args) =>
            {
                (await args.input).ShouldBe(input);
                call.Returned?.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenTypedRecordFuncCallStreamWithTaskParam_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.EchoGeneric(Is<Task<string>>.Any, Is<Task<int>>.Any))
                .Record<(Task<string> input1, Task<int> input2)>();

            var input1 = Guid.NewGuid().ToString();
            var input2 = BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0);

            // ACT
            var result = _proxy.EchoGeneric(Task.FromResult(input1), Task.FromResult(input2));

            // ASSERT
            (await result.Item1).ShouldBe(input1);
            (await result.Item2).ShouldBe(input2);
            
            (await calls.Verify(async call =>
            {
                (await call.Args.input1).ShouldBe(input1);
                (await call.Args.input2).ShouldBe(input2);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify(async (call, args) =>
            {
                (await args.input1).ShouldBe(input1);
                (await args.input2).ShouldBe(input2);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, Task<int> i2)>(async call =>
            {
                (await call.Args.i1).ShouldBe(input1);
                (await call.Args.i2).ShouldBe(input2);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, Task<int> i2)>(async (call, args) =>
            {
                (await args.i1).ShouldBe(input1);
                (await args.i2).ShouldBe(input2);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, __)>(async call =>
            {
                (await call.Args.i1).ShouldBe(input1);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, __)>(async (call, args) =>
            {
                (await args.i1).ShouldBe(input1);
                call.Returned!.Value.ShouldBe(result);
            })).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecordActionCallStreamWithTaskParam_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.SetName(Is<Task<string>>.Any))
                .Record();

            var input = Guid.NewGuid().ToString();

            // ACT
            _proxy.SetName(Task.FromResult(input));

            // ASSERT
            _foo.Name.ShouldBe(input);
            
            (await calls.Verify(async call =>
            {
                (await (Task<string>) call.Args[0]).ShouldBe(input);
                call.Returned!.Value.ShouldBeNull();
            })).Count.ShouldBe(1);
            
            (await calls.Verify(async (call, args) =>
            {
                (await (Task<string>) args[0]).ShouldBe(input);
                call.Returned!.Value.ShouldBeNull();
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async call =>
            {
                (await call.Args.input).ShouldBe(input);
                call.Returned!.Value.ShouldBeNull();
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async (call, args) =>
            {
                (await args.input).ShouldBe(input);
                call.Returned!.Value.ShouldBeNull();
            })).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordFuncCallStream_WhenVerifyArgsWithInvalidType_ThenThrowsException()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Record();

            // ACT
            Action verify = () => calls.Verify<(int input, __)>();
            
            Action verifyCalls = () => calls.Verify<(int input, __)>(_ =>
            {
            });

            Action verifyCallsWithArgs = () => calls.Verify<(int input, __)>((_, _) =>
            {
            });
            
            // ASSERT
            verify.ShouldThrow<DiverterValidationException>();
            verifyCalls.ShouldThrow<DiverterValidationException>();
            verifyCallsWithArgs.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenRecordActionCallStream_WhenVerifyArgsWithInvalidType_ThenThrowsException()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.SetName(Is<string>.Any))
                .Record();

            // ACT
            Action verify = () => calls.Verify<(int input, __)>();
            
            Action verifyCalls = () => calls.Verify<(int input, __)>(_ =>
            {
            });

            Action verifyCallsWithArgs = () => calls.Verify<(int input, __)>((_, _) =>
            {
            });
            
            // ASSERT
            verify.ShouldThrow<DiverterValidationException>();
            verifyCalls.ShouldThrow<DiverterValidationException>();
            verifyCallsWithArgs.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenRecordActionCallStream_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.SetName(Is<string>.Any))
                .Record();

            var input = Guid.NewGuid().ToString();

            // ACT
            _proxy.SetName(input);

            // ASSERT
            _foo.Name.ShouldBe(input);
            
            calls.Verify(call =>
            {
                call.Args[0].ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>((call, args) =>
            {
                args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify<(string input, __)>((call, args) =>
            {
                args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenTypedRecordActionCallStream_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooVia
                .To(x => x.SetName(Is<string>.Any))
                .Record<(string input, __)>();

            var input = Guid.NewGuid().ToString();

            // ACT
            _proxy.SetName(input);

            // ASSERT
            _foo.Name.ShouldBe(input);
            
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>((call, args) =>
            {
                args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>((call, args) =>
            {
                args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Args<(object input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>().Select(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Returned?.Value.ShouldBeNull();
                return call;
            }).Count().ShouldBe(1);
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
            
            calls.Args<(string input, __)>().Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                call.Returned?.Value.ShouldBe(_foo.Echo(call.Args.input));
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenFuncCallStreamWithRelayRootRedirect_WhenCalled_ThenRecordsCalls()
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
        public void GivenFuncCallStreamWithRelayNextRedirect_WhenCalled_ThenRecordsCalls()
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
        public void GivenActionCallStreamWithRelayRootRedirect_WhenCalled_ThenRecordsCalls()
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
        public void GivenActionCallStreamWithRelayNextRedirect_WhenCalled_ThenRecordsCalls()
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
            var results = inputs.Select(input => _proxy.Echo(input)).ToList();

            // ASSERT
            recordedCalls.Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                call.Returned!.Value.ShouldBe($"{results[i]}");
                return call;
            }).Count().ShouldBe(results.Count);
        }

        [Fact]
        public void GivenStrictModeEnabledAndRecordRedirect_WhenProxyCalled_ThenThrowsException()
        {
            // ARRANGE
            var recordedCalls = _fooVia
                .Strict()
                .To(x => x.Name)
                .Record();
            
            // ACT
            var testAction = () => _proxy.Name;
            
            // ASSERT
            var exception = testAction.ShouldThrow<StrictNotSatisfiedException>();
            recordedCalls
                .Verify(call => call.Returned!.Exception.ShouldBeSameAs(exception))
                .Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenStrictModeEnabledAndRecordRedirectWithSatisfyStrictEnabled_WhenProxyCalled_ThenReturns()
        {
            // ARRANGE
            var recordedCalls = _fooVia
                .Strict()
                .To(x => x.Name)
                .Record(opt => opt.DisableSatisfyStrict(false));
            
            // ACT
            var result = _proxy.Name;
            
            // ASSERT
            result.ShouldBe(_foo.Name);
            recordedCalls
                .Verify(call => call.Returned!.Value.ShouldBe(_foo.Name))
                .Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordCalls_WhenWhereCalled_ThenFiltersCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var calls = _fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>(call => call.Args.input)
                .Record();
            
            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            calls
                .Filter(call => call.Args.input == results[5])
                .Verify(call => call.Args.input.ShouldBe(results[5])).Count.ShouldBe(1);
        }
    }
}