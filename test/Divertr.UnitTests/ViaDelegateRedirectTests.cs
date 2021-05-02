using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaDelegateRedirectTests
    {
        private readonly Via<IFoo> _via = new();

        [Fact]
        public void GivenProxy_WhenAddRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var redirectMessage = "hi DivertR";
            
            // ACT
            _via.Redirect(x => x.Message).To(redirectMessage);

            // ASSERT
            proxy.Message.ShouldBe(redirectMessage);
        }
        
        [Fact]
        public void GivenProxyWithRedirect_WhenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.Redirect(x => x.Message).To("test");

            // ACT
            _via.Reset();

            // ASSERT
            proxy.Message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenProxy_WhenCallNextRedirect_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.Redirect(x => x.Message).To(() => (string) _via.Relay.CallNext());

            // ACT
            _via.Reset();

            // ASSERT
            proxy.Message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenProxy_WhenAddRedirectWithRelayToOriginal_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via
                .Redirect(x => x.Message)
                .To(() => $"hello {_via.Relay.Original.Message}");

            // ASSERT
            proxy.Message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxy_WhenAddRedirectWithRelayToNext_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            _via
                .Redirect(x => x.Message)
                .To(() => $"hello {_via.Next.Message}");

            // ASSERT
            proxy.Message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxy_WhenAddRedirectWithRelayToOriginalInstance_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;

            // ACT
            _via
                .Redirect(x => x.Message)
                .To(() =>
                {
                    originalReference = _via.Relay.CallInfo.Original;
                    return $"hello {originalReference!.Message}";
                });

            // ASSERT
            proxy.Message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenConstantExpressionRedirect_WhenCallMatches_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .Redirect(x => x.Echo("test"))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

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
                .Redirect(x => x.Echo("test"))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("no match");
        }

        [Fact]
        public void GivenVariableExpressionRedirect_WhenCallMatches_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var match = "test";
            via
                .Redirect(x => x.Echo(match))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

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
                .Redirect(x => x.Echo(input.Item))
                .To((string i) => $"{via.Relay.Original.Message} {i}");

            // ACT
            var proxy = via.Proxy(new Foo());
            input.Item = "no match";
            var message = proxy.Echo(input.Item);

            // ASSERT
            message.ShouldBe("no match");
        }

        [Fact]
        public void GivenMatchExpressionRedirect_WhenCallMatches_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var input = new Wrapper<string>("test");
            via
                .Redirect(x => x.Echo(Is<string>.Match(p => p == input.Item)))
                .To((string i) => $"{via.Relay.Original.Message} {i}");

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
                .Redirect(x => x.Echo(Is<string>.Match(p => p == "test")))
                .To((string i) => $"{via.Relay.Original.Message} {i}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("no match");
        }
        
        [Fact]
        public void GivenIsAnyExpressionRedirect_WhenCallMatches_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .Redirect(x => x.Echo(Is<string>.Any))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }

        [Fact]
        public void GivenSetPropertyRedirect_WhenValueMatches_ShouldDivert()
        {
            // ARRANGE
            _via
                .RedirectSet(x => x.Message, () => Is<string>.Any)
                .To((string value) => _via.Relay.Original.Message = $"New {value} set");

            // ACT
            var proxy = _via.Proxy(new Foo("hello foo"));
            proxy.Message = "test";

            // ASSERT
            proxy.Message.ShouldBe("New test set");
        }
        
        [Fact]
        public void GivenSetPropertyRedirect_WhenValueDoesNotMatch_ShouldDefaultToOriginal()
        {
            // ARRANGE
            _via
                .RedirectSet(x => x.Message, () => Is<string>.Match(s => s == "test"))
                .To((string value) => _via.Relay.Original.Message = $"New {value} set");

            // ACT
            var proxy = _via.Proxy(new Foo("hello foo"));
            proxy.Message = "no match";

            // ASSERT
            proxy.Message.ShouldBe("no match");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericTypeMatch_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            
            via.RedirectTo(new Number(x => x * 2));
            via.Redirect(x => x.GenericNumber(Is<string>.Any, Is<int>.Any))
                .To((string s, int i) => via.Relay.Next.GenericNumber(s, i) + " - again");
            
            // ACT
            var proxy = via.Proxy(new Number());
            var result = proxy.GenericNumber("Hello", 5);

            // ASSERT
            result.ShouldBe("Hello - 10 - again");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericAssignable_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            
            via.RedirectTo(new Number(x => x * 2));
            via.Redirect(x => x.GenericNumber(Is<object>.Any, Is<int>.Any))
                .To((object s, int i) => via.Relay.Next.GenericNumber(s, i) + " - again");
            
            // ACT
            var proxy = via.Proxy(new Number());
            var result = proxy.GenericNumber("Hello", 5);

            // ASSERT
            result.ShouldBe("Hello - 10 - again");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericNotAssignable_ShouldNotDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            
            via
                .Redirect(x => x.GenericNumber(Is<string>.Any, Is<int>.Any))
                .To((string s, int i) => via.Relay.Next.GenericNumber(s, i) + " - again");
            
            via.RedirectTo(new Number(x => x * 2));

            // ACT
            var proxy = via.Proxy(new Number());
            var result = proxy.GenericNumber(5, 5);

            // ASSERT
            result.ShouldBe("5 - 10");
        }
        
        [Fact]
        public void GivenRedirectWithArrayParameter_WhenIsAny_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            
            via.RedirectTo(new Number(x => x * 2));
            via
                .Redirect(x => x.ArrayNumber(Is<int[]>.Any))
                .To((int[] inputs) =>
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
        public void TestMock()
        {
            // ARRANGE
            var mock = new Mock<IFoo>();

            var input = new Wrapper<string>("test");
            
            mock
                .Setup(x => x.SetMessage(input))
                .Returns((Wrapper<string> i) => i.Item);
            
            _via.Redirect(x => x.SetMessage(input))
                .To((Wrapper<string> i) => i.Item);
            
            // ACT
            input.Item = "result";
            
            var result = _via.Proxy().SetMessage(input);

            // ASSERT
            result.ShouldBe("result");
        }
        
        [Fact]
        public void TestProperty()
        {
            // ARRANGE
            var input = new Wrapper<string>("test");

            _via.Redirect(x => x.Echo(input.Item))
                .To((string i) => i);
            
            // ACT
            input.Item = "test";
            
            var result = _via.Proxy().Echo(input.Item);

            // ASSERT
            result.ShouldBe("test");
        }
        
        Wrapper<string> GetInput()
        {
            return new("test");
        }
        
        [Fact]
        public void TestMethod()
        {
            // ARRANGE
            _via.Redirect(x => x.SetMessage(GetInput()))
                .To((Wrapper<string> i) => i.Item);
            
            // ACT
            var result = _via.Proxy().SetMessage(GetInput());

            // ASSERT
            result.ShouldBe("test");
        }
        
        [Fact]
        public void TestGetFoo()
        {
            // ARRANGE
            IFoo getFoo = new Foo();
            _via.Redirect(x => x.GetFoo())
                .To(getFoo);
            
            // ACT
            var result = _via.Proxy().GetFoo();

            // ASSERT
            result.ShouldBeSameAs(getFoo);
        }
        
        [Fact]
        public void GivenMultipleRedirectsWithOrderWeights_ShouldChain()
        {
            // ARRANGE
            _via
                .Redirect(x => x.Message).WithOrderWeight(30).To(() => $"1 {_via.Next.Message} 1")
                .Redirect(x => x.Message).WithOrderWeight(20).To(() => $"2 {_via.Next.Message} 2")
                .Redirect(x => x.Message).WithOrderWeight(10).To(() => $"3 {_via.Next.Message} 3");
            
            // ACT
            var result = _via.Proxy(new Foo("hello")).Message;

            // ASSERT
            result.ShouldBe("1 2 3 hello 3 2 1");
        }
    }
}