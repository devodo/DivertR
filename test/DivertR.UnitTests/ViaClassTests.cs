using DivertR.DynamicProxy;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaClassTests
    {
        private static readonly DiverterSettings DiverterSettings = new(new DynamicProxyFactory());

        private readonly IVia<Foo> _via;

        public ViaClassTests()
        {
            _via = new ViaSet(DiverterSettings).Via<Foo>();
        }
        
        [Fact]
        public void GivenClassProxy_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            var message = proxy.NameVirtual;
            
            // ASSERT
            message.ShouldBe(original.Name);
        }

        [Fact]
        public void GivenClassProxy_WhenTargetRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");

            // ACT
            _via.Retarget(foo);

            // ASSERT
            proxy.NameVirtual.ShouldBe(foo.Name);
        }
        
        [Fact]
        public void GivenClassProxy_WhenDelegateRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));

            // ACT
            _via.To(x => x.NameVirtual).Redirect(() => _via.Relay.Next.NameVirtual + " bar");

            // ASSERT
            proxy.NameVirtual.ShouldBe("hello foo bar");
        }
        
        [Fact]
        public void GivenClassProxy_ShouldCopyProperty()
        {
            // ARRANGE
            var original = new Foo("hello foo");

            // ACT
            var proxy = _via.Proxy(original);

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenClassProxy_ShouldCopyBackingField()
        {
            // ARRANGE
            var original = new Foo("hello foo");

            // ACT
            var proxy = _via.Proxy(original);

            // ASSERT
            proxy.CreatedNameBacked.ShouldBe(original.CreatedNameBacked);
        }
        
        [Fact]
        public void GivenClassProxy_ShouldCopyGetOnlyProperty()
        {
            // ARRANGE
            var original = new Foo("hello foo");

            // ACT
            var proxy = _via.Proxy(original);

            // ASSERT
            proxy.CreatedName.ShouldBe(original.CreatedName);
        }
        
        [Fact]
        public void GivenClassProxy_WhenCopyFieldsDisabled_ShouldNotCopyPropertyBackingField()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = new Via<Foo>(new DiverterSettings(proxyFactory: new DynamicProxyFactory(copyRootFields: false)));

            // ACT
            var proxy = via.Proxy(original);

            // ASSERT
            proxy.Name.ShouldBe((new Foo()).Name);
            proxy.Name.ShouldNotBe(original.Name);
        }
        
        [Fact]
        public void GivenClassProxy_WhenAddRedirectToNonVirtualGetter_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _via.To(x => x.Name);

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenClassProxy_WhenAddRedirectToNonVirtualSetter_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _via.ToSet(x => x.Name);

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenClassProxy_WhenAddRedirectToNonVirtualMethod_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _via.To(x => x.Echo(Is<string>.Any));

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenClassProxy_WhenProxyDerivedClass_ShouldCopyPrivateField()
        {
            // ARRANGE
            var original = new DerivedFoo("hello foo", "other");
            original._publicField = "test";
            var via = new Via<DerivedFoo>(new DiverterSettings(proxyFactory: new DynamicProxyFactory()));

            // ACT
            var proxy = via.Proxy(original);

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