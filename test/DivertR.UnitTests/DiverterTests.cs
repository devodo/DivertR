using System;
using System.Collections.Generic;
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
        public void GivenRetargetWithRelay_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var via = _diverter.Via<IFoo>().Proxy(original);

            // ACT
            _diverter.Via<IFoo>().Retarget(new FooAlt(() => $"{_diverter.Via<IFoo>().Relay.Next.Name} diverted"));
            
            // ASSERT
            via.Name.ShouldBe(original.Name + " diverted");
        }

        [Fact]
        public void GivenRedirects_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = _diverter.Via<IFoo>();
            var subject = via.Proxy(original);
            
            via.Retarget(new FooAlt(() => $"{via.Relay.Next} me"));
            via.Retarget(new FooAlt(() => $"{via.Relay.Next} again"));

            // ACT
            _diverter.ResetAll();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRedirects_WhenResetGroup_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = _diverter.Via<IFoo>();
            var subject = via.Proxy(original);
            
            via.Retarget(new FooAlt(() => $"{via.Relay.Next} me"));
            via.Retarget(new FooAlt(() => $"{via.Relay.Next} again"));

            // ACT
            _diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRegisteredVia_ShouldSetStrict()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = _diverter.Via<IFoo>();
            var subject = via.Proxy(original);

            // ACT
            _diverter.Strict();
            
            // ASSERT
            Func<string> testAction = () => subject.Name;
            testAction.ShouldThrow<StrictNotSatisfiedException>();
        }
        
        [Fact]
        public void GivenStrict_WhenResetAll_ShouldReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = _diverter.Via<IFoo>();
            var subject = via.Proxy(original);
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
            var via = _diverter.Via<IFoo>();
            var subject = via.Proxy(original);
            _diverter.Strict();

            // ACT
            _diverter.Reset();
            
            // ASSERT
            subject.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRegisterByType_ShouldRegisterVia()
        {
            // ARRANGE
            var diverter = new Diverter();
            diverter.Register(typeof(IFoo));

            // ACT
            var foo = diverter.Via<IFoo>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
        }
        
        [Fact]
        public void GivenRegisterByTypes_ShouldRegisterVia()
        {
            // ARRANGE
            var diverter = new Diverter();
            diverter.Register(new[] { typeof(IFoo), typeof(IList<int>) });

            // ACT
            var foo = diverter.Via<IFoo>().Proxy();
            var list = diverter.Via<IList<int>>().Proxy();
            
            // ASSERT
            foo.ShouldNotBeNull();
            list.ShouldNotBeNull();
        }
    }
}
