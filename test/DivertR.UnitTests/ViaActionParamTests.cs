using System;
using System.Collections.Generic;
using System.Linq;
using DivertR.Record;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaActionParamTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IFoo _original = new Foo();
        private readonly IFoo _proxy;
        private readonly IRecordStream<IFoo> _recordStream;

        private readonly List<(int Arg, int Incr, int Result)[]> _inputs = new();

        public ViaActionParamTests()
        {
            _proxy = _via.Proxy(_original);
            _recordStream = _via.Record(options => options.OrderWeight(int.MaxValue));
            InitInputs();
        }

        private void InitInputs()
        {
            const int MaxParams = 8;
            var random = new Random();

            for (var i = 1; i <= MaxParams; i++)
            {
                _inputs.Add(Enumerable.Range(0, i).Select(_ =>
                {
                    var arg = random.Next();
                    var redirectArg = random.Next();
                    var result = arg + redirectArg;
                    
                    return (arg, redirectArg, result);
                }).ToArray());
            }
        }

        private void SetupRedirects()
        {
            _via
                .To(x => x.GenericAction(Is<int>.Any))
                .Redirect<(int i1, __)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1);
                    var item1 = (int) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = item1 + _inputs[0][0].Incr;
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any))
                .Redirect<(int i1, int i2)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1, call.Args.i2);
                    var (item1, item2) = ((int, int)) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = (item1 + _inputs[1][0].Incr, item2 + _inputs[1][1].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect<(int i1, int i2, int i3)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1, call.Args.i2, call.Args.i3);
                    var (item1, item2, item3) = ((int, int, int)) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = (item1 + _inputs[2][0].Incr, item2 + _inputs[2][1].Incr, item3 + _inputs[2][2].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect<(int i1, int i2, int i3, int i4)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1, call.Args.i2, call.Args.i3, call.Args.i4);
                    var (item1, item2, item3, item4) = ((int, int, int, int)) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = (item1 + _inputs[3][0].Incr, item2 + _inputs[3][1].Incr, item3 + _inputs[3][2].Incr, item4 + _inputs[3][3].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect<(int i1, int i2, int i3, int i4, int i5)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1, call.Args.i2, call.Args.i3, call.Args.i4, call.Args.i5);
                    var (item1, item2, item3, item4, item5) = ((int, int, int, int, int)) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = (item1 + _inputs[4][0].Incr, item2 + _inputs[4][1].Incr, item3 + _inputs[4][2].Incr, item4 + _inputs[4][3].Incr,
                        item5 + _inputs[4][4].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect<(int i1, int i2, int i3, int i4, int i5, int i6)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1, call.Args.i2, call.Args.i3, call.Args.i4, call.Args.i5, call.Args.i6);
                    var (item1, item2, item3, item4, item5, item6) = ((int, int, int, int, int, int)) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = (item1 + _inputs[5][0].Incr, item2 + _inputs[5][1].Incr, item3 + _inputs[5][2].Incr, item4 + _inputs[5][3].Incr,
                        item5 + _inputs[5][4].Incr, item6 + _inputs[5][5].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect<(int i1, int i2, int i3, int i4, int i5, int i6, int i7)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1, call.Args.i2, call.Args.i3, call.Args.i4, call.Args.i5, call.Args.i6, call.Args.i7);
                    var (item1, item2, item3, item4, item5, item6, item7) = ((int, int, int, int, int, int, int)) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = (item1 + _inputs[6][0].Incr, item2 + _inputs[6][1].Incr, item3 + _inputs[6][2].Incr, item4 + _inputs[6][3].Incr,
                        item5 + _inputs[6][4].Incr, item6 + _inputs[6][5].Incr, item7 + _inputs[6][6].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect<(int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8)>(call =>
                {
                    call.Relay.Next.GenericAction(call.Args.i1, call.Args.i2, call.Args.i3, call.Args.i4, call.Args.i5, call.Args.i6, call.Args.i7, call.Args.i8);
                    var (item1, item2, item3, item4, item5, item6, item7, item8) = ((int, int, int, int, int, int, int, int)) call.Relay.Next.LastAction;
                    call.Relay.Next.LastAction = (item1 + _inputs[7][0].Incr, item2 + _inputs[7][1].Incr, item3 + _inputs[7][2].Incr, item4 + _inputs[7][3].Incr,
                        item5 + _inputs[7][4].Incr, item6 + _inputs[7][5].Incr, item7 + _inputs[7][6].Incr, item8 + _inputs[7][7].Incr);
                });
        }
        
        [Fact]
        public void Given1Param_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[0][0].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe(_inputs[0][0].Result);
            _recordStream
                .To(x => x.GenericAction(_inputs[0][0].Arg))
                .Args<(int i1, __)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[0][0].Arg);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void Given2Params_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[1][0].Arg, _inputs[1][1].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe((_inputs[1][0].Result, _inputs[1][1].Result));
            _recordStream
                .To(x => x.GenericAction(_inputs[1][0].Arg, _inputs[1][1].Arg))
                .Args<(int i1, int i2)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[1][0].Arg);
                    call.Args.i2.ShouldBe(_inputs[1][1].Arg);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void Given3Params_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[2][0].Arg, _inputs[2][1].Arg, _inputs[2][2].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe((_inputs[2][0].Result, _inputs[2][1].Result, _inputs[2][2].Result));
            _recordStream
                .To(x => x.GenericAction(_inputs[2][0].Arg, _inputs[2][1].Arg, _inputs[2][2].Arg))
                .Args<(int i1, int i2, int i3)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[2][0].Arg);
                    call.Args.i2.ShouldBe(_inputs[2][1].Arg);
                    call.Args.i3.ShouldBe(_inputs[2][2].Arg);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void Given4Params_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[3][0].Arg, _inputs[3][1].Arg, _inputs[3][2].Arg, _inputs[3][3].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe((_inputs[3][0].Result, _inputs[3][1].Result, _inputs[3][2].Result, _inputs[3][3].Result));
            _recordStream
                .To(x => x.GenericAction(_inputs[3][0].Arg, _inputs[3][1].Arg, _inputs[3][2].Arg, _inputs[3][3].Arg))
                .Args<(int i1, int i2, int i3, int i4)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[3][0].Arg);
                    call.Args.i2.ShouldBe(_inputs[3][1].Arg);
                    call.Args.i3.ShouldBe(_inputs[3][2].Arg);
                    call.Args.i4.ShouldBe(_inputs[3][3].Arg);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void Given5Params_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[4][0].Arg, _inputs[4][1].Arg, _inputs[4][2].Arg, _inputs[4][3].Arg,
                _inputs[4][4].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe((_inputs[4][0].Result, _inputs[4][1].Result, _inputs[4][2].Result, _inputs[4][3].Result,
                _inputs[4][4].Result));
            _recordStream
                .To(x => x.GenericAction(_inputs[4][0].Arg, _inputs[4][1].Arg, _inputs[4][2].Arg, _inputs[4][3].Arg,
                    _inputs[4][4].Arg))
                .Args<(int i1, int i2, int i3, int i4, int i5)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[4][0].Arg);
                    call.Args.i2.ShouldBe(_inputs[4][1].Arg);
                    call.Args.i3.ShouldBe(_inputs[4][2].Arg);
                    call.Args.i4.ShouldBe(_inputs[4][3].Arg);
                    call.Args.i5.ShouldBe(_inputs[4][4].Arg);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void Given6Params_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[5][0].Arg, _inputs[5][1].Arg, _inputs[5][2].Arg, _inputs[5][3].Arg,
                _inputs[5][4].Arg, _inputs[5][5].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe((_inputs[5][0].Result, _inputs[5][1].Result, _inputs[5][2].Result, _inputs[5][3].Result,
                _inputs[5][4].Result, _inputs[5][5].Result));
            _recordStream
                .To(x => x.GenericAction(_inputs[5][0].Arg, _inputs[5][1].Arg, _inputs[5][2].Arg, _inputs[5][3].Arg,
                    _inputs[5][4].Arg, _inputs[5][5].Arg))
                .Args<(int i1, int i2, int i3, int i4, int i5, int i6)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[5][0].Arg);
                    call.Args.i2.ShouldBe(_inputs[5][1].Arg);
                    call.Args.i3.ShouldBe(_inputs[5][2].Arg);
                    call.Args.i4.ShouldBe(_inputs[5][3].Arg);
                    call.Args.i5.ShouldBe(_inputs[5][4].Arg);
                    call.Args.i6.ShouldBe(_inputs[5][5].Arg);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void Given7Params_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[6][0].Arg, _inputs[6][1].Arg, _inputs[6][2].Arg, _inputs[6][3].Arg,
                _inputs[6][4].Arg, _inputs[6][5].Arg, _inputs[6][6].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe((_inputs[6][0].Result, _inputs[6][1].Result, _inputs[6][2].Result, _inputs[6][3].Result,
                _inputs[6][4].Result, _inputs[6][5].Result, _inputs[6][6].Result));
            _recordStream
                .To(x => x.GenericAction(_inputs[6][0].Arg, _inputs[6][1].Arg, _inputs[6][2].Arg, _inputs[6][3].Arg,
                    _inputs[6][4].Arg, _inputs[6][5].Arg, _inputs[6][6].Arg))
                .Args<(int i1, int i2, int i3, int i4, int i5, int i6, int i7)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[6][0].Arg);
                    call.Args.i2.ShouldBe(_inputs[6][1].Arg);
                    call.Args.i3.ShouldBe(_inputs[6][2].Arg);
                    call.Args.i4.ShouldBe(_inputs[6][3].Arg);
                    call.Args.i5.ShouldBe(_inputs[6][4].Arg);
                    call.Args.i6.ShouldBe(_inputs[6][5].Arg);
                    call.Args.i7.ShouldBe(_inputs[6][6].Arg);
                }).Count.ShouldBe(1);
        }
        
        [Fact]
        public void Given8Params_ShouldRedirect()
        {
            // ARRANGE
            SetupRedirects();

            // ACT
            _proxy.GenericAction(_inputs[7][0].Arg, _inputs[7][1].Arg, _inputs[7][2].Arg, _inputs[7][3].Arg,
                _inputs[7][4].Arg, _inputs[7][5].Arg, _inputs[7][6].Arg, _inputs[7][7].Arg);
            var result = _original.LastAction;

            // ASSERT
            result.ShouldBe((_inputs[7][0].Result, _inputs[7][1].Result, _inputs[7][2].Result, _inputs[7][3].Result,
                _inputs[7][4].Result, _inputs[7][5].Result, _inputs[7][6].Result, _inputs[7][7].Result));
            _recordStream
                .To(x => x.GenericAction(_inputs[7][0].Arg, _inputs[7][1].Arg, _inputs[7][2].Arg, _inputs[7][3].Arg,
                    _inputs[7][4].Arg, _inputs[7][5].Arg, _inputs[7][6].Arg, _inputs[7][7].Arg))
                .Args<(int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8)>()
                .Verify(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args.i1.ShouldBe(_inputs[7][0].Arg);
                    call.Args.i2.ShouldBe(_inputs[7][1].Arg);
                    call.Args.i3.ShouldBe(_inputs[7][2].Arg);
                    call.Args.i4.ShouldBe(_inputs[7][3].Arg);
                    call.Args.i5.ShouldBe(_inputs[7][4].Arg);
                    call.Args.i6.ShouldBe(_inputs[7][5].Arg);
                    call.Args.i7.ShouldBe(_inputs[7][6].Arg);
                    call.Args.i8.ShouldBe(_inputs[7][7].Arg);
                }).Count.ShouldBe(1);
        }
    }
}