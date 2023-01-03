using System;
using System.Linq;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RecordViaTests
    {
        private readonly IRedirect<IFoo> _fooRedirect = new Redirect<IFoo>();
        private readonly IFoo _proxy;
        private readonly IFoo _foo = new Foo();

        public RecordViaTests()
        {
            _proxy = _fooRedirect.Proxy(_foo);
        }

        [Fact]
        public void GivenRecordGetParamVia_WhenGet_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooRedirect
                .To(x => x.Name)
                .Record();

            // ACT
            var result = _proxy.Name;

            // ASSERT
            calls.Verify(call =>
            {
                result.ShouldBe(_foo.Name);
                call.Args.Count.ShouldBe(0);
                call.Return.ShouldBe(_foo.Name);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordFuncCallStream_WhenProxyNotCalled_ThenRecordsNoCalls()
        {
            // ARRANGE
            var calls = _fooRedirect
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
            var calls = _fooRedirect
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
            var calls = _fooRedirect
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
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenTypedRecordFuncCallStream_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooRedirect
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
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Args<(object input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>().Select(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBe(result);
                return call;
            }).Count().ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecordFuncCallStreamWithTaskParam_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooRedirect
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
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify(async call =>
            {
                (await (Task<string>) call.Args[0]).ShouldBe(input);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async call =>
            {
                (await call.Args.input).ShouldBe(input);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async call =>
            {
                (await call.Args.input).ShouldBe(input);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenTypedRecordFuncCallStreamWithTaskParam_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooRedirect
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
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify(async call =>
            {
                (await call.Args.input1).ShouldBe(input1);
                (await call.Args.input2).ShouldBe(input2);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, Task<int> i2)>(async call =>
            {
                (await call.Args.i1).ShouldBe(input1);
                (await call.Args.i2).ShouldBe(input2);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, Task<int> i2)>(async call =>
            {
                (await call.Args.i1).ShouldBe(input1);
                (await call.Args.i2).ShouldBe(input2);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, __)>(async call =>
            {
                (await call.Args.i1).ShouldBe(input1);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> i1, __)>(async call =>
            {
                (await call.Args.i1).ShouldBe(input1);
                call.Return.ShouldBe(result);
            })).Count.ShouldBe(1);
        }
        
        [Fact]
        public async Task GivenRecordActionCallStreamWithTaskParam_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooRedirect
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
                call.Return.ShouldBeNull();
            })).Count.ShouldBe(1);
            
            (await calls.Verify(async call =>
            {
                (await (Task<string>) call.Args[0]).ShouldBe(input);
                call.Return.ShouldBeNull();
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async call =>
            {
                (await call.Args.input).ShouldBe(input);
                call.Return.ShouldBeNull();
            })).Count.ShouldBe(1);
            
            (await calls.Verify<(Task<string> input, __)>(async call =>
            {
                (await call.Args.input).ShouldBe(input);
                call.Return.ShouldBeNull();
            })).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordFuncCallStream_WhenVerifyArgsWithInvalidType_ThenThrowsException()
        {
            // ARRANGE
            var calls = _fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Record();

            // ACT
            Action verify = () => calls.Verify<(int input, __)>();
            
            Action verifyCalls = () => calls.Verify<(int input, __)>(_ =>
            {
            });

            // ASSERT
            verify.ShouldThrow<DiverterValidationException>();
            verifyCalls.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenRecordActionCallStream_WhenVerifyArgsWithInvalidType_ThenThrowsException()
        {
            // ARRANGE
            var calls = _fooRedirect
                .To(x => x.SetName(Is<string>.Any))
                .Record();

            // ACT
            Action verify = () => calls.Verify<(int input, __)>();
            
            Action verifyCalls = () => calls.Verify<(int input, __)>(_ =>
            {
            });

            // ASSERT
            verify.ShouldThrow<DiverterValidationException>();
            verifyCalls.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenRecordActionCallStream_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooRedirect
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
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenTypedRecordActionCallStream_WhenProxyCalled_ThenRecordsCall()
        {
            // ARRANGE
            var calls = _fooRedirect
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
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(string input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Args<(string input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Args<(object input, __)>().Verify(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(1);
            
            calls.Verify<(object input, __)>().Select(call =>
            {
                call.Args.input.ShouldBe(input);
                call.Return.ShouldBeNull();
                return call;
            }).Count().ShouldBe(1);
        }

        [Fact]
        public void GivenRecordViaWithArgs_WhenCalled_ThenRecordsCallsWithArgs()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooRedirect
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
                call.Return.ShouldBe(_foo.Echo((string) call.Args[0]));
            }).Count.ShouldBe(inputs.Length);
            
            calls.Verify(call =>
            {
                call.Args.Count.ShouldBe(1);
                call.Return.ShouldBe(_foo.Echo((string) call.Args[0]));
            }).Count.ShouldBe(inputs.Length);
            
            calls.Args<(string input, __)>().Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                call.Return.ShouldBe(_foo.Echo(call.Args.input));
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenFuncCallStreamWithRelayRootVia_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Root.Echo(call.Args.input)} redirected")
                .Record();

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} redirected"));

            var count = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count]);
                call.Return.ShouldBe(results[count++]);
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count2]);
                call.Return.ShouldBe(results[count2++]);
            }).Count.ShouldBe(inputs.Length);

            calls.Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                call.Return.ShouldBe(results[i]);
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenFuncCallStreamWithRelayNextVia_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Next.Echo(call.Args.input)} redirected")
                .Record();

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} redirected"));

            var count = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count]);
                call.Return.ShouldBe(results[count++]);
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count2]);
                call.Return.ShouldBe(results[count2++]);
            }).Count.ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenActionCallStreamWithRelayRootVia_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooRedirect
                .To(x => x.SetName(Is<string>.Any))
                .Via<(string input, __)>(call => call.Root.SetName($"{call.Args.input} redirected"))
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
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count2++]);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenActionCallStreamWithRelayNextVia_WhenCalled_ThenRecordsCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();
            
            var calls = _fooRedirect
                .To(x => x.SetName(Is<string>.Any))
                .Via<(string input, __)>(call => call.Next.SetName($"{call.Args.input} redirected"))
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
                call.IsReturned.ShouldBeTrue();
                call.IsCompleted.ShouldBeTrue();
                call.Exception.ShouldBeNull();
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
            
            var count2 = 0;
            calls.Verify(call =>
            {
                call.Args.input.ShouldBe(inputs[count2++]);
                call.Return.ShouldBeNull();
            }).Count.ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenRecordBeforeRelayRootVia_WhenCalled_ThenDoesNotRecord()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var viaBuilder = _fooRedirect.To(x => x.Echo(Is<string>.Any));
            var calls = viaBuilder.Record();
            viaBuilder
                .Via<(string input, __)>(call => $"{call.Root.Echo(call.Args.input)} redirected");

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} redirected"));
            calls.Count.ShouldBe(0);
        }
        
        [Fact]
        public void GivenRecordBeforeRelayNextVia_WhenCalled_ThenRecordsCallAfterVia()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var viaBuilder = _fooRedirect.To(x => x.Echo(Is<string>.Any));
            var calls = viaBuilder.Record<(string input, __)>();
            viaBuilder
                .Via<(string input, __)>(call =>
                {
                    var modifiedInput = $"{call.Args.input} modified";
                    return $"{call.Next.Echo(modifiedInput)} redirected";
                });

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} modified redirected"));
            calls.Verify().Select((call, i) =>
            {
                call.Args.input.ShouldBe($"{inputs[i]} modified");
                call.Return.ShouldBe($"{_foo.Echo(inputs[i])} modified");
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenOrderFirstRecordBeforeRelayRootVia_WhenCalled_ThenRecordsCall()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var viaBuilder = _fooRedirect.To(x => x.Echo(Is<string>.Any));
            var recordedCalls = viaBuilder.Record<(string input, __)>(opt => opt.OrderFirst());
            viaBuilder
                .Via<(string input, __)>(call =>
                {
                    var modifiedInput = $"{call.Args.input} modified";
                    return $"{call.Next.Echo(modifiedInput)} redirected";
                });

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            results.ShouldBe(inputs.Select(input => $"{_foo.Echo(input)} modified redirected"));
            recordedCalls.Verify().Select((call, i) =>
            {
                call.Args.input.ShouldBe($"{inputs[i]}");
                call.Return.ShouldBe($"{results[i]}");
                return call;
            }).Count().ShouldBe(inputs.Length);
        }
        
        [Fact]
        public void GivenOrderLastViaAfterRecord_WhenCalled_ThenRecordsCall()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var viaBuilder = _fooRedirect.To(x => x.Echo(Is<string>.Any));
            var recordedCalls = viaBuilder.Record<(string input, __)>();
            viaBuilder
                .Via<(string input, __)>(call =>
                {
                    var modifiedInput = $"{call.Args.input} modified";
                    return $"{call.Next.Echo(modifiedInput)} redirected";
                }, opt => opt.OrderLast());

            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToList();

            // ASSERT
            recordedCalls.Select((call, i) =>
            {
                call.Args.input.ShouldBe(inputs[i]);
                call.Return.ShouldBe($"{results[i]}");
                return call;
            }).Count().ShouldBe(results.Count);
        }

        [Fact]
        public void GivenStrictModeEnabledAndRecordVia_WhenProxyCalled_ThenThrowsException()
        {
            // ARRANGE
            var recordedCalls = _fooRedirect
                .Strict()
                .To(x => x.Name)
                .Record();
            
            // ACT
            var testAction = () => _proxy.Name;
            
            // ASSERT
            var exception = testAction.ShouldThrow<StrictNotSatisfiedException>();
            recordedCalls
                .Verify(call =>
                {
                    call.Exception.ShouldBeSameAs(exception);
                    call.RawException.ShouldBeSameAs(exception);
                    call.IsCompleted.ShouldBeTrue();
                    call.IsReturned.ShouldBeFalse();
                })
                .Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenStrictModeEnabledAndRecordViaWithSatisfyStrictEnabled_WhenProxyCalled_ThenReturns()
        {
            // ARRANGE
            var recordedCalls = _fooRedirect
                .Strict()
                .To(x => x.Name)
                .Record(opt => opt.DisableSatisfyStrict(false));
            
            // ACT
            var result = _proxy.Name;
            
            // ASSERT
            result.ShouldBe(_foo.Name);
            recordedCalls
                .Verify(call => call.Return.ShouldBe(_foo.Name))
                .Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordCalls_WhenFilterCalled_ThenFiltersCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var calls = _fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => call.Args.input)
                .Record();
            
            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            calls
                .Filter(call => call.Args.input == results[5])
                .Verify(call => call.Args.input.ShouldBe(results[5])).Count.ShouldBe(1);
        }
        
        [Fact]
        public void GivenRecordCalls_WhenForeachCalled_ThenIteratesCalls()
        {
            // ARRANGE
            var inputs = Enumerable.Range(0, 10)
                .Select(_ => Guid.NewGuid().ToString())
                .ToArray();

            var calls = _fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => call.Args.input)
                .Record();
            
            // ACT
            var results = inputs.Select(input => _proxy.Echo(input)).ToArray();

            // ASSERT
            var count = 0;
            calls.Verify()
                .ForEach(call =>
                {
                    call.Return!.ShouldBe(results[count]);
                    call.Return!.ShouldBe(inputs[count++]);
                })
                .ForEach((call, i) =>
                {
                    call.Return!.ShouldBe(results[i]);
                    call.Return!.ShouldBe(inputs[i]);
                }).Count.ShouldBe(inputs.Length);
        }
    }
}