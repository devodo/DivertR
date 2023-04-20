using System.Collections.Generic;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DiverterTests
    {
        private readonly IDiverter _diverter = new Diverter();

        public DiverterTests()
        {
            _diverter.Register<IFoo>();
        }
        
        [Fact]
        public void GivenRegisteredType_WhenSameGenericTypeRegisteredWithDifferentName_ShouldRegister()
        {
            // ARRANGE

            // ACT
            _diverter.Register<IFoo>("test");
            
            // ASSERT
            _diverter.Redirect<IFoo>("test").ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRegisteredType_WhenSameTypeRegisteredWithDifferentName_ShouldRegister()
        {
            // ARRANGE

            // ACT
            _diverter.Register(typeof(IFoo), "test");
            
            // ASSERT
            _diverter.Redirect<IFoo>("test").ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRegisteredType_WhenSameTypeRegistered_ShouldThrowDiverterException()
        {
            // ARRANGE
            _diverter.Register<IFoo>("test");

            // ACT
            var testAction = () => _diverter.Register<IFoo>("test");
            
            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void GivenDiverter_WhenParamsOfTypesRegistered_ShouldRegister()
        {
            // ARRANGE

            // ACT
            _diverter.Register(typeof(IList<int>), typeof(IList<string>), typeof(IList<object>));
            
            // ASSERT
            _diverter.Redirect<IList<int>>().ShouldNotBeNull();
            _diverter.Redirect<IList<string>>().ShouldNotBeNull();
            _diverter.Redirect<IList<object>>().ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenDiverter_WhenNamedParamsOfTypesRegistered_ShouldRegister()
        {
            // ARRANGE

            // ACT
            _diverter.Register("test", typeof(IList<int>), typeof(IList<string>), typeof(IList<object>));
            
            // ASSERT
            _diverter.Redirect<IList<int>>("test").ShouldNotBeNull();
            _diverter.Redirect<IList<string>>("test").ShouldNotBeNull();
            _diverter.Redirect<IList<object>>("test").ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRetargetWithRelay_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var redirect = _diverter.Redirect<IFoo>().Proxy(original);

            // ACT
            _diverter.Redirect<IFoo>().Retarget(new FooAlt(() => $"{_diverter.Redirect<IFoo>().Relay.Next.Name} diverted"));
            
            // ASSERT
            redirect.Name.ShouldBe(original.Name + " diverted");
        }

        [Fact]
        public void GivenVias_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = _diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"));
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"));

            // ACT
            _diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenPersistentVias_WhenResetAll_ShouldNotReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = _diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"), opt => opt.Persist());
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"), opt => opt.Persist());

            // ACT
            _diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe("hello foo me again");
        }

        [Fact]
        public void GivenVias_WhenResetGroup_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = _diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"));
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"));

            // ACT
            _diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenPersistentVias_WhenResetGroup_ShouldNotReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = _diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"), opt => opt.Persist());
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"), opt => opt.Persist());

            // ACT
            _diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe("hello foo me again");
        }

        [Fact]
        public void GivenRegisteredRedirect_ShouldSetStrict()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = _diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);

            // ACT
            _diverter.Strict();
            
            // ASSERT
            var testAction = () => subject.Name;
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
        
        [Fact]
        public void GivenStrict_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = _diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            _diverter.Strict();

            // ACT
            _diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenStrict_WhenResetGroup_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = _diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            _diverter.Strict();

            // ACT
            _diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRegisterByType_ShouldRegisterRedirect()
        {
            // ARRANGE
            var diverter = new Diverter();
            diverter.Register(typeof(IFoo));

            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRegisterByTypes_ShouldRegisterRedirect()
        {
            // ARRANGE
            var diverter = new Diverter();
            diverter.Register(new[] { typeof(IFoo), typeof(IList<int>) });

            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy();
            var list = diverter.Redirect<IList<int>>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
            list.ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenDiverter_WhenGetUnregisteredRedirect_ThrowsDiverterException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _diverter.Redirect<INumber>();
            
            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void GivenDiverterWithNonRegisteredViaRedirect_WhenGetUnregisteredRedirect_ThenReturnsRedirect()
        {
            // ARRANGE
            _diverter
                .Redirect<IFoo>()
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .ViaRedirect();

            // ACT
            var numberRedirect = _diverter.Redirect<INumber>();
            
            // ASSERT
            numberRedirect.ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenRegisteredTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = new Diverter().Register<IFoo>(inner => inner.ThenRegister<IBar>());
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());

            diverter
                .Redirect<IBar>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar redirected");
        }
        
        [Fact]
        public async Task GivenNestedRegistration_WhenRegisteredTaskTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = new Diverter().Register<IFoo>(inner => inner.ThenRegister<IBar>());
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());

            diverter
                .Redirect<IBar>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            // ACT
            var bar = await fooProxy.EchoGeneric(Task.FromResult<IBar>(new Bar("bar")));

            // ASSERT
            bar.Name.ShouldBe("bar redirected");
        }
        
        [Fact]
        public async Task GivenNestedRegistration_WhenRegisteredValueTaskTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = new Diverter().Register<IFoo>(inner => inner.ThenRegister<IBar>());
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());

            diverter
                .Redirect<IBar>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            // ACT
            var bar = await fooProxy.EchoGeneric(new ValueTask<IBar>(new Bar("bar")));

            // ASSERT
            bar.Name.ShouldBe("bar redirected");
        }
        
        [Fact]
        public void GivenNestedNestedRegistration_WhenNestedRegisteredTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = new Diverter()
                .Register<IFoo>(x1 => x1
                    .ThenRegister<IFoo>(x2 => x2
                        .ThenRegister<IBar>()));
            
            var foo = new Foo("inner");
            var fooProxy = diverter.Redirect<IFoo>().Proxy(foo);

            diverter
                .Redirect<IBar>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            // ACT
            var innerFoo = fooProxy.EchoGeneric<IFoo>(foo);
            var bar = innerFoo.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar redirected");
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenSameProxyInstanceReturned_ShouldCache()
        {
            // ARRANGE
            var diverter = new Diverter()
                .Register<IFoo>(x => x
                    .ThenRegister<IFoo>());

            var foo = new Foo("inner");
            var fooProxy = diverter.Redirect<IFoo>().Proxy(foo);
            
            // ACT
            var innerFoo = fooProxy.EchoGeneric<IFoo>(foo);

            // ASSERT
            innerFoo.ShouldBeSameAs(fooProxy);
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenRedirectReset_ShouldPersist()
        {
            // ARRANGE
            var diverter = new Diverter().Register<IFoo>(x => x.ThenRegister<IBar>());

            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            diverter.ResetAll();
            
            diverter
                .Redirect<IBar>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");

            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar redirected");
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenRegisterExistingNestedType_ShouldThrowDiverterException()
        {
            // ARRANGE
            var diverter = new Diverter();
            
            // ACT
            var testAction = () =>
            {
                diverter.Register<IFoo>(x => x
                    .ThenRegister<IBar>()
                    .ThenRegister<IFoo>(y => y.ThenRegister<IBar>()));
            };

            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenStrictMode_ShouldDisableSatisfyStrict()
        {
            // ARRANGE
            var diverter = new Diverter().Register<IFoo>(x => x.ThenRegister<IBar>());

            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            diverter.Redirect<IFoo>().Strict();
            
            // ACT
            var testAction = () => fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
        
        [Fact]
        public void GivenNestedDecorator_WhenRegisteredTypeReturned_ShouldDecorate()
        {
            // ARRANGE
            var diverter = new Diverter().Register<IFoo>(inner =>
                inner.ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")));
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            
            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar decorated");
        }
        
        [Fact]
        public void GivenNestedStructDecorator_WhenRegisteredTypeReturned_ShouldDecorate()
        {
            // ARRANGE
            var diverter = new Diverter().Register<IFoo>(inner =>
                inner.ThenDecorate<int>(n => n + 1));
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            
            // ACT
            var result = fooProxy.EchoGeneric(10);

            // ASSERT
            result.ShouldBe(11);
        }
        
        [Fact]
        public void GivenNestedDecorator_WhenSameProxyInstanceReturned_ShouldCache()
        {
            // ARRANGE
            var diverter = new Diverter()
                .Register<IFoo>(x => x
                    .ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")));

            var bar = new Bar("bar");
            
            // ACT
            var bar1 = diverter.Redirect<IFoo>().Proxy(new Foo()).EchoGeneric<IBar>(bar);
            var bar2 = diverter.Redirect<IFoo>().Proxy(new Foo()).EchoGeneric<IBar>(bar);

            // ASSERT
            bar1.Name.ShouldBe("bar decorated");
            bar1.ShouldBeSameAs(bar2);
        }
        
        [Fact]
        public void GivenNestedDecorator_WhenRedirectReset_ShouldPersist()
        {
            // ARRANGE
            var diverter = new Diverter()
                .Register<IFoo>(x => x
                    .ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")));

            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            diverter.ResetAll();

            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar decorated");
        }
        
        [Fact]
        public void GivenNestedDecorator_WhenStrictMode_ShouldDisableSatisfyStrict()
        {
            // ARRANGE
            var diverter = new Diverter()
                .Register<IFoo>(x => x
                    .ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")));

            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            diverter.Redirect<IFoo>().Strict();
            
            // ACT
            var testAction = () => fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
    }
}
