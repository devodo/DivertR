using System;
using System.Collections.Generic;
using System.Linq;
using DivertR.UnitTests.Model;
using Moq;
using Shouldly;
using Xunit;

namespace DivertR.UnitTests
{
    public class RedirectUpdaterTests
    {
        private readonly IRedirect<IFoo> _redirect = new Redirect<IFoo>();
        
        [Fact]
        public void GivenValueVia_ShouldRedirect()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            var viaMessage = "hi DivertR";
            _redirect.To(x => x.Name).Via(viaMessage);
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe(viaMessage);
        }

        [Fact]
        public void GivenVia_WhenReset_ShouldDefaultToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect.To(x => x.Name).Via("test");

            // ACT
            _redirect.Reset();

            // ASSERT
            proxy.Name.ShouldBe(original.Name);
        }
        
        [Fact]
        public void GivenViaWithCallNextRelay_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect.To(x => x.Name).Via(() => "relay " + (string?) _redirect.Relay.CallNext());

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenViaWithCallNext_ShouldRelay()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect.To(x => x.Name).Via(call => "relay " + call.CallNext());

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenViaWithCallNextRelayWithArgs_ShouldRelayArgs()
        {
            // ARRANGE
            var original = new Foo();
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via(call => call.CallNext(new[] { "relay" }));

            // ACT
            var result = proxy.Echo("test");

            // ASSERT
            result.ShouldBe("original: relay");
        }
        
        [Fact]
        public void GivenProxyWithViaWithRelayToRoot_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Name)
                .Via(() => $"hello {_redirect.Relay.Root.Name}");

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxyWithViaWithRelayToNext_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Name)
                .Via(() => $"hello {_redirect.Relay.Next.Name}");
            
            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("hello foo");
        }
        
        [Fact]
        public void GivenProxyWithViaWithRelayToRootInstance_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            IFoo? originalReference = null;
            _redirect
                .To(x => x.Name)
                .Via(call =>
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
        public void GivenConstantExpressionVia_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            redirect
                .To(x => x.Echo("test"))
                .Via<(string input, __)>(call => $"{call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = redirect.Proxy(new Foo("hello foo"));
            var message = proxy.Echo("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenConstantExpressionVia_WhenCallDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            redirect
                .To(x => x.Echo("test"))
                .Via<(string input, __)>(call => $"via {call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = redirect.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("original: no match");
        }

        [Fact]
        public void GivenVariableExpressionVia_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            var match = "test";
            redirect
                .To(x => x.Echo(match))
                .Via<(string input, __)>(call => $"{call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = redirect.Proxy(new Foo("hello foo"));
            var message = proxy.Echo(match);

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenVariableExpressionCallConstraint_WhenCallDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            var input = new Wrapper<string>("test");
            redirect
                .To(x => x.Echo(input.Item))
                .Via<(string i, __)>(call => $"{call.Relay.Root.Name} {call.Args.i}");

            // ACT
            var proxy = redirect.Proxy(new Foo());
            input.Item = "no match";
            var message = proxy.Echo(input.Item);

            // ASSERT
            message.ShouldBe("original: no match");
        }

        [Fact]
        public void GivenMatchExpressionVia_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            var input = new Wrapper<string>("test");
            redirect
                .To(x => x.Echo(Is<string>.Match(p => p == input.Item)))
                .Via<(string i, __)>(call => $"{call.Relay.Root.Name} {call.Args.i}");

            // ACT
            input.Item = "other";
            var proxy = redirect.Proxy(new Foo("hello foo"));
            var message = proxy.Echo(input.Item);

            // ASSERT
            message.ShouldBe("hello foo other");
        }
        
        [Fact]
        public void GivenMatchExpressionVia_WhenCallDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            redirect
                .To(x => x.Echo(Is<string>.Match(p => p == "test")))
                .Via<(string i, __)>(call => $"via {call.Relay.Root.Name} {call.Args.i}");

            // ACT
            var proxy = redirect.Proxy(new Foo());
            var message = proxy.Echo("no match");

            // ASSERT
            message.ShouldBe("original: no match");
        }
        
        [Fact]
        public void GivenIsAnyExpressionVia_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Relay.Root.Name} {call.Args.input}");

            // ACT
            var proxy = redirect.Proxy(new Foo("hello foo"));
            var message = proxy.Echo("test");

            // ASSERT
            message.ShouldBe("hello foo test");
        }
        
        [Fact]
        public void GivenViaWithArgumentConstraintAsArrayIndexer_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var arguments = new[] { "test" };
            _redirect
                .To(x => x.Echo(arguments[0]))
                .Via(() => "matched");

            // ACT
            var proxy = _redirect.Proxy();
            var result = proxy.Echo("test");

            // ASSERT
            result.ShouldBe("matched");
        }
        
        [Fact]
        public void GivenViaWithArgumentConstraintAsListIndexer_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var arguments = new List<string> { "test" };
            _redirect
                .To(x => x.Echo(arguments[0]))
                .Via(() => "matched");

            // ACT
            var proxy = _redirect.Proxy();
            var result = proxy.Echo("test");

            // ASSERT
            result.ShouldBe("matched");
        }

        [Fact]
        public void GivenSubTypeIsAnyExpressionVia_WhenCallSubTypeNotMatch_ShouldNotVia()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            redirect
                .To(x => x.EchoGeneric<object>(Is<string>.Any))
                .Via<(object input, __)>(call => $"{call.Next.Name} {call.Args.input}");

            // ACT
            var proxy = redirect.Proxy(new Foo());
            var result = proxy.EchoGeneric<object>(1);

            // ASSERT
            result.ShouldBe(1);
        }
        
        [Fact]
        public void GivenSubTypeIsSpecialisedAnyExpressionVia_WhenCallSubTypeMatch_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            
            redirect
                .To(x => x.EchoGeneric<object>(Is<int>.Any))
                .Via<(int input, __)>(call => call.Args.input + 1);

            // ACT
            var proxy = redirect.Proxy(new Foo());
            var result = proxy.EchoGeneric<object>(1);

            // ASSERT
            result.ShouldBe(2);
        }
        
        [Fact]
        public void GivenSubTypeIsSpecialisedMatchExpressionVia_WhenCallSubTypeMatch_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            
            redirect
                .To(x => x.EchoGeneric<object>(Is<int>.Match(m => true)))
                .Via<(int input, __)>(call => call.Args.input + 1);

            // ACT
            var proxy = redirect.Proxy(new Foo());
            var result = proxy.EchoGeneric<object>(1);

            // ASSERT
            result.ShouldBe(2);
        }
        
        [Fact]
        public void GivenSubTypeIsSpecialisedNullableAnyExpressionVia_WhenCallSubTypeMatch_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            
            redirect
                .To(x => x.EchoGeneric<object>(Is<int?>.Any!))
                .Via<(int? input, __)>(call => call.Args.input!.Value + 1);

            // ACT
            var proxy = redirect.Proxy(new Foo());
            var result = proxy.EchoGeneric<object>(1);

            // ASSERT
            result.ShouldBe(2);
        }
        
        [Fact]
        public void GivenSubTypeIsMatchExpressionVia_WhenCallSubTypeNotMatch_ShouldNotVia()
        {
            // ARRANGE
            var redirect = new Redirect<IFoo>();
            redirect
                .To(x => x.EchoGeneric<object>(Is<string>.Match(a => true)))
                .Via<(object input, __)>(call => $"{call.Next.Name} {call.Args.input}");

            // ACT
            var proxy = redirect.Proxy(new Foo());
            var result = proxy.EchoGeneric<object>(1);

            // ASSERT
            result.ShouldBe(1);
        }
        
        [Fact]
        public void GivenPropertyExpressionVia_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            var input = new Wrapper<string>("Hello");

            _redirect.To(x => x.Echo(input.Item))
                .Via<(string i, __)>(call => $"{call.Args.i} - {_redirect.Relay.Next.Name}");
            
            // ACT
            var result = _redirect.Proxy(new Foo("Foo")).Echo(input.Item);

            // ASSERT
            result.ShouldBe("Hello - Foo");
        }
        
        string GetMethodValue()
        {
            return "test";
        }
        
        [Fact]
        public void GivenExpressionMethodBodyVia_WhenCallMatches_ShouldRedirect()
        {
            // ARRANGE
            _redirect.To(x => x.Echo(GetMethodValue()))
                .Via<(string, __)>(_ => "matched");
            
            // ACT
            var result = _redirect.Proxy().Echo(GetMethodValue());

            // ASSERT
            result.ShouldBe("matched");
        }
        
        [Fact]
        public void GivenSetPropertyViaWithNoValueConstraint_WhenValueMatches_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .ToSet(x => x.Name)
                .Via<(string value, __)>(call => call.Relay.Root.Name = $"New {call.Args.value} set");

            // ACT
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            proxy.Name = "test";

            // ASSERT
            proxy.Name.ShouldBe("New test set");
        }

        [Fact]
        public void GivenSetPropertyVia_WhenValueMatches_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .ToSet(x => x.Name, () => Is<string>.Any)
                .Via<(string value, __)>(call => call.Relay.Root.Name = $"New {call.Args.value} set");

            // ACT
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            proxy.Name = "test";

            // ASSERT
            proxy.Name.ShouldBe("New test set");
        }
        
        [Fact]
        public void GivenRedirect_WhenToSetBuilderWithIsRefAnyConstraint_ThenThrowsException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.ToSet(x => x.Name, () => IsRef<string>.Any);

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenToSetBuilderWithIsRefMatchConstraint_ThenThrowsException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.ToSet(x => x.Name, () => IsRef<string>.Match(m => true).Value);

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenSetPropertyVia_WhenValueDoesNotMatch_ShouldDefaultToRoot()
        {
            // ARRANGE
            _redirect
                .ToSet(x => x.Name, () => Is<string>.Match(s => s == "test"))
                .Via<(string value, __)>(call => call.Relay.Root.Name = $"New {call.Args.value} set");

            // ACT
            var proxy = _redirect.Proxy(new Foo("hello foo"));
            proxy.Name = "no match";

            // ASSERT
            proxy.Name.ShouldBe("no match");
        }
        
        [Fact]
        public void GivenGenericInputVia_WhenGenericTypeMatch_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<string>.Any))
                .Via<(string i, __)>(call => $"{call.Args.i} - {_redirect.Relay.Next.Name}");

            // ACT
            var proxy = _redirect.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello - foo");
        }
        
        [Fact]
        public void GivenGenericInputVia_WhenAssignableGenericType_ShouldNotVia()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<object>.Any))
                .Via<(object i, __)>(call => $"{call.Args.i} - {_redirect.Relay.Next.Name}");

            // ACT
            var proxy = _redirect.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }

        [Fact]
        public void GivenGenericInputVia_WhenTypeMatchConstraintFails_ShouldNotVia()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<object>.Match(m => m.GetType() == typeof(object))))
                .Via<(object i, __)>(call => $"{call.Args.i} - {_redirect.Relay.Next.Name}");

            // ACT
            var proxy = _redirect.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric<object>("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenGenericInputVia_WhenNotAssignableGenericType_ShouldNotVia()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<string>.Any))
                .Via<(string i, __)>(call => $"{call.Args.i} - {_redirect.Relay.Next.Name}");

            // ACT
            var proxy = _redirect.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric<object>("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenGenericInputVia_WhenMethodDoesNotMatch_ShouldNotVia()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGenericAlt(Is<string>.Any))
                .Via<(string i, __)>(call => $"{call.Args.i} - {_redirect.Relay.Next.Name}");

            // ACT
            var proxy = _redirect.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenGenericInputVia_WhenGenericAssignable_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<string>.Any))
                .Via<(string i, __)>(call => $"{call.Args.i} - {_redirect.Relay.Next.Name}");
            
            // ACT
            var proxy = _redirect.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello - foo");
        }
        
        [Fact]
        public void GivenGenericInputVia_WhenGenericNotAssignable_ShouldDefaultToRoot()
        {
            // ARRANGE
            _redirect
                .To(x => x.EchoGeneric(Is<int>.Any))
                .Via<(int i, __)>(call => call.Args.i + 10);
            
            // ACT
            var proxy = _redirect.Proxy(new Foo("foo"));
            var result = proxy.EchoGeneric("Hello");

            // ASSERT
            result.ShouldBe("Hello");
        }
        
        [Fact]
        public void GivenViaWithArrayParameter_WhenIsAny_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            
            redirect.Retarget(new Number(x => x * 2));
            redirect
                .To(x => x.ArrayNumber(Is<int[]>.Any))
                .Via<(int[] inputs, __)>(call =>
                {
                    call.Relay.Next.ArrayNumber(call.Args.inputs);
                    
                    for (var i = 0; i < call.Args.inputs.Length; i++)
                    {
                        call.Args.inputs[i] += 1;
                    }
                });
            
            // ACT
            var input = new[] { 1, 5 };
            var proxy = redirect.Proxy(new Number());
            proxy.ArrayNumber(input);

            // ASSERT
            input[0].ShouldBe(3);
            input[1].ShouldBe(11);
        }

        [Fact]
        public void GivenViaReturnTypeSameAsTargetType_ShouldRedirect()
        {
            // ARRANGE
            IFoo getFoo = new Foo();
            _redirect.To(x => x.GetFoo())
                .Via(getFoo);
            
            // ACT
            var result = _redirect.Proxy().GetFoo();

            // ASSERT
            result.ShouldBeSameAs(getFoo);
        }
        
        [Fact]
        public void GivenMultipleViasWithOrderWeights_ShouldOrderVias()
        {
            // ARRANGE
            _redirect
                .To(x => x.Name)
                .Via(() => $"1 {_redirect.Relay.Next.Name} 1", options => options.OrderWeight(30))
                .Via(() => $"2 {_redirect.Relay.Next.Name} 2", options => options.OrderWeight(20))
                .Via(() => $"3 {_redirect.Relay.Next.Name} 3", options => options.OrderWeight(10));
            
            // ACT
            var result = _redirect.Proxy(new Foo("hello")).Name;

            // ASSERT
            result.ShouldBe("1 2 3 hello 3 2 1");
        }
        
        [Fact]
        public void GivenMultipleOrderedVias_ShouldChain()
        {
            // ARRANGE
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            string WriteMessage(int num)
            {
                return $"{num} {_redirect.Relay.Next.Name} {num}";
            }
            
            _redirect
                .To(x => x.Name)
                .Via(() => WriteMessage(1), options => options.OrderWeight(30))
                .Via(() => WriteMessage(2), options => options.OrderWeight(20))
                .Via(() => WriteMessage(3), options => options.OrderWeight(10));

            // ACT
            var name = proxy.Name;

            // ASSERT
            name.ShouldBe("1 2 3 foo 3 2 1");
        }
        
        [Fact]
        public void GivenVia_WithCallRootRelay_ShouldRelayToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Name)
                .Via(() => "relay " + (string?) _redirect.Relay.CallRoot());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenVia_WithCallRoot_ShouldRelayToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Name)
                .Via(call => "relay " + call.CallRoot());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenVia_WithCallNext_ShouldRelayToRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Name)
                .Via(() => "relay " + (string?) _redirect.Relay.CallNext());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenVia_WithCallNextRelay_ShouldCallNextVia()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect.To(x => x.Name).Via("test");
            _redirect
                .To(x => x.Name)
                .Via(() => (string) _redirect.Relay.CallNext()!);

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("test");
        }
        
        [Fact]
        public void GivenVia_WithCallRootRelayWithMethod_ShouldCallRoot()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Args.input}-test");
            _redirect
                .To(x => x.Echo("here"))
                .Via(call => (string) call.CallRoot(call.CallInfo.Method, new object[] { "alter" })!);

            // ACT
            var result = proxy.Echo("here");

            // ASSERT
            result.ShouldBe("foo: alter");
        }
        
        [Fact]
        public void GivenVia_WithCallRootRelayWithArgs_ShouldRelayArgs()
        {
            // ARRANGE
            var original = new Foo();
            var proxy = _redirect.Proxy(original);
            _redirect.To(x => x.Echo(Is<string>.Any)).Via(call => call.CallRoot(new[] { "relay" }));

            // ACT
            var result = proxy.Echo("test");

            // ASSERT
            result.ShouldBe("original: relay");
        }
        
        [Fact]
        public void GivenVia_WithCallNextRelayWithMethod_ShouldCallNextVia()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Args.input}-test");
            _redirect
                .To(x => x.Echo("here"))
                .Via(call => (string?) call.CallNext(call.CallInfo.Method, new CallArguments(new object[] { "alter" }))!);

            // ACT
            var result = proxy.Echo("here");

            // ASSERT
            result.ShouldBe("alter-test");
        }

        [Fact]
        public void GivenFuncRelayVia_ShouldRedirect()
        {
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Next.Echo(call.Args.input)} diverted");
            
            // ACT
            var result = proxy.Echo("test");
            
            // ASSERT
            result.ShouldBe("foo: test diverted");
        }
        
        [Fact]
        public void GivenRelayFuncArgsVia_ShouldRedirect()
        {
            var proxy = _redirect.Proxy(new Foo("foo"));
            
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>((call, args) => $"{call.Next.Echo(args.input)} diverted");
            
            // ACT
            var result = proxy.Echo("test");
            
            // ASSERT
            result.ShouldBe("foo: test diverted");
        }
        
        [Fact]
        public void GivenViaWithOptions_ShouldRedirect()
        {
            var proxy = _redirect.Proxy(new Foo("foo"));

            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call =>
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
        public void GivenVia_WithReturnMatch_ShouldRedirect()
        {
            // ARRANGE
            var original = new Foo("foo");
            var proxy = _redirect.Proxy(original);
            _redirect
                .To(x => Is<string>.Return)
                .Via(call => "relay " + call.CallRoot());

            // ACT
            var result = proxy.Name;

            // ASSERT
            result.ShouldBe("relay " + original.Name);
        }
        
        [Fact]
        public void GivenRedirect_WhenIsAnyForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE
            
            // ACT
            var testAction = () => _redirect.To(x => Is<string>.Any);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenIsMatchForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE
            
            // ACT
            var testAction = () => _redirect.To(x => Is<string>.Match(m => true));

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenIsRefAnyForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE
            
            // ACT
            var testAction = () => _redirect.To(x => IsRef<string>.Any);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenIsRefMatchValueForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE
            
            // ACT
            var testAction = () => _redirect.To(x => IsRef<string>.Match(m => true).Value);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenIsRefMatchForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE
            
            // ACT
            var testAction = () => _redirect.To(x => IsRef<string>.Match(m => true));

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenConstantForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.To(x => "test");

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenMethodForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE
            var arguments = new List<string> { "test" };

            // ACT
            var testAction = () => _redirect.To(x => arguments[0]);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenPropertyForReturnMatch_ThenThrowsArgumentException()
        {
            // ARRANGE
            var foo = new Foo();
            
            // ACT
            var testAction = () => _redirect.To(x => foo.Name);

            // ASSERT
            testAction.ShouldThrow<ArgumentException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenAddViaWithReturnMatchParameter_ThenThrowsException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.To(x => x.Echo(Is<string>.Return));

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenAddViaWithIsRefAnyParameter_ThenThrowsException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.To(x => x.Echo(IsRef<string>.Any));

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenRedirect_WhenAddViaWithIsRefMatchParameter_ThenThrowsException()
        {
            // ARRANGE

            // ACT
            var testAction = () => _redirect.To(x => x.Echo(IsRef<string>.Match(m => true).Value));

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }

        [Fact]
        public void GivenActionRelayVia_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            redirect
                .To(x => x.GetNumber(Is<int>.Any))
                .Via<(int input, __)>(call => call.Next.GetNumber(call.Args.input) + 5);

            // ACT
            var proxy = redirect.Proxy(new Number(x => x * 2));
            var result = proxy.GetNumber(10);

            // ASSERT
            result.ShouldBe(25);
        }
        
        [Fact]
        public void GivenActionRelayArgsVia_ShouldRedirect()
        {
            // ARRANGE
            var redirect = new Redirect<INumber>();
            redirect
                .To(x => x.GetNumber(Is<int>.Any))
                .Via<(int input, __)>((call, args) => call.Next.GetNumber(args.input) + 5);

            // ACT
            var proxy = redirect.Proxy(new Number(x => x * 2));
            var result = proxy.GetNumber(10);

            // ASSERT
            result.ShouldBe(25);
        }
        
        [Fact]
        public void GivenInsertedVias_ShouldReturnRedirectPlan()
        {
            // ARRANGE
            const int Num = 20;
            var redirectBuilder = _redirect.To(x => x.Name);
            for (var i = 0; i < Num; i++)
            {
                redirectBuilder.Via("test" + i);
            }

            // ACT
            var redirectPlan = _redirect.RedirectRepository.RedirectPlan;

            // ASSERT
            redirectPlan.IsStrictMode.ShouldBe(false);
            redirectPlan.Vias.Count.ShouldBe(Num);
        }
        
        [Fact]
        public void GivenAddedCallConstraint_ShouldApply()
        {
            // ARRANGE
            _redirect
                .To(x => x.Echo(Is<string>.Any))
                .Filter(new CallConstraint<IFoo>(call => (string) call.Arguments[0] != "ignore"))
                .Via<(string input, __)>(call => call.CallNext(new[] { $"{call.Args.input} redirected" }));

            var proxy = _redirect.Proxy(new Foo());

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
            Action testAction = () => _redirect.To(x => x.Echo(It.IsAny<string>()));

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenMoqIsArgumentSyntax_ShouldThrowException()
        {
            // ARRANGE

            // ACT
            Action testAction = () => _redirect.To(x => x.Echo(It.Is<string>(m => true)));

            // ASSERT
            testAction.ShouldThrow<DiverterValidationException>();
        }
        
        [Fact]
        public void GivenConstraintVia_WhenConstraintMatches_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(new CallConstraint<IFoo>(call => call.Method.ReturnType.IsAssignableFrom(typeof(string))))
                .Via(() => "redirected")
                .Via(call => $"{call.CallNext()} call {call.Args.LastOrDefault()}".Trim())
                .Via((call, args) => $"{call.CallNext()} args {args.LastOrDefault()}".Trim());
            
            var proxy = _redirect.Proxy();

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
        public void GivenConstraintInstanceVia_WhenConstraintMatches_ShouldRedirect()
        {
            // ARRANGE
            _redirect
                .To(new CallConstraint<IFoo>(call => call.Method.ReturnType.IsAssignableFrom(typeof(string))))
                .Via("redirected");

            var proxy = _redirect.Proxy();

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