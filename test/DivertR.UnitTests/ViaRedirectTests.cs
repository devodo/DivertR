using System;
using System.Linq;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class ViaRedirectTests
    {
        private readonly IVia<IFoo> _via = new Via<IFoo>();

        [Fact]
        public void GivenNoRedirects_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("hello foo");
            var proxy = _via.Proxy(original);

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenValidRootObjectType_WhenCreateProxyObject_ShouldCreateProxy()
        {
            // ARRANGE
            var original = new Foo();

            // ACT
            var proxy = (IFoo) ((IVia) _via).Proxy(original);

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenProxyWithNullRoot_WhenProxyMemberCalled_ShouldThrowException()
        {
            // ARRANGE
            var proxy = _via.Proxy(null);

            // ACT
            Func<object> testAction = () => proxy.Name;

            // ASSERT
            testAction.ShouldThrow<DiverterNullRootException>();
        }
        
        [Fact]
        public void GivenInvalidRootObjectType_WhenCreateProxyObject_ShouldThrowArgumentException()
        {
            // ARRANGE
            var invalidOriginal = new object();

            // ACT
            Func<object> testAction = () => _via.Proxy(invalidOriginal);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GivenValueRedirect_ShouldRedirect()
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
        public void GivenRedirect_WhenReset_ShouldDefaultToRoot()
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
        public void GivenRedirectWithCallNextRelay_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.To(x => x.Name).Redirect(() => "relay " + (string?) _via.Relay.CallNext());

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenRedirectWithCallNext_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.To(x => x.Name).Redirect(call => "relay " + call.CallNext());

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenRedirectWithCallNextRelayWithArgs_ShouldRelayArgs()
        {
            // ARRANGE
            var original = new Foo();
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect(call => call.CallNext(new[] { "relay" }));

            // ACT
            var result = proxy.Echo("test");

            // ASSERT
            result.ShouldBe("original: relay");
        }
        
        [Fact]
        public void GivenProxyWithRedirectWithRelayToRoot_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(() => $"hello {_via.Relay.Root.Name}");

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
                .Redirect(() => $"hello {_via.Relay.Next.Name}");
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxyWithRedirectWithRelayToRootInstance_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            IFoo? originalReference = null;
            _via
                .To(x => x.Name)
                .Redirect(call =>
                {
                    originalReference = call.CallInfo.Root;
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
                .Redirect<(string input, __)>(call => $"{call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenConstantExpressionRedirect_WhenCallDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.Echo("test"))
                .Redirect<(string input, __)>(call => $"redirect {call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("original: no match");
        }

        [Fact]
        public void GivenVariableExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var match = "test";
            via
                .To(x => x.Echo(match))
                .Redirect<(string input, __)>(call => $"{call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo(match);

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenVariableExpressionCallConstraint_WhenCallDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var input = new Wrapper<string>("test");
            via
                .To(x => x.Echo(input.Item))
                .Redirect<(string i, __)>(call => $"{call.Relay.Root.Name} {call.Args.i}");

            // ACT
            var proxy = via.Proxy(new Foo());
            input.Item = "no match";
            var message = proxy.Echo(input.Item);

            // ASSERT
            message.ShouldBe("original: no match");
        }

        [Fact]
        public void GivenMatchExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            var input = new Wrapper<string>("test");
            via
                .To(x => x.Echo(Is<string>.Match(p => p == input.Item)))
                .Redirect<(string i, __)>(call => $"{call.Relay.Root.Name} {call.Args.i}");

            // ACT
            input.Item = "other";
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo(input.Item);

            // ASSERT
            message.ShouldBe("hello foo other");
        }
        
        [Fact]
        public void GivenMatchExpressionRedirect_WhenCallDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.Echo(Is<string>.Match(p => p == "test")))
                .Redirect<(string i, __)>(call => $"redirect {call.Relay.Root.Name} {call.Args.i}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("original: no match");
        }
        
        [Fact]
        public void GivenIsAnyExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>(call => $"{call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = via.Proxy(new Foo("hello foo"));
            var message = proxy.Echo("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }

        [Fact]
        public void GivenSubTypeIsAnyExpressionRedirect_WhenCallSubTypeNotMatch_ShouldNotRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.EchoGeneric<object>(Is<string>.Any))
                .Redirect<(object input, __)>(call => $"{call.Next.Name} {call.Args.input}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var result = proxy.EchoGeneric<object>(1);

            // ASSERT
            result.ShouldBe(1);
        }
        
        [Fact]
        public void GivenSubTypeIsMatchExpressionRedirect_WhenCallSubTypeNotMatch_ShouldNotRedirect()
        {
            // ARRANGE
            var via = new Via<IFoo>();
            via
                .To(x => x.EchoGeneric<object>(Is<string>.Match(a => true)))
                .Redirect<(object input, __)>(call => $"{call.Next.Name} {call.Args.input}");

            // ACT
            var proxy = via.Proxy(new Foo());
            var result = proxy.EchoGeneric<object>(1);

            // ASSERT
            result.ShouldBe(1);
        }
        
        [Fact]
        public void GivenPropertyExpressionRedirect_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var input = new Wrapper<string>("Hello");

            _via.To(x => x.Echo(input.Item))
                .Redirect<(string i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");
            
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
                .Redirect<(string, __)>(_ => "matched");
            
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
                .Redirect<(string value, __)>(call => call.Relay.Root.Name = $"New {call.Args.value} set");

            // ACT
            var proxy = _via.Proxy(new Foo("hello foo"));
            proxy.Name = "test";

            // ASSERT
            proxy.Name.ShouldBe("New test set");
        }
        
        [Fact]
        public void GivenSetPropertyRedirect_WhenValueDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            _via
                .ToSet(x => x.Name, () => Is<string>.Match(s => s == "test"))
                .Redirect<(string value, __)>(call => call.Relay.Root.Name = $"New {call.Args.value} set");

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
                .Redirect<(string i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");

            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello - foo");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenAssignableGenericType_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<object>.Any))
                .Redirect<(object i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");

            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello - foo");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenAssignableGenericTypeWithAdditionalConstraint_ShouldNotRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<object>.Any))
                .AddConstraint(new MatchCallConstraint<IFoo>(callInfo => callInfo.Method.GetGenericArguments()[0] == typeof(object)))
                .Redirect<(object i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");

            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenAssignableGenericTypeWithMatchConstraint_ShouldNotRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<object>.Match(m => m.GetType() == typeof(object))))
                .Redirect<(object i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");

            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenNotAssignableGenericType_ShouldNotRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<string>.Any))
                .Redirect<(string i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");

            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric<object>("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenMethodDoesNotMatch_ShouldNotRedirect()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGenericAlt(Is<string>.Any))
                .Redirect<(string i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");

            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericAssignable_ShouldDivert()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<string>.Any))
                .Redirect<(string i, __)>(call => $"{call.Args.i} - {_via.Relay.Next.Name}");
            
            // ACT
            var proxy = _via.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello - foo");
        }
        
        [Fact]
        public void GivenGenericInputRedirect_WhenGenericNotAssignable_ShouldDefaultToRoot()
        {
            // ARRANGE
            _via
                .To(x => x.EchoGeneric(Is<int>.Any))
                .Redirect<(int i, __)>(call => call.Args.i + 10);
            
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
                .Redirect<(int[] inputs, __)>(call =>
                {
                    call.Relay.Next.ArrayNumber(call.Args.inputs);
                    
                    for (var i = 0; i < call.Args.inputs.Length; i++)
                    {
                        call.Args.inputs[i] += 1;
                    }
                });
            
            // ACT
            var input = new[] { 1, 5 };
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
                .To(x => x.Name)
                .Redirect(() => $"1 {_via.Relay.Next.Name} 1", options => options.OrderWeight(30))
                .Redirect(() => $"2 {_via.Relay.Next.Name} 2", options => options.OrderWeight(20))
                .Redirect(() => $"3 {_via.Relay.Next.Name} 3", options => options.OrderWeight(10));
            
            // ACT
            var result = _via.Proxy(new Foo("hello")).Name;

            // ASSERT
            result.ShouldBe("1 2 3 hello 3 2 1");
        }
        
        [Fact]
        public void GivenMultipleOrderedRedirects_ShouldChain()
        {
            // ARRANGE
            var proxy = _via.Proxy(new Foo("foo"));
            
            string WriteMessage(int num)
            {
                return $"{num} {_via.Relay.Next.Name} {num}";
            }
            
            _via
                .To(x => x.Name)
                .Redirect(() => WriteMessage(1), options => options.OrderWeight(30))
                .Redirect(() => WriteMessage(2), options => options.OrderWeight(20))
                .Redirect(() => WriteMessage(3), options => options.OrderWeight(10));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public void GivenRedirect_WithCallRootRelay_ShouldRelayToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(() => "relay " + (string?) _via.Relay.CallRoot());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenRedirect_WithCallRoot_ShouldRelayToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(call => "relay " + call.CallRoot());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenRedirect_WithCallNext_ShouldRelayToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Name)
                .Redirect(() => "relay " + (string?) _via.Relay.CallNext());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenRedirect_WithCallNextRelay_ShouldCallNextRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via.To(x => x.Name).Redirect("test");
            _via
                .To(x => x.Name)
                .Redirect(() => (string) _via.Relay.CallNext()!);

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("test");
        }
        
        [Fact]
        public void GivenRedirect_WithCallRootRelayWithMethod_ShouldCallRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>(call => $"{call.Args.input}-test");
            _via
                .To(x => x.Echo("here"))
                .Redirect(call => (string) call.CallRoot(call.CallInfo.Method, new object[] { "alter" })!);

            // ACT
            var result = proxy.Echo("here");

            // ASSERT
            result.ShouldBe("foo: alter");
        }
        
        [Fact]
        public void GivenRedirect_WithCallRootRelayWithArgs_ShouldRelayArgs()
        {
            // ARRANGE
            var original = new Foo();
            var proxy = _via.Proxy(original);
            _via.To(x => x.Echo(Is<string>.Any)).Redirect(call => call.CallRoot(new[] { "relay" }));

            // ACT
            var result = proxy.Echo("test");

            // ASSERT
            result.ShouldBe("original: relay");
        }
        
        [Fact]
        public void GivenRedirect_WithCallNextRelayWithMethod_ShouldCallNextRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>(call => $"{call.Args.input}-test");
            _via
                .To(x => x.Echo("here"))
                .Redirect(call => (string?) call.CallNext(call.CallInfo.Method, new CallArguments(new object[] { "alter" }))!);

            // ACT
            var result = proxy.Echo("here");

            // ASSERT
            result.ShouldBe("alter-test");
        }

        [Fact]
        public void GivenFuncRelayRedirect_ShouldRedirect()
        {
            var proxy = _via.Proxy(new Foo("foo"));
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>(call => $"{call.Next.Echo(call.Args.input)} diverted");
            
            // ACT
            var result = proxy.Echo("test");
            
            // ASSERT
            result.ShouldBe("foo: test diverted");
        }
        
        [Fact]
        public void GivenRelayFuncArgsRedirect_ShouldRedirect()
        {
            var proxy = _via.Proxy(new Foo("foo"));
            
            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>((call, args) => $"{call.Next.Echo(args.input)} diverted");
            
            // ACT
            var result = proxy.Echo("test");
            
            // ASSERT
            result.ShouldBe("foo: test diverted");
        }
        
        [Fact]
        public void GivenRedirectWithOptions_ShouldRedirect()
        {
            var proxy = _via.Proxy(new Foo("foo"));

            _via
                .To(x => x.Echo(Is<string>.Any))
                .Redirect<(string input, __)>(call =>
                {
                    var echo = call.Next.Echo(call.Args.input);
                    return $"{echo} diverted";
                }, options => options.Repeat(10));

            // ACT
            var result = proxy.Echo("test");
            
            // ASSERT
            result.ShouldBe("foo: test diverted");
        }
        
        [Fact]
        public void GivenRedirect_WithReturnMatch_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _via.Proxy(original);
            _via
                .To(x => Is<string>.Return)
                .Redirect(call => "relay " + call.CallRoot());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }

        [Fact]
        public void GivenActionRelayRedirect_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            via
                .To(x => x.GetNumber(Is<int>.Any))
                .Redirect<(int input, __)>(call => call.Next.GetNumber(call.Args.input) + 5);

            // ACT
            var proxy = via.Proxy(new Number(x => x * 2));
            var result = proxy.GetNumber(10);

            // ASSERT
            result.ShouldBe(25);
        }
        
        [Fact]
        public void GivenActionRelayArgsRedirect_ShouldRedirect()
        {
            // ARRANGE
            var via = new Via<INumber>();
            via
                .To(x => x.GetNumber(Is<int>.Any))
                .Redirect<(int input, __)>((call, args) => call.Next.GetNumber(args.input) + 5);

            // ACT
            var proxy = via.Proxy(new Number(x => x * 2));
            var result = proxy.GetNumber(10);

            // ASSERT
            result.ShouldBe(25);
        }
        
        [Fact]
        public void GivenInsertedRedirects_ShouldReturnRedirectPlan()
        {
            // ARRANGE
            const int Num = 20;
            var viaBuilder = _via.To(x => x.Name);
            for (var i = 0; i < Num; i++)
            {
                viaBuilder.Redirect("test" + i);
            }

            // ACT
            var redirectPlan = _via.RedirectRepository.RedirectPlan;

            // ASSERT
            redirectPlan.IsStrictMode.ShouldBe(false);
            redirectPlan.Redirects.Count.ShouldBe(Num);
        }
        
        [Fact]
        public void GivenAddedCallConstraint_ShouldApply()
        {
            // ARRANGE
            _via
                .To(x => x.Echo(Is<string>.Any))
                .AddConstraint(new CallConstraint<IFoo>(call => (string) call.Arguments[0] != "ignore"))
                .Redirect<(string input, __)>(call => call.CallNext(new[] { $"{call.Args.input} redirected" }));

            var proxy = _via.Proxy(new Foo());

            // ACT
            var result1 = proxy.Echo("test");
            var result2 = proxy.Echo("ignore");

            // ASSERT
            result1.ShouldBe("original: test redirected");
            result2.ShouldBe("original: ignore");
        }
        
        [Fact]
        public void GivenMoqAnyArgumentSyntax_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            Action testAction = () => _via.To(x => x.Echo(It.IsAny<string>()));

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenMoqIsArgumentSyntax_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            Action testAction = () => _via.To(x => x.Echo(It.Is<string>(m => true)));

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenConstraintRedirect_WhenConstraintMatches_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(new CallConstraint<IFoo>(call => call.Method.ReturnType.IsAssignableFrom(typeof(string))))
                .Redirect(() => "redirected")
                .Redirect(call => $"{call.CallNext()} call {call.Args.LastOrDefault()}".Trim())
                .Redirect((call, args) => $"{call.CallNext()} args {args.LastOrDefault()}".Trim());
            
            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;
            var objectReturn = proxy.EchoGeneric<object>("hello");
            var intReturn = proxy.EchoGeneric(1);

            // ASSERT
            result.ShouldBe("redirected call hello args hello");
            name.ShouldBe("redirected call args");
            objectReturn.ShouldBe("redirected call hello args hello");
            intReturn.ShouldBe(0);
        }
        
        [Fact]
        public void GivenConstraintInstanceRedirect_WhenConstraintMatches_ShouldRedirect()
        {
            // ARRANGE
            _via
                .To(new CallConstraint<IFoo>(call => call.Method.ReturnType.IsAssignableFrom(typeof(string))))
                .Redirect("redirected");

            var proxy = _via.Proxy();

            // ACT
            var result = proxy.EchoGeneric("hello");
            var name = proxy.Name;
            var objectReturn = proxy.EchoGeneric<object>("hello");
            var intReturn = proxy.EchoGeneric(1);

            // ASSERT
            result.ShouldBe("redirected");
            name.ShouldBe("redirected");
            objectReturn.ShouldBe("redirected");
            intReturn.ShouldBe(0);
        }
    }
}