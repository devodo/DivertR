using System.Linq;
using DivertR.DynamicProxy;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ByRefDelegateTestsDynamicProxy : ByRefTests
    {
        private static readonly DiverterSettings DiverterSettings = new(proxyFactory: new DynamicProxyFactory());

        public ByRefDelegateTestsDynamicProxy()
            : base(new RedirectSet(DiverterSettings))
        {
        }
    }
    
    public class ByRefDelegateTests
    {
        private readonly IRedirect<INumber> _redirect;

        public ByRefDelegateTests() : this(new Redirect<INumber>())
        {
        }

        protected ByRefDelegateTests(IRedirect<INumber> redirect)
        {
            _redirect = redirect;
        }

        [Fact]
        public void GivenRefParameterVia_ShouldUpdateRefInput()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefNumber(ref IsRef<int>.Match(m => m == 3).Value))
                .Via<(Ref<int> i, __)>(call =>
                {
                    var refIn = call.Next.RefNumber(ref call.Args.i.Value);
                    call.Args.i.Value += 10;

                    return refIn;
                });
            
            var redirectProxy = _redirect.Proxy(new Number(i => i * 2));
            
            // ACT
            int input = 3;
            var result = redirectProxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(3);
            input.ShouldBe(16);
        }
        
        [Fact]
        public void GivenRefParameterRetarget_ShouldRedirect()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _redirect.Retarget(test);

            // ACT
            int input = 5;
            var proxy = _redirect.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(test.GetNumber(5));
        }
        
        [Fact]
        public void GivenOutParameterVia_ShouldUpdateOutParameter()
        {
            // ARRANGE
            _redirect
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Via<(int i, Ref<int> o)>(call =>
                {
                    call.Next.OutNumber(call.Args.i, out call.Args.o.Value);
                    call.Args.o.Value += 10;
                });
            
            var redirectProxy = _redirect.Proxy(new Number());

            // ACT
            redirectProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
        }

        [Fact]
        public void GivenOutParameterRetarget_ShouldUpdateOutParameter()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _redirect.Retarget(test);
            
            var redirectProxy = _redirect.Proxy(new Number());
            
            // ACT
            redirectProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(6);
        }

        [Fact]
        public void GivenOutParameterVia_ShouldRecord()
        {
            // ARRANGE
            var recordStream = _redirect
                .To(x => x.OutNumber(Is<int>.Any, out IsRef<int>.Any))
                .Via<(int i, Ref<int> o)>(call =>
                {
                    call.Next.OutNumber(call.Args.i, out call.Args.o.Value);
                    call.Args.o.Value += 10;
                })
                .Record();
            
            var redirectProxy = _redirect.Proxy(new Number());

            // ACT
            redirectProxy.OutNumber(3, out var output);

            // ASSERT
            output.ShouldBe(13);
            recordStream.Count.ShouldBe(1);
            recordStream.Verify(call =>
            {
                call.Args.i.ShouldBe(3);
                call.Args.o.Value.ShouldBe(13);
            }).Count.ShouldBe(1);
        }

        [Fact]
        public void GivenRefArrayParameterRetarget_ShouldRedirect()
        {
            // ARRANGE
            var test = new Number(x => x * 2);
            _redirect.Retarget(test);

            // ACT
            int[] inputOriginal = { 5, 8 };
            var input = inputOriginal;
            var proxy = _redirect.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x * 2));
        }
        
        [Fact]
        public void GivenRefArrayParameterDelegateVia_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.RefArrayNumber(ref IsRef<int[]>.Any))
                .Via<(Ref<int[]> input, __)>(call =>
                {
                    _redirect.Relay.Next.RefArrayNumber(ref call.Args.input.Value);

                    for (var i = 0; i < call.Args.input.Value.Length; i++)
                    {
                        call.Args.input.Value[i] += 10;
                    }
                });

            // ACT
            int[] inputOriginal = { 5, 8 };
            var input = inputOriginal;
            var proxy = _redirect.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input.ShouldBe(inputOriginal.Select(x => x + 10));
        }

        [Fact]
        public void GivenRefDelegateVia_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            int input = 5;
            redirect
                .To(x => x.RefNumber(ref input))
                .Via<(Ref<int> input, __)>(call =>
                {
                    var refIn = call.Args.input.Value;
                    call.Args.input.Value = 50;

                    return refIn + 1;
                });

            // ACT
            var i2 = 5;
            var proxy = redirect.Proxy(new Number());
            var result = proxy.RefNumber(ref i2);

            // ASSERT
            result.ShouldBe(6);
            i2.ShouldBe(50);
        }

        [Fact]
        public void GivenRefParameter_WhenParameterlessDelegateVia_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            redirect
                .To(x => x.RefNumber(ref IsRef<int>.Any))
                .Via(() => 10);

            // ACT
            var input = 5;
            var proxy = redirect.Proxy(new Number());
            var result = proxy.RefNumber(ref input);

            // ASSERT
            result.ShouldBe(10);
            input.ShouldBe(5);
        }
        
        [Fact]
        public void GivenGenericRefParameterVia_ShouldRedirect()
        {
            // ARRANGE
            var fooRedirect = new Redirect<IFoo>();

            fooRedirect
                .To(x => x.EchoGenericRef(ref IsRef<int>.Any))
                .Via<(Ref<int> input, __)>(call =>
                {
                    var input = call.Args.input.Value;
                    call.Args.input.Value = 10;

                    return input;
                });

            // ACT
            int input = 5;
            var proxy = fooRedirect.Proxy();
            var result = proxy.EchoGenericRef(ref input);

            // ASSERT
            result.ShouldBe(5);
            input.ShouldBe(10);
        }
    }
}