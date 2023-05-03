using System.Collections.Generic;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DiverterTests
    {
        private readonly IDiverterBuilder _diverterBuilder = new DiverterBuilder();

        [Fact]
        public void GivenRegisteredType_WhenSameGenericTypeRegisteredWithDifferentName_ShouldRegister()
        {
            // ARRANGE
            _diverterBuilder.Register<IFoo>();

            // ACT
            var diverter = _diverterBuilder.Register<IFoo>("test").Create();
            
            // ASSERT
            diverter.Redirect<IFoo>("test").ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRegisteredType_WhenSameTypeRegisteredWithDifferentName_ShouldRegister()
        {
            // ARRANGE
            _diverterBuilder.Register<IFoo>();

            // ACT
            var diverter = _diverterBuilder.Register(typeof(IFoo), "test").Create();
            
            // ASSERT
            diverter.Redirect<IFoo>("test").ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRegisteredType_WhenSameTypeRegistered_ShouldThrowDiverterException()
        {
            // ARRANGE
            _diverterBuilder.Register<IFoo>("test");

            // ACT
            var testAction = () => _diverterBuilder.Register<IFoo>("test");
            
            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void GivenDiverter_WhenParamsOfTypesRegistered_ShouldRegister()
        {
            // ARRANGE

            // ACT
            var diverter = _diverterBuilder.Register(typeof(IList<int>), typeof(IList<string>), typeof(IList<object>)).Create();
            
            // ASSERT
            diverter.Redirect<IList<int>>().ShouldNotBeNull();
            diverter.Redirect<IList<string>>().ShouldNotBeNull();
            diverter.Redirect<IList<object>>().ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenDiverter_WhenNamedParamsOfTypesRegistered_ShouldRegister()
        {
            // ARRANGE

            // ACT
            var diverter = _diverterBuilder.Register("test", typeof(IList<int>), typeof(IList<string>), typeof(IList<object>)).Create();
            
            // ASSERT
            diverter.Redirect<IList<int>>("test").ShouldNotBeNull();
            diverter.Redirect<IList<string>>("test").ShouldNotBeNull();
            diverter.Redirect<IList<object>>("test").ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRetargetWithRelay_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>().Proxy(original);

            // ACT
            diverter.Redirect<IFoo>().Retarget(new FooAlt(() => $"{diverter.Redirect<IFoo>().Relay.Next.Name} diverted"));
            
            // ASSERT
            redirect.Name.ShouldBe(original.Name + " diverted");
        }

        [Fact]
        public void GivenVias_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"));
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"));

            // ACT
            diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenPersistentVias_WhenResetAll_ShouldNotReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"), opt => opt.Persist());
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"), opt => opt.Persist());

            // ACT
            diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe("hello foo me again");
        }

        [Fact]
        public void GivenVias_WhenResetGroup_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"));
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"));

            // ACT
            diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenPersistentVias_WhenResetGroup_ShouldNotReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} me"), opt => opt.Persist());
            redirect.Retarget(new FooAlt(() => $"{redirect.Relay.CallNext()} again"), opt => opt.Persist());

            // ACT
            diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe("hello foo me again");
        }

        [Fact]
        public void GivenRegisteredRedirect_ShouldSetStrict()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);

            // ACT
            diverter.Strict();
            
            // ASSERT
            var testAction = () => subject.Name;
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
        
        [Fact]
        public void GivenStrict_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            diverter.Strict();

            // ACT
            diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenStrict_WhenResetGroup_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            var redirect = diverter.Redirect<IFoo>();
            var subject = redirect.Proxy(original);
            diverter.Strict();

            // ACT
            diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRegisterByType_ShouldRegisterRedirect()
        {
            // ARRANGE
            var diverter = _diverterBuilder.Register(typeof(IFoo)).Create();

            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRegisterByTypes_ShouldRegisterRedirect()
        {
            // ARRANGE
            var diverter = _diverterBuilder.Register(new[] { typeof(IFoo), typeof(IList<int>) }).Create();

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
            var diverter = _diverterBuilder.Create();

            // ACT
            var testAction = () => diverter.Redirect<INumber>();
            
            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void GivenDiverterWithNonRegisteredViaRedirect_WhenGetUnregisteredRedirect_ThenReturnsRedirect()
        {
            // ARRANGE
            var diverter = _diverterBuilder.Register<IFoo>().Create();
            
            diverter
                .Redirect<IFoo>()
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .ViaRedirect();

            // ACT
            var numberRedirect = diverter.Redirect<INumber>();
            
            // ASSERT
            numberRedirect.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddRedirect_ShouldAdd()
        {
            // ARRANGE
            var diverter = _diverterBuilder.AddRedirect<IFoo>().Create();

            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddNamedRedirect_ShouldAdd()
        {
            // ARRANGE
            var diverter = _diverterBuilder.AddRedirect<IFoo>("test").Create();

            // ACT
            var foo = diverter.Redirect<IFoo>("test").Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddRedirectByType_ShouldAdd()
        {
            // ARRANGE
            var diverter = _diverterBuilder.AddRedirect(typeof(IFoo)).Create();

            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddNamedRedirectByType_ShouldAdd()
        {
            // ARRANGE
            var diverter = _diverterBuilder.AddRedirect(typeof(IFoo), "test").Create();

            // ACT
            var foo = diverter.Redirect<IFoo>("test").Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenRegisteredTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = _diverterBuilder.Register<IFoo>(inner => inner.ThenRegister<IBar>()).Create();
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
            var diverter = _diverterBuilder.Register<IFoo>(inner => inner.ThenRegister<IBar>()).Create();
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
            var diverter = _diverterBuilder.Register<IFoo>(inner => inner.ThenRegister<IBar>()).Create();
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
            var diverter = _diverterBuilder
                .Register<IFoo>(x1 => x1
                    .ThenRegister<IFoo>(x2 => x2
                        .ThenRegister<IBar>()))
                .Create();
            
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
            var diverter = _diverterBuilder
                .Register<IFoo>(x => x
                    .ThenRegister<IFoo>())
                .Create();

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
            var diverter = _diverterBuilder.Register<IFoo>(x => x.ThenRegister<IBar>()).Create();

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

            // ACT
            var testAction = () =>
            {
                _diverterBuilder.Register<IFoo>(x => x
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
            var diverter = _diverterBuilder.Register<IFoo>(x => x.ThenRegister<IBar>()).Create();

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
            var diverter = _diverterBuilder
                .Register<IFoo>(inner =>
                    inner.ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")))
                .Create();
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
            var diverter = _diverterBuilder.Register<IFoo>(inner =>
                    inner.ThenDecorate<int>(n => n + 1))
                .Create();
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
            var diverter = _diverterBuilder
                .Register<IFoo>(x => x
                    .ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")))
                .Create();

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
            var diverter = _diverterBuilder
                .Register<IFoo>(x => x
                    .ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")))
                .Create();

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
            var diverter = _diverterBuilder
                .Register<IFoo>(x => x
                    .ThenDecorate<IBar>(bar => new Bar(bar.Name + " decorated")))
                .Create();

            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            diverter.Redirect<IFoo>().Strict();
            
            // ACT
            var testAction = () => fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
    }
}
