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
        private readonly ICallStream<IFoo> _calls;

        private readonly List<(int Arg, int Incr, int Result)[]> _inputs = new();

        public ViaActionParamTests()
        {
            _proxy = _via.Proxy(_original);
            _calls = _via.Record();
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
                .Redirect((int i1) =>
                {
                    _via.Relay.Next.GenericAction(i1);
                    var item1 = (int) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = item1 + _inputs[0][0].Incr;
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any))
                .Redirect((int i1, int i2) =>
                {
                    _via.Relay.Next.GenericAction(i1, i2);
                    var (item1, item2) = ((int, int)) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = (item1 + _inputs[1][0].Incr, item2 + _inputs[1][1].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect((int i1, int i2, int i3) =>
                {
                    _via.Relay.Next.GenericAction(i1, i2, i3);
                    var (item1, item2, item3) = ((int, int, int)) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = (item1 + _inputs[2][0].Incr, item2 + _inputs[2][1].Incr, item3 + _inputs[2][2].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect((int i1, int i2, int i3, int i4) =>
                {
                    _via.Relay.Next.GenericAction(i1, i2, i3, i4);
                    var (item1, item2, item3, item4) = ((int, int, int, int)) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = (item1 + _inputs[3][0].Incr, item2 + _inputs[3][1].Incr, item3 + _inputs[3][2].Incr, item4 + _inputs[3][3].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect((int i1, int i2, int i3, int i4, int i5) =>
                {
                    _via.Relay.Next.GenericAction(i1, i2, i3, i4, i5);
                    var (item1, item2, item3, item4, item5) = ((int, int, int, int, int)) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = (item1 + _inputs[4][0].Incr, item2 + _inputs[4][1].Incr, item3 + _inputs[4][2].Incr, item4 + _inputs[4][3].Incr,
                        item5 + _inputs[4][4].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect((int i1, int i2, int i3, int i4, int i5, int i6) =>
                {
                    _via.Relay.Next.GenericAction(i1, i2, i3, i4, i5, i6);
                    var (item1, item2, item3, item4, item5, item6) = ((int, int, int, int, int, int)) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = (item1 + _inputs[5][0].Incr, item2 + _inputs[5][1].Incr, item3 + _inputs[5][2].Incr, item4 + _inputs[5][3].Incr,
                        item5 + _inputs[5][4].Incr, item6 + _inputs[5][5].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect((int i1, int i2, int i3, int i4, int i5, int i6, int i7) =>
                {
                    _via.Relay.Next.GenericAction(i1, i2, i3, i4, i5, i6, i7);
                    var (item1, item2, item3, item4, item5, item6, item7) = ((int, int, int, int, int, int, int)) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = (item1 + _inputs[6][0].Incr, item2 + _inputs[6][1].Incr, item3 + _inputs[6][2].Incr, item4 + _inputs[6][3].Incr,
                        item5 + _inputs[6][4].Incr, item6 + _inputs[6][5].Incr, item7 + _inputs[6][6].Incr);
                });
            
            _via
                .To(x => x.GenericAction(Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any, Is<int>.Any))
                .Redirect((int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8) =>
                {
                    _via.Relay.Next.GenericAction(i1, i2, i3, i4, i5, i6, i7, i8);
                    var (item1, item2, item3, item4, item5, item6, item7, item8) = ((int, int, int, int, int, int, int, int)) _via.Relay.Next.LastAction;
                    _via.Relay.Next.LastAction = (item1 + _inputs[7][0].Incr, item2 + _inputs[7][1].Incr, item3 + _inputs[7][2].Incr, item4 + _inputs[7][3].Incr,
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
            _calls
                .To(x => x.GenericAction(_inputs[0][0].Arg))
                .ForEach<(int i1, __)>(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1) =>
                    {
                        i1.ShouldBe(_inputs[0][0].Arg);
                    }).ShouldBe(_inputs[0][0].Arg);
                    
                    call.Args((int i1) => i1).ShouldBe(_inputs[0][0].Arg);
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
            _calls
                .To(x => x.GenericAction(_inputs[1][0].Arg, _inputs[1][1].Arg))
                .ForEach(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1, int i2) =>
                    {
                        i1.ShouldBe(_inputs[1][0].Arg);
                        i2.ShouldBe(_inputs[1][1].Arg);
                    }).ShouldBe((_inputs[1][0].Arg, _inputs[1][1].Arg));
                    
                    call.Args((int i1, int i2) => (i1, i2)).ShouldBe((_inputs[1][0].Arg, _inputs[1][1].Arg));
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
            _calls
                .To(x => x.GenericAction(_inputs[2][0].Arg, _inputs[2][1].Arg, _inputs[2][2].Arg))
                .ForEach(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1, int i2, int i3) =>
                    {
                        i1.ShouldBe(_inputs[2][0].Arg);
                        i2.ShouldBe(_inputs[2][1].Arg);
                        i3.ShouldBe(_inputs[2][2].Arg);
                    }).ShouldBe((_inputs[2][0].Arg, _inputs[2][1].Arg, _inputs[2][2].Arg));
                    
                    call.Args((int i1, int i2, int i3) => (i1, i2, i3)).ShouldBe((_inputs[2][0].Arg, _inputs[2][1].Arg, _inputs[2][2].Arg));
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
            _calls
                .To(x => x.GenericAction(_inputs[3][0].Arg, _inputs[3][1].Arg, _inputs[3][2].Arg, _inputs[3][3].Arg))
                .ForEach(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1, int i2, int i3, int i4) =>
                    {
                        i1.ShouldBe(_inputs[3][0].Arg);
                        i2.ShouldBe(_inputs[3][1].Arg);
                        i3.ShouldBe(_inputs[3][2].Arg);
                        i4.ShouldBe(_inputs[3][3].Arg);
                    }).ShouldBe((_inputs[3][0].Arg, _inputs[3][1].Arg, _inputs[3][2].Arg, _inputs[3][3].Arg));
                    
                    call.Args((int i1, int i2, int i3, int i4) => (i1, i2, i3, i4)).ShouldBe((_inputs[3][0].Arg, _inputs[3][1].Arg, _inputs[3][2].Arg, _inputs[3][3].Arg));
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
            _calls
                .To(x => x.GenericAction(_inputs[4][0].Arg, _inputs[4][1].Arg, _inputs[4][2].Arg, _inputs[4][3].Arg,
                    _inputs[4][4].Arg))
                .ForEach(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1, int i2, int i3, int i4, int i5) =>
                    {
                        i1.ShouldBe(_inputs[4][0].Arg);
                        i2.ShouldBe(_inputs[4][1].Arg);
                        i3.ShouldBe(_inputs[4][2].Arg);
                        i4.ShouldBe(_inputs[4][3].Arg);
                        i5.ShouldBe(_inputs[4][4].Arg);
                    }).ShouldBe((_inputs[4][0].Arg, _inputs[4][1].Arg, _inputs[4][2].Arg, _inputs[4][3].Arg,
                        _inputs[4][4].Arg));
                    
                    call.Args((int i1, int i2, int i3, int i4, int i5) => (i1, i2, i3, i4, i5)).ShouldBe((_inputs[4][0].Arg, _inputs[4][1].Arg, _inputs[4][2].Arg, _inputs[4][3].Arg,
                        _inputs[4][4].Arg));
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
            _calls
                .To(x => x.GenericAction(_inputs[5][0].Arg, _inputs[5][1].Arg, _inputs[5][2].Arg, _inputs[5][3].Arg,
                    _inputs[5][4].Arg, _inputs[5][5].Arg))
                .ForEach(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1, int i2, int i3, int i4, int i5, int i6) =>
                    {
                        i1.ShouldBe(_inputs[5][0].Arg);
                        i2.ShouldBe(_inputs[5][1].Arg);
                        i3.ShouldBe(_inputs[5][2].Arg);
                        i4.ShouldBe(_inputs[5][3].Arg);
                        i5.ShouldBe(_inputs[5][4].Arg);
                        i6.ShouldBe(_inputs[5][5].Arg);
                    }).ShouldBe((_inputs[5][0].Arg, _inputs[5][1].Arg, _inputs[5][2].Arg, _inputs[5][3].Arg,
                        _inputs[5][4].Arg, _inputs[5][5].Arg));
                    
                    call.Args((int i1, int i2, int i3, int i4, int i5, int i6) => (i1, i2, i3, i4, i5, i6)).ShouldBe((_inputs[5][0].Arg, _inputs[5][1].Arg, _inputs[5][2].Arg, _inputs[5][3].Arg,
                        _inputs[5][4].Arg, _inputs[5][5].Arg));
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
            _calls
                .To(x => x.GenericAction(_inputs[6][0].Arg, _inputs[6][1].Arg, _inputs[6][2].Arg, _inputs[6][3].Arg,
                    _inputs[6][4].Arg, _inputs[6][5].Arg, _inputs[6][6].Arg))
                .ForEach(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1, int i2, int i3, int i4, int i5, int i6, int i7) =>
                    {
                        i1.ShouldBe(_inputs[6][0].Arg);
                        i2.ShouldBe(_inputs[6][1].Arg);
                        i3.ShouldBe(_inputs[6][2].Arg);
                        i4.ShouldBe(_inputs[6][3].Arg);
                        i5.ShouldBe(_inputs[6][4].Arg);
                        i6.ShouldBe(_inputs[6][5].Arg);
                        i7.ShouldBe(_inputs[6][6].Arg);
                    }).ShouldBe((_inputs[6][0].Arg, _inputs[6][1].Arg, _inputs[6][2].Arg, _inputs[6][3].Arg,
                        _inputs[6][4].Arg, _inputs[6][5].Arg, _inputs[6][6].Arg));
                    
                    call.Args((int i1, int i2, int i3, int i4, int i5, int i6, int i7) => (i1, i2, i3, i4, i5, i6, i7)).ShouldBe((_inputs[6][0].Arg, _inputs[6][1].Arg, _inputs[6][2].Arg, _inputs[6][3].Arg,
                        _inputs[6][4].Arg, _inputs[6][5].Arg, _inputs[6][6].Arg));
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
            _calls
                .To(x => x.GenericAction(_inputs[7][0].Arg, _inputs[7][1].Arg, _inputs[7][2].Arg, _inputs[7][3].Arg,
                    _inputs[7][4].Arg, _inputs[7][5].Arg, _inputs[7][6].Arg, _inputs[7][7].Arg))
                .ForEach(call =>
                {
                    call.Returned!.Value.ShouldBeNull();
                    call.Args((int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8) =>
                    {
                        i1.ShouldBe(_inputs[7][0].Arg);
                        i2.ShouldBe(_inputs[7][1].Arg);
                        i3.ShouldBe(_inputs[7][2].Arg);
                        i4.ShouldBe(_inputs[7][3].Arg);
                        i5.ShouldBe(_inputs[7][4].Arg);
                        i6.ShouldBe(_inputs[7][5].Arg);
                        i7.ShouldBe(_inputs[7][6].Arg);
                        i8.ShouldBe(_inputs[7][7].Arg);
                    }).ShouldBe((_inputs[7][0].Arg, _inputs[7][1].Arg, _inputs[7][2].Arg, _inputs[7][3].Arg,
                        _inputs[7][4].Arg, _inputs[7][5].Arg, _inputs[7][6].Arg, _inputs[7][7].Arg));
                    
                    call.Args((int i1, int i2, int i3, int i4, int i5, int i6, int i7, int i8) => (i1, i2, i3, i4, i5, i6, i7, i8)).ShouldBe((_inputs[7][0].Arg, _inputs[7][1].Arg, _inputs[7][2].Arg, _inputs[7][3].Arg,
                        _inputs[7][4].Arg, _inputs[7][5].Arg, _inputs[7][6].Arg, _inputs[7][7].Arg));
                }).Count.ShouldBe(1);
        }
    }
}