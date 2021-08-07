using System;
using System.Threading.Tasks;
using DivertR.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DivertR.DemoApp
{
    public interface IFoo
    {
        string Echo(string input);
        Task<string> EchoAsync(string input);
        public string Name { get; set; }
    }

    public class Foo : IFoo
    {
        public string Echo(string input)
        {
            return $"{Name}: {input}";
        }
        
        public async Task<string> EchoAsync(string input)
        {
            await Task.Yield();
            return $"{Name}: {input}";
        }

        public string Name { get; set; } = "original";
    }
    
    class Program
    {
        static async Task Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IFoo, Foo>();
            
            var diverter = new Diverter();
            services.Divert<IFoo>(diverter);
            
            IServiceProvider provider = services.BuildServiceProvider();
            IFoo foo = provider.GetRequiredService<IFoo>();
            foo.Name = "Foo1";

            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello"
            
            IVia<IFoo> fooVia = diverter.Via<IFoo>();
            fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Redirect((string input) => $"{input} DivertR");
  
            Console.WriteLine(foo.Echo("Hello")); // "Hello DivertR"
            
            IFoo foo2 = provider.GetRequiredService<IFoo>();
            foo2.Name = "Foo2";
            Console.WriteLine(foo2.Echo("Hello")); // "Hello DivertR"

            fooVia.Reset();
  
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello"
            
            IFoo next = fooVia.Relay.Next;
            fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Redirect((string input) =>
                {
                    // run test code before
                    // ...

                    // call original instance
                    var message = next.Echo(input);
    
                    // run test code after
                    // ...
    
                    return $"{message} - Redirected";
                });
  
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Redirected"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Redirected"
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.Echo(It.IsAny<string>()))
                .Returns((string input) => $"{next.Echo(input)} - Mocked");

            fooVia
                .To() // Default matches all calls
                .Redirect(mock.Object);
    
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Redirected - Mocked"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Redirected - Mocked"
            
            fooVia
                .Reset()
                .Redirect(mock.Object);
            
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Mocked"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Mocked"
            
            IFoo original = fooVia.Relay.Original;
            fooVia
                .To(x => x.Echo(Is<string>.Any))
                .Redirect((string input) => $"{original.Echo(input)} - Skipped");
  
            Console.WriteLine(foo.Echo("Hello")); // "Foo1: Hello - Skipped"
            Console.WriteLine(foo2.Echo("Hello")); // "Foo2: Hello - Skipped"

            diverter.ResetAll();

            fooVia
                .To(x => x.EchoAsync(Is<string>.Any))
                .Redirect(async (string input) => $"{await next.EchoAsync(input)} - Async");
            
            Console.WriteLine(await foo.EchoAsync("Hello")); // "Foo1: Hello - Async"
            Console.WriteLine(await foo2.EchoAsync("Hello")); // "Foo2: Hello - Async"
        }
    }
}