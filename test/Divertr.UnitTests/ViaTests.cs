using System.Linq;
using System.Threading;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaTests
    {
        private readonly Via<IFoo> _via = new();
        
        [Fact]
        public void GivenProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenRedirect_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            _via.Redirect().To(foo);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe(foo.Message);
        }
        
        [Fact]
        public void GivenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new Foo("diverted"));
            _via.Reset();

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact]
        public void GivenRedirectWithOriginalReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new Foo(() => $"hello {_via.Relay.Original.Message}"));

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRedirectWithNextReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.RedirectTo(new Foo(() => $"hello {_via.Relay.Next.Message}"));

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenRedirectWithOriginalInstanceReference_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo originalReference = null;
            _via.RedirectTo(new Foo(() =>
            {
                originalReference = _via.Relay.OriginalInstance;
                return $"hello {originalReference!.Message}";
            }));

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello foo");
            originalReference.ShouldBeSameAs(original);
        }
        
        [Fact]
        public void GivenMultipleProxiesWithOriginalRelay_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();
            
            _via.RedirectTo(new Foo( () => $"diverted {_via.Relay.Original.Message}"));

            // ACT
            var messages = proxies.Select(p => p.Message).ToList();

            // ASSERT
            for (var i = 0; i < messages.Count; i++)
            {
                messages[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMultipleProxiesWithNextRelay_ShouldDivert()
        {
            // ARRANGE
            var proxies = Enumerable.Range(0, 10)
                .Select(i => _via.Proxy(new Foo($"foo{i}")))
                .ToList();

            _via
                .RedirectTo(new Foo( () => $"diverted {_via.Relay.Next.Message}"));

            // ACT
            var messages = proxies.Select(p => p.Message).ToList();

            // ASSERT
            for (var i = 0; i < messages.Count; i++)
            {
                messages[i].ShouldBe($"diverted foo{i}");
            }
        }
        
        [Fact]
        public void GivenMockedRedirect_ShouldDivert()
        {
            // ARRANGE
            var original = new Foo("hello");
            var proxy = _via.Proxy(original);
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => $"{_via.Relay.Original.Message} world");

            _via.RedirectTo(mock.Object);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe("hello world");
        }

        [Fact]
        public void GivenMultipleAddRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .RedirectTo(new Foo(() => $"DivertR {next.Message} 1"))
                .RedirectTo(new Foo(() => $"here {next.Message} 2"))
                .RedirectTo(new Foo(() => $"again {next.Message} 3"));

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe("DivertR here again hello foo 3 2 1");
        }
        
        [Fact]
        public void GivenMultipleInsertRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("hello foo"));
            var next = _via.Relay.Next;
            _via
                .InsertRedirect(0, new Foo(() => $"DivertR {next.Message} 1"))
                .InsertRedirect(0, new Foo(() => $"here {next.Message} 2"))
                .InsertRedirect(2, new Foo(() => $"again {next.Message} 3"));

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe("here DivertR again hello foo 3 1 2");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithNextAndOriginalRelays_ShouldChain()
        {
            // ARRANGE
            const int numRedirects = 100;
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            for (var i = 0; i < numRedirects; i++)
            {
                var counter = i;
                _via.RedirectTo(new Foo(() => $"{orig.Message} {counter} {next.Message}"));
            }

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            var join = string.Join(" foo ", Enumerable.Range(0, numRedirects).Select(i => $"{i}"));
            message.ShouldBe($"foo {join} foo");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithRecursiveProxy_ShouldDivert()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            var next = _via.Relay.Next;
            var orig = _via.Relay.Original;

            var recursive = new Foo( () =>
            {
                var state = (int[]) _via.Relay.State;
                var decrement = Interlocked.Decrement(ref state[0]);

                if (decrement > 0)
                {
                    return $"[{decrement}{next.Message} {proxy.Message} {orig.Message}{decrement}]";
                }

                return next.Message;
            });

            _via
                .RedirectTo(recursive, new[] {4})
                .RedirectTo(new Foo( () => next.Message.Replace(orig.Message, "bar")));

            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe("[3bar [2bar [1bar bar foo1] foo2] foo3]");
        }
        
        [Fact]
        public void GivenMultipleAddRedirectsWithState_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));

            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Message)
                .Returns(() => 
                    $"{_via.Relay.State} {_via.Relay.Next.Message} {_via.Relay.State}");

            // ACT
            _via
                .RedirectTo(mock.Object, "1")
                .RedirectTo(mock.Object, "2")
                .RedirectTo(mock.Object, "3");
            
            // ASSERT
            proxy.Message.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public void GivenResetBetweenAddRedirects_ShouldOnlyRedirectAfterReset()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.RedirectTo(new Foo(() => $"{_via.Relay.Next.Message} me"));
            _via.Reset();
            _via.RedirectTo(new Foo(() => $"{_via.Relay.Next.Message} again"));

            // ASSERT
            proxy.Message.ShouldBe("hello foo again");
        }
        
        [Fact]
        public void GivenMultipleRedirects_WhenReset_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            _via.RedirectTo(new Foo(() => $"{_via.Relay.Next.Message} me"));
            _via.RedirectTo(new Foo(() => $"{_via.Relay.Next.Message} again"));
            _via.Reset();


            // ASSERT
            proxy.Message.ShouldBe(original.Message);
        }
        
        [Fact(Skip = "Class support removed")]
        public void GivenClassProxy_ShouldDefaultToOriginal()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var via = new Via<Foo>();
            var proxy = via.Proxy(original);
            
            // ACT
            var message = proxy.Message;
            
            // ASSERT
            message.ShouldBe(original.Message);
        }
        
        [Fact(Skip = "Class support removed")]
        public void GivenClassProxy_ShouldDivert()
        {
            // ARRANGE
            var via = new Via<Foo>();
            var proxy = via.Proxy(new Foo("hello foo"));
            var foo = new Foo("hi DivertR");
            via.RedirectTo(foo);

            // ACT
            var message = proxy.Message;

            // ASSERT
            message.ShouldBe(foo.Message);
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
            
            via
                .Redirect(x => x.GenericNumber(Is<string>.Any, Is<int>.Any))
                .To((string s, int i) => via.Relay.Next.GenericNumber(s, i) + " - again");
            
            via.RedirectTo(new Number(x => x * 2));

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
                .Redirect(x => x.GenericNumber(Is<object>.Any, Is<int>.Any))
                .To((object s, int i) => via.Relay.Next.GenericNumber(s, i) + " - again");
            
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
            
            via.RedirectTo(new Number(x => x * 2));

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
    }
}