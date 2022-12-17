using System.Collections.Generic;
using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaRedirectTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        private readonly IFoo _proxy;

        public ViaRedirectTests()
        {
            _proxy = _redirect.Proxy(new Foo());
        }
        
        [Fact]
        public void GivenViaRedirect_ShouldDefaultToRoot()
        {
            // ARRANGE
            _redirect.To(x => x.GetFoo()).ViaRedirect();

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe(_proxy.Name);
        }

        [Fact]
        public void GivenViaRedirect_withVia_ShouldRedirect()
        {
            // ARRANGE
            var fooRedirect = _redirect
                .To(x => x.GetFoo())
                .ViaRedirect("nested");
            
            fooRedirect.To(x => x.Name).Via("Diverted");

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenParentRedirectReset_ViaRedirectShouldStillRedirect()
        {
            // ARRANGE
            var fooRedirect = _redirect
                .To(x => x.GetFoo())
                .ViaRedirect("nested");
            
            fooRedirect.To(x => x.Name).Via("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _redirect.Reset();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenResetViaRedirectGroup_ShouldReset()
        {
            // ARRANGE
            var fooRedirect = _redirect
                .To(x => x.GetFoo())
                .ViaRedirect("nested");
            
            fooRedirect.To(x => x.Name).Via("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _redirect.RedirectSet.Reset("nested");

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
        
        [Fact]
        public void GivenResetAll_ShouldResetViaRedirect()
        {
            // ARRANGE
            var fooRedirect = _redirect
                .To(x => x.GetFoo())
                .ViaRedirect("nested");
            
            fooRedirect.To(x => x.Name).Via("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _redirect.RedirectSet.ResetAll();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
        
        [Fact]
        public void GivenViaRedirect_WithVia_ShouldRedirect()
        {
            // ARRANGE
            var viaRedirect = _redirect
                .To(x => x.EchoGeneric(Is<IList<string>>.Any))
                .Via<(IList<string> input, __)>((_, args) => args.input.Select(x => $"via: {x}").ToList())
                .ViaRedirect();
            
            viaRedirect
                .To(x => x[Is<int>.Any])
                .Via<(int index, __)>((call, args) => call.Next[args.index] + " diverted");

            IList<string> input = Enumerable.Range(0, 10).Select(x => $"test{x}").ToList();
            var divertedList = _proxy.EchoGeneric(input);
            
            // ACT
            var results = new List<string>();
            for (var i = 0; i < divertedList.Count; i++)
            {
                results.Add(divertedList[i]);
            }
            
            // ASSERT
            results.ShouldBe(input.Select(x => $"via: {x} diverted"));
        }
        
        [Fact]
        public void GivenViaRedirectWithMultipleProxies_ShouldRedirect()
        {
            // ARRANGE
            var numberRedirect = _redirect
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .ViaRedirect();

            var counter = 0;

            numberRedirect
                .To(x => x.GetNumber(Is<int>.Any))
                .Via(call =>
                {
                    counter += 10;
                    return call.CallNext() + counter;
                });
            
            var numberProxy1 = _proxy.EchoGeneric<INumber>(new Number());
            var numberProxy2 = _proxy.EchoGeneric<INumber>(new Number());
            
            // ACT
            var result1 = numberProxy1.GetNumber(1);
            var result2 = numberProxy2.GetNumber(1);

            // ASSERT
            result1.ShouldBe(11);
            result2.ShouldBe(21);
        }
        
        [Fact]
        public void GivenViaRedirect_WhenSameReturnInstance_ThenShouldCacheProxies()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .ViaRedirect();

            var number = new Number();
            
            // ACT
            var numberProxy1 = _proxy.EchoGeneric<INumber>(number);
            var numberProxy2 = _proxy.EchoGeneric<INumber>(number);
            var numberProxy3 = _proxy.EchoGeneric<INumber>(new Number());
            
            // ASSERT
            numberProxy1.ShouldBeSameAs(numberProxy2);
            numberProxy3.ShouldNotBeSameAs(numberProxy1);
        }
        
        [Fact]
        public void GivenViaRedirect_WhenReturnIsNull_ThenShouldReturnNull()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<INumber?>.Any))
                .Via(() => null)
                .ViaRedirect();

            // ACT
            var result = _proxy.EchoGeneric<INumber>(new Number());
            
            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenMultipleViaRedirects_ShouldRedirect()
        {
            // ARRANGE
            var numberRedirect1 = _redirect
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .ViaRedirect("redirect1");
            
            var numberRedirect2 = _redirect
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .ViaRedirect("redirect2");

            numberRedirect1
                .To(x => x.GetNumber(Is<int>.Any))
                .Via<(int input, __)>(call =>
                {
                    var result = call.CallNext() + call.Args.input * 10;
                    return result;
                });
            
            numberRedirect2
                .To(x => x.GetNumber(Is<int>.Any))
                .Via<(int input, __)>(call =>
                {
                    var result = call.CallNext() + call.Args.input * 100;
                    return result;
                });

            var number = new Number();
            var numberProxy1 = _proxy.EchoGeneric<INumber>(number);
            var numberProxy2 = _proxy.EchoGeneric<INumber>(number);
            
            // ACT
            var result1 = numberProxy1.GetNumber(1);
            var result2 = numberProxy2.GetNumber(2);

            // ASSERT
            result1.ShouldBe(111);
            result2.ShouldBe(222);
        }
    }
}