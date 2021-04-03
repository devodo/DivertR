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
        public void GivenProxy_WhenAddRedirectWithRelayToOriginal_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.RedirectTo(new Foo(() => $"hello {_via.Relay.Original.Message}"));

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
            _via.RedirectTo(new Foo(() => $"hello {_via.Relay.Next.Message}"));

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
            _via.RedirectTo(new Foo(() =>
            {
                originalReference = _via.Relay.CallInfo.Original;
                return $"hello {originalReference!.Message}";
            }));

            // ASSERT
            proxy.Message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenWhenRedirect_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .Redirect(x => x.GetMessage("test"))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.GetMessage("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenExpressionRedirectWithLiteralParam_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .Redirect(x => x.GetMessage("test"))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.GetMessage("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }

        [Fact]
        public void GivenExpressionRedirectWithVariableParam_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var match = "test";
            via
                .Redirect(x => x.GetMessage(match))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.GetMessage(match);

            // ASSERT
            message.ShouldBe("hello foo test");
        }

        [Fact]
        public void GivenWhenRedirectWithMatchParam_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var input = new Wrapper<string>("test");
            via
                .Redirect(x => x.GetMessage(Is<string>.Match(p => p == input.Item)))
                .To((string i) => $"{via.Relay.Original.Message} {i}");

            // ACT
            input.Item = "other";
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.GetMessage(input.Item);

            // ASSERT
            message.ShouldBe("hello foo other");
        }
        
        [Fact]
        public void GivenWhenRedirectWithAnyParam_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .Redirect(x => x.GetMessage(Is<string>.Any))
                .To((string input) => $"{via.Relay.Original.Message} {input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.GetMessage("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenWhenPropertyRedirect_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .Redirect(x => x.Message)
                .To(() => $"before {via.Relay.Original.Message} after");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("before hello foo after");
        }
        
        [Fact]
        public void GivenSetPropertyRedirect_ShouldDivert()
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
        public void GivenRefInputRedirect_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            var test = new Number(x => x * 2);
            via
                .Redirect()
                .To(test);

            // ACT
            int input = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(test.GetNumber(5));
        }
        
        [Fact]
        public void GivenRefArrayInputRedirect_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            var test = new Number(x => x * 2);
            via
                .Redirect()
                .To(test);

            // ACT
            int[] inputOriginal = {5};
            var input = inputOriginal;
            var proxy = via.Proxy(new Number());
            proxy.RefArrayNumber(ref input);

            // ASSERT
            input[0].ShouldBe(test.GetNumber(5));
        }

        delegate void RefCall(ref int input);
        
        [Fact]
        public void GivenRefDelegate_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            int input = 5;
            via
                .Redirect(x => x.RefNumber(ref input))
                .To(new RefCall((ref int i2) =>
                {
                    i2 = 50;
                }));

            // ACT
            var i2 = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref i2);

            // ASSERT
            i2.ShouldBe(50);
        }

        [Fact]
        public void GivenRefDelegateWithAnyParam_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<INumber>();
            via
                .Redirect(x => x.RefNumber(ref Is<int>.AnyRef))
                .To(new RefCall((ref int i2) =>
                {
                    i2 = 50;
                }));

            // ACT
            var input = 5;
            var proxy = via.Proxy(new Number());
            proxy.RefNumber(ref input);

            // ASSERT
            input.ShouldBe(50);
        }

        [Fact]
        public void GivenGenericInputRedirect_ShouldDivert()
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
        public void GivenRedirectWithGenericMismatch_ShouldNotDivert()
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
        public void GivenArrayInputRedirect_ShouldDivert()
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
            var mock = new Mock<IFoo>();

            var input = new Wrapper<string>("test");
            
            mock
                .Setup(x => x.SetMessage(input))
                .Returns((Wrapper<string> i) => i.Item);
            
            _via.Redirect(x => x.GetMessage(input.Item))
                .To((string i) => i);
            
            // ACT
            input.Item = "test";
            
            var result = _via.Proxy().GetMessage(input.Item);

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
            var mock = new Mock<IFoo>();
            
            mock
                .Setup(x => x.SetMessage(GetInput()))
                .Returns((Wrapper<string> i) => i.Item);
            
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
    }
}