using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class DiverterBuilderTests
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
            var diverter = _diverterBuilder.IncludeRedirect<IFoo>().Create();

            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddNamedRedirect_ShouldAdd()
        {
            // ARRANGE
            var diverter = _diverterBuilder.IncludeRedirect<IFoo>("test").Create();

            // ACT
            var foo = diverter.Redirect<IFoo>("test").Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddRedirectByType_ShouldAdd()
        {
            // ARRANGE
            var diverter = _diverterBuilder.IncludeRedirect(typeof(IFoo)).Create();

            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddNamedRedirectByType_ShouldAdd()
        {
            // ARRANGE
            var diverter = _diverterBuilder.IncludeRedirect(typeof(IFoo), "test").Create();

            // ACT
            var foo = diverter.Redirect<IFoo>("test").Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void AddNestedRedirect_ShouldProxyChain()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>()
                .Create();

            diverter
                .Redirect<IBar>()
                .To(x => x.Name)
                .Via(call => call.CallNext() + " diverted");
            
            // ACT
            var foo = diverter.Redirect<IFoo>().Proxy(new Foo());
            var bar = foo.EchoGeneric<IBar>(new Bar("bar"));
            
            // ASSERT
            bar.Name.ShouldBe("bar diverted");
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenRegisteredTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>()
                .Create();
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
        public void GivenNamedNestedRegistration_WhenRegisteredTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>("group")
                .Create();
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());

            diverter
                .Redirect<IBar>("group")
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
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>()
                .Create();
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
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>()
                .Create();
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
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>()
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
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IFoo>()
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
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>()
                .Create();

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
                _diverterBuilder
                    .Register<IFoo>()
                    .Redirect<IFoo>().ViaRedirect<IBar>()
                    .Redirect<IFoo>().ViaRedirect<IFoo>()
                    .Redirect<IFoo>().ViaRedirect<IBar>();
            };

            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void GivenNestedRegistration_WhenStrictMode_ShouldDisableSatisfyStrict()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().ViaRedirect<IBar>()
                .Create();

            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            diverter.Redirect<IFoo>().Strict();
            
            // ACT
            var testAction = () => fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
        
        [Fact]
        public void GivenNestedRedirect_WhenRedirectTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().To(x => x.EchoGeneric(Is<IBar>.Any)).ViaRedirect()
                .Create();
            
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
        public void GivenNamedNestedRedirect_WhenRedirectTypeReturned_ShouldProxy()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().To(x => x.EchoGeneric(Is<IBar>.Any)).ViaRedirect("group")
                .Create();
            
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());

            diverter
                .Redirect<IBar>("group")
                .To(x => x.Name)
                .Via(call => call.CallNext() + " redirected");
            
            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar redirected");
        }
        
        [Fact]
        public void GivenNestedRedirect_WhenRedirectReset_ShouldPersist()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().To(foo => foo.EchoGeneric(Is<IBar>.Any)).ViaRedirect()
                .Create();

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
        public void GivenNestedRedirect_WhenStrictMode_ShouldDisableSatisfyStrict()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().To(foo => foo.EchoGeneric(Is<IBar>.Any)).ViaRedirect()
                .Create();

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
                .Register<IFoo>()
                .Redirect<IFoo>().Decorate<IBar>(bar => new Bar(bar.Name + " decorated"))
                .Create();
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            
            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar decorated");
        }
        
        [Fact]
        public void GivenNestedDiverterDecorator_WhenRegisteredTypeReturned_ShouldDecorate()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .IncludeRedirect<IBar>()
                .Redirect<IFoo>().Decorate<IBar>((bar, d) => d.Redirect<IBar>().Proxy(new Bar(bar.Name + " decorated")))
                .Create();
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            
            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar decorated");
        }
        
        [Fact]
        [SuppressMessage("ReSharper", "BadIndent")]
        public void GivenNestedToDecorator_WhenRegisteredTypeReturned_ShouldDecorate()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>()
                    .To(x => x.EchoGeneric(Is<IBar>.Any))
                    .Decorate(bar => new Bar(bar.Name + " decorated"))
                .Create();
            var fooProxy = diverter.Redirect<IFoo>().Proxy(new Foo());
            
            // ACT
            var bar = fooProxy.EchoGeneric<IBar>(new Bar("bar"));

            // ASSERT
            bar.Name.ShouldBe("bar decorated");
        }
        
        [Fact]
        [SuppressMessage("ReSharper", "BadIndent")]
        public void GivenNestedToDiverterDecorator_WhenRegisteredTypeReturned_ShouldDecorate()
        {
            // ARRANGE
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .IncludeRedirect<IBar>()
                .Redirect<IFoo>()
                    .To(x => x.EchoGeneric(Is<IBar>.Any))
                    .Decorate((bar, d) => d.Redirect<IBar>().Proxy(new Bar(bar.Name + " decorated")))
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
            var diverter = _diverterBuilder
                .Register<IFoo>()
                .Redirect<IFoo>().Decorate<int>(n => n + 1)
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
                .Register<IFoo>()
                .Redirect<IFoo>().Decorate<IBar>(bar => new Bar(bar.Name + " decorated"))
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
                .Register<IFoo>()
                .Redirect<IFoo>().Decorate<IBar>(bar => new Bar(bar.Name + " decorated"))
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
                .Register<IFoo>()
                .Redirect<IFoo>().Decorate<IBar>(bar => new Bar(bar.Name + " decorated"))
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
