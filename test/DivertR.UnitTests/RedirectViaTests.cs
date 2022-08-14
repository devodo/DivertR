using System.Collections.Generic;
using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectViaTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IFoo _proxy;

        public RedirectViaTests()
        {
            _proxy = _via.Proxy(new Foo());
        }
        
        [Fact]
        public void GivenRedirectVia_ShouldDefaultToRoot()
        {
            // ARRANGE
            _via.To(x => x.GetFoo()).RedirectVia();

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe(_proxy.Name);
        }

        [Fact]
        public void GivenRedirectVia_withRedirect_ShouldRedirect()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .RedirectVia("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenParentViaReset_RedirectViaShouldStillRedirect()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .RedirectVia("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.Reset();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenResetRedirectViaGroup_ShouldReset()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .RedirectVia("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.ViaSet.Reset("nested");

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
        
        [Fact]
        public void GivenResetAll_ShouldResetRedirectVia()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .RedirectVia("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.ViaSet.ResetAll();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
        
        [Fact]
        public void GivenRedirectVia_WithRedirect_ShouldRedirect()
        {
            // ARRANGE
            var redirectVia = _via
                .To(x => x.EchoGeneric(Is<IList<string>>.Any))
                .Redirect<(IList<string> input, __)>((_, args) => args.input.Select(x => $"redirect: {x}").ToList())
                .RedirectVia();
            
            redirectVia
                .To(x => x[Is<int>.Any])
                .Redirect<(int index, __)>((call, args) => call.Next[args.index] + " diverted");

            IList<string> input = Enumerable.Range(0, 10).Select(x => $"test{x}").ToList();
            var divertedList = _proxy.EchoGeneric(input);
            
            // ACT
            var results = new List<string>();
            for (var i = 0; i < divertedList.Count; i++)
            {
                results.Add(divertedList[i]);
            }
            
            // ASSERT
            results.ShouldBe(input.Select(x => $"redirect: {x} diverted"));
        }
        
        [Fact]
        public void GivenRedirectViaWithMultipleProxies_ShouldRedirect()
        {
            // ARRANGE
            var numberVia = _via
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .RedirectVia();

            var counter = 0;

            numberVia
                .To(x => x.GetNumber(Is<int>.Any))
                .Redirect(call =>
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
        public void GivenRedirectVia_WhenSameReturnInstance_ThenShouldCacheProxies()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .RedirectVia();

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
        public void GivenRedirectVia_WhenReturnIsNull_ThenShouldReturnNull()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<INumber?>.Any))
                .Redirect(() => null)
                .RedirectVia();

            // ACT
            var result = _proxy.EchoGeneric<INumber>(new Number());
            
            // ASSERT
            result.ShouldBeNull();
        }
        
        [Fact]
        public void GivenMultipleRedirectVias_ShouldRedirect()
        {
            // ARRANGE
            var numberVia1 = _via
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .RedirectVia("via1");
            
            var numberVia2 = _via
                .To(x => x.EchoGeneric(Is<INumber>.Any))
                .RedirectVia("via2");

            numberVia1
                .To(x => x.GetNumber(Is<int>.Any))
                .Redirect<(int input, __)>(call =>
                {
                    var result = call.CallNext() + call.Args.input * 10;
                    return result;
                });
            
            numberVia2
                .To(x => x.GetNumber(Is<int>.Any))
                .Redirect<(int input, __)>(call =>
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