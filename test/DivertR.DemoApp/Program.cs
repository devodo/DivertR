using System;
using System.Threading.Tasks;
using DivertR.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DivertR.DemoApp
{
    public interface IFoo
    {
        string Name { get; set; }
        string Echo(string input);
        Task<string> EchoAsync(string input);
        T Echo<T>(T input);
    }

    public interface IBar
    {
        IFoo Foo { get; }
    }

    public class Foo : IFoo
    {
        public string Name { get; set; } = "Foo";
    
        public string Echo(string input)
        {
            return $"{Name}: {input}";
        }
    
        public async Task<string> EchoAsync(string input)
        {
            await Task.Yield();
            return $"{Name}: {input}";
        }

        public T Echo<T>(T input)
        {
            return input;
        }
    }

    public class Bar : IBar
    {
        public Bar(IFoo foo)
        {
            Foo = foo;
        }
        
        public IFoo Foo { get; }
    }
    
    class Program
    {
        static async Task Main()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IFoo, Foo>();
            
            var diverter = new DiverterBuilder().Register<IFoo>().Create();
            services.Divert(diverter);
            
            IServiceProvider provider = services.BuildServiceProvider();
            IFoo foo = provider.GetRequiredService<IFoo>();
            foo.Name = "Foo1";

            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello"
            
            IRedirect<IFoo> fooRedirect = diverter.Redirect<IFoo>();
            fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Args.input} DivertR");
  
            Console.WriteLine(foo.Echo("Hello")); // "Hello DivertR"
            
            IFoo foo2 = provider.GetRequiredService<IFoo>();
            foo2.Name = "Foo2";
            Console.WriteLine(foo2.Echo("Hello")); // "Hello DivertR"

            fooRedirect.Reset();
  
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello"
            
            fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call =>
                {
                    // run test code before
                    // ...

                    // call original instance
                    IFoo root = call.Relay.Root;
                    var message = root.Echo(call.Args.input);
    
                    // run test code after
                    // ...
    
                    return $"{message} - Redirected";
                });
  
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Redirected"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Redirected"
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Echo(It.IsAny<string>()))
                .Returns((string input) => $"{fooRedirect.Relay.Next.Echo(input)} - Mocked");

            fooRedirect
                .To() // Default matches all calls
                .Retarget(mock.Object);
    
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Redirected - Mocked"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Redirected - Mocked"
            
            fooRedirect
                .Reset()
                .Retarget(mock.Object);
            
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Mocked"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Mocked"
            
            fooRedirect
                .To(x => x.Echo(Is<string>.Any))
                .Via<(string input, __)>(call => $"{call.Root.Echo(call.Args.input)} - Skipped");
  
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Skipped"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Skipped"

            diverter.ResetAll();

            fooRedirect
                .To(x => x.EchoAsync(Is<string>.Any))
                .Via<(string input, __)>(async call => $"{await call.Next.EchoAsync(call.Args.input)} - Async");
            
            Console.WriteLine(await foo.EchoAsync("Hello")); // "Foo1: Hello - Async"
            Console.WriteLine(await foo2.EchoAsync("Hello")); // "Foo2: Hello - Async"

            fooRedirect
                .To(x => x.Echo(Is<int>.Any))
                .Via(call => call.CallNext() + 10);

            fooRedirect
                .To(x => x.Echo(Is<Task<int>>.Any))
                .Via(async call => await call.CallNext() + 100);
            
            Console.WriteLine(foo.Echo(5)); // 15
            Console.WriteLine(await foo.Echo(Task.FromResult(50))); // 150
        }
    }
}