using System;
using DivertR.Core;
using DivertR.UnitTests.Model;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaDelegateRedirectTests
    {
        private readonly Via<IFoo> _via = new();

        [Fact]
        public void GivenProxyWithDelegateRedirect_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var redirectMessage = "hi DivertR";
            _via.To(x => x.Name).Redirect(redirectMessage);
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(redirectMessage);
        }
        
        [Fact]
        public void GivenStrictModeWithNoRedirect_ShouldThrowException()
        {
            // ARRANGE
            _via.Strict();
            var proxy = _via.Proxy(new Foo("hello foo"));

            // ACT
            Func<string> testAction = () => proxy.Name;

            // ASSERT
            testAction.ShouldThrow<DiverterException>();
        }
        
        [Fact]
        public void GivenProxyWithRedirect_WhenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.To(x => x.Name).Redirect("test");

            // ACT
            _via.Reset();

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenProxyWithCallNextRedirect_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.To(x => x.Name).Redirect(() => (string) _via.Relay.CallNext());

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenProxyWithRedirectWithRelayToOriginal_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(() => $"hello {_via.Relay.Original.Name}");

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxyWithRedirectWithRelayToNext_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(() => $"hello {_via.Next.Name}");
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxyWithRedirectWithRelayToOriginalInstance_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;
            _via
                .To(x => x.Name)
                .Redirect(() =>
                {
                    originalReference = _via.Relay.CallInfo.Original;
                    return $"hello {originalReference!.Name}";
                });

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenConstantExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.Echo("test"))
                .Redirect((string input) => $"{via.Relay.Original.Name} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenConstantExpressionRedirect_WhenCallDoesNotMatch_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.Echo("test"))
                .Redirect((string input) => $"{via.Relay.Original.Name} {input}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("no match");
        }

        [Fact]
        public void GivenVariableExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var match = "test";
            via
                .To(x => x.Echo(match))
                .Redirect((string input) => $"{via.Relay.Original.Name} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo(match);

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenVariableExpressionRedirect_WhenCallDoesNotMatch_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var input = new Wrapper<string>("test");
            via
                .To(x => x.Echo(input.Item))
                .Redirect((string i) => $"{via.Relay.Original.Name} {i}");

            // ACT
            var proxy = via.Proxy(new Foo());
            input.Item = "no match";
            var message = proxy.Echo(input.Item);

            // ASSERT
            message.ShouldBe("no match");
        }

        [Fact]
        public void GivenMatchExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var input = new Wrapper<string>("test");
            via
                .To(x => x.Echo(Is<string>.Match(p => p == input.Item)))
                .Redirect((string i) => $"{via.Relay.Original.Name} {i}");

            // ACT
            input.Item = "other";
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo(input.Item);

            // ASSERT
            message.ShouldBe("hello foo other");
        }
        
        [Fact]
        public void GivenMatchExpressionRedirect_WhenCallDoesNotMatch_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.Echo(Is<string>.Match(p => p == "test")))
                .Redirect((string i) => $"{via.Relay.Original.Name} {i}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("no match");
        }
        
        [Fact]
        public void GivenIsAnyExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect((string input) => $"{via.Relay.Original.Name} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenPropertyExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var input = new Wrapper<string>("Hello");

            _via.To(x => x.Echo(input.Item))
                .Redirect((string i) => $"{i} - {_via.Next.Name}");
            
            // ACT
            var result = _via.Proxy(new Foo("Foo")).Echo(input.Item);

            // ASSERT
            result.ShouldBe("Hello - Foo");
        }
        
        string GetMethodValue()
        {
            return "test";
        }
        
        [Fact]
        public void GivenExpressionMethodBodyRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            _via.To(x => x.Echo(GetMethodValue()))
                .Redirect<string>(i => "matched");
            
            // ACT
            var result = _via.Proxy().Echo(GetMethodValue());

            // ASSERT
            result.ShouldBe("matched");
        }

        [Fact]
        public void GivenSetPropertyRedirect_WhenValueMatches_ShouldRedirect()
        {
            // ARRANGE
            _via
                .ToSet(x => x.Name, () => Is<string>.Any)
                .Redirect((string value) => _via.Relay.Original.Name = $"New {value} set");

            // ACT
            var proxy = _via.Proxy(new Foo("hello foo"));
            proxy.Name = "test";

            // ASSERT
            proxy.Name.ShouldBe("New test set");
        }
        
        [Fact]
        public void GivenSetPropertyRedirect_WhenValueDoesNotMatch_ShouldDefaultToOriginal()
        {
            // ARRANGE
            _via
                .ToSet(x => x.Name, () => Is<string>.Match(s => s == "test"))
                .Redirect((string value) => _via.Relay.Original.Name = $"New {value} set");

            // ACT
            var proxy = _via.Proxy(new Foo("hello foo"));
            proxy.Name = "no match";

            // ASSERT
            proxy.Name.ShouldBe("no match");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericTypeMatch_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<string>.Any))
                .Redirect((string i) => $"{i} - {_via.Relay.Next.Name}");

            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello - foo");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericAssignable_ShouldDivert()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<string>.Any))
                .Redirect((string i) => $"{i} - {_via.Relay.Next.Name}");
            
            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello - foo");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericNotAssignable_ShouldDefaultToOriginal()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<int>.Any))
                .Redirect((int i) => $"{i} - {_via.Relay.Next.Name}");
            
            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenRedirectWithArrayParameter_WhenIsAny_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            
            via.Retarget(new Number(x => x * 2));
            via
                .To(x => x.ArrayNumber(Is<int[]>.Any))
                .Redirect((int[] inputs) =>
                {
                    via.Relay.Next.ArrayNumber(inputs);
                    
                    for (var i = 0; i < inputs.Length; i++)
                    {
                        inputs[i] = inputs[i] + 1;
                    }
                });
            
            // ACT
            var input = new[] {1, 5};
            var proxy = via.Proxy(new Number());
            proxy.ArrayNumber(input);

            // ASSERT
            input[0].ShouldBe(3);
            input[1].ShouldBe(11);
        }

        [Fact]
        public void GivenRedirectReturnTypeSameAsTargetType_ShouldRedirect()
        {
            // ARRANGE
            IFoo getFoo = new Foo();
            _via.To(x => x.GetFoo())
                .Redirect(getFoo);
            
            // ACT
            var result = _via.Proxy().GetFoo();

            // ASSERT
            result.ShouldBeSameAs(getFoo);
        }
        
        [Fact]
        public void GivenMultipleRedirectsWithOrderWeights_ShouldOrderRedirects()
        {
            // ARRANGE
            _via
                .To(x => x.Name).WithOrderWeight(30).Redirect(() => $"1 {_via.Next.Name} 1")
                .To(x => x.Name).WithOrderWeight(20).Redirect(() => $"2 {_via.Next.Name} 2")
                .To(x => x.Name).WithOrderWeight(10).Redirect(() => $"3 {_via.Next.Name} 3");
            
            // ACT
            var result = _via.Proxy(new Foo("hello")).Name;

            // ASSERT
            result.ShouldBe("1 2 3 hello 3 2 1");
        }
        
        [Fact]
        public void GivenProxyWithInsertMultipleOrderedRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            
            string WriteMessage(int num)
            {
                return $"{num} {_via.Relay.Next.Name} {num}";
            }
            
            _via
                .To(x => x.Name).WithOrderWeight(30).Redirect(() => WriteMessage(1))
                .To(x => x.Name).WithOrderWeight(20).Redirect(() => WriteMessage(2))
                .To(x => x.Name).WithOrderWeight(10).Redirect(() => WriteMessage(3));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public void GivenRedirect_WithCallOriginal_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(() => (string) _via.Relay.CallOriginal());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRedirect_WithCallNext_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(() => (string) _via.Relay.CallNext());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenRedirect_WithCallNext_ShouldCallNextRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.To(x => x.Name).Redirect("test");
            _via
                .To(x => x.Name)
                .Redirect(() => (string) _via.Relay.CallNext());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("test");
        }
        
        [Fact]
        public void GivenRedirect_WithCallOriginal_WithMethod_ShouldCallOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect((string input) => $"{input}-test");
            _via
                .To(x => x.Echo("here"))
                .Redirect(() => (string) _via.Relay.CallOriginal(_via.Relay.CallInfo.Method, new CallArguments(new object[] {"alter"})));

            // ACT
            var result = proxy.Echo("here");

            // ASSERT
            result.ShouldBe("alter");
        }
        
        [Fact]
        public void GivenRedirect_WithCallNext_WithMethod_ShouldCallNextRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect((string input) => $"{input}-test");
            _via
                .To(x => x.Echo("here"))
                .Redirect(() => (string) _via.Relay.CallNext(_via.Relay.CallInfo.Method, new CallArguments(new object[] {"alter"})));

            // ACT
            var result = proxy.Echo("here");

            // ASSERT
            result.ShouldBe("alter-test");
        }
    }
}