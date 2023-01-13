using DivertR.DynamicProxy;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectClassTests
    {
        private static readonly DiverterSettings DiverterSettings = new(new DynamicProxyFactory());

        private readonly IRedirect<Foo> _redirect;

        public RedirectClassTests()
        {
            _redirect = new RedirectSet(DiverterSettings).GetOrCreate<Foo>();
        }
        
        [Fact]
        public void GivenClassProxy_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _redirect.Proxy(original);
            
            // ACT
            var message = proxy.NameVirtual;
            
            // ASSERT
            message.ShouldBe(original.Name);
        }

        [Fact]
        public void GivenClassProxy_WhenRetarget_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");

            // ACT
            _redirect.Retarget(foo);

            // ASSERT
            proxy.NameVirtual.ShouldBe(foo.Name);
        }
        
        [Fact]
        public void GivenClassProxy_WhenDelegateVia_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));

            // ACT
            _redirect.To(x => x.NameVirtual).Via(() => _redirect.Relay.Next.NameVirtual + " bar");

            // ASSERT
            proxy.NameVirtual.ShouldBe("hello foo bar");
        }
        
        [Fact]
        public void GivenClassProxy_ShouldCopyProperty()
        {
            // ARRANGE
            var original = new Foo("hello foo");

            // ACT
            var proxy = _redirect.Proxy(original);

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenClassProxy_ShouldCopyBackingField()
        {
            // ARRANGE
            var original = new Foo("hello foo");

            // ACT
            var proxy = _redirect.Proxy(original);

            // ASSERT
            proxy.CreatedNameBacked.ShouldBe(original.CreatedNameBacked);
        }
        
        [Fact]
        public void GivenClassProxy_ShouldCopyGetOnlyProperty()
        {
            // ARRANGE
            var original = new Foo("hello foo");

            // ACT
            var proxy = _redirect.Proxy(original);

            // ASSERT
            proxy.CreatedName.ShouldBe(original.CreatedName);
        }
        
        [Fact]
        public void GivenClassProxy_WhenCopyFieldsDisabled_ShouldNotCopyPropertyBackingField()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var redirect = new Redirect<Foo>(new DiverterSettings(proxyFactory: new DynamicProxyFactory(copyRootFields: false)));

            // ACT
            var proxy = redirect.Proxy(original);

            // ASSERT
            proxy.Name.ShouldBe((new Foo()).Name);
            proxy.Name.ShouldNotBe(original.Name);
        }
        
        [Fact]
        public void GivenClassProxy_WhenAddViaToNonVirtualGetter_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.To(x => x.Name);

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenClassProxy_WhenAddViaToNonVirtualSetter_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.ToSet(x => x.Name);

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenClassProxy_WhenAddViaToNonVirtualMethod_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.To(x => x.Echo(Is<string>.Any));

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenClassProxy_WhenProxyDerivedClass_ShouldCopyPrivateField()
        {
            // ARRANGE
            var original = new DerivedFoo("hello foo", "other");
            original._publicField = "test";
            var redirect = new Redirect<DerivedFoo>(new DiverterSettings(proxyFactory: new DynamicProxyFactory()));

            // ACT
            var proxy = redirect.Proxy(original);

            // ASSERT
            proxy.CreatedName.ShouldBe(original.CreatedName);
            proxy.AltName.ShouldBe(original.AltName);
            proxy._publicField.ShouldBe(original._publicField);
        }

        public class DerivedFoo : Foo
        {
            public DerivedFoo(string name, string altName) : base(name)
            {
                AltName = altName;
            }
            
            public string AltName { get; }
        }
    }
}