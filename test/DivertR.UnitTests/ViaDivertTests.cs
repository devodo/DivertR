using System.Collections.Generic;
using System.Linq;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaDivertTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();
        private readonly IFoo _proxy;

        public ViaDivertTests()
        {
            _proxy = _via.Proxy(new Foo());
        }
        
        [Fact]
        public void GivenDivert_ShouldDefaultToOriginal()
        {
            // ARRANGE
            _via.To(x => x.GetFoo()).Divert();

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe(_proxy.Name);
        }

        [Fact]
        public void GivenDivertRedirect_ShouldDivertAndRedirect()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");

            // ACT
            var result = _proxy.GetFoo().Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenParentViaReset_DivertShouldStillRedirect()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.Reset();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("Diverted");
        }
        
        [Fact]
        public void GivenResetDivertViaGroup_ShouldResetDivert()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.ViaSet.Reset("nested");

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
        
        [Fact]
        public void GivenResetAll_ShouldResetDivert()
        {
            // ARRANGE
            var fooVia = _via
                .To(x => x.GetFoo())
                .Divert("nested");
            
            fooVia.To(x => x.Name).Redirect("Diverted");
            var nestedFoo = _proxy.GetFoo();
            _via.ViaSet.ResetAll();

            // ACT
            var result = nestedFoo.Name;
            
            // ASSERT
            result.ShouldBe("original");
        }
        
        [Fact]
        public void GivenRedirectDivert_ShouldRedirectAndDivert()
        {
            // ARRANGE
            var divertedVia = _via
                .To(x => x.EchoGeneric(Is<IList<string>>.Any))
                .Redirect<(IList<string> input, __)>((_, args) => args.input.Select(x => $"redirect: {x}").ToList())
                .Divert();
            
            divertedVia
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
    }
}