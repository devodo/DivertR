using System;
using System.Threading.Tasks;
using DivertR.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DivertR.DemoApp
{
    public interface IFoo
    {
        string GetMessage(string input);
        Task<string> GetMessageAsync(string input);
        public string Name { get; set; }
    }

    public class Foo : IFoo
    {
        public string GetMessage(string input)
        {
            return $"{input} {Name}";
        }
        
        public async Task<string> GetMessageAsync(string input)
        {
            await Task.Yield();
            return $"{input} async";
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

            Console.WriteLine(foo.GetMessage("Hello")); // "Hello original"
            
            IVia<IFoo> fooVia = diverter.Via<IFoo>();
            fooVia
                .Redirect(x => x.GetMessage(Is<string>.Any))
                .To((string input) => $"{input} DivertR");
  
            Console.WriteLine(foo.GetMessage("Hello")); // "Hello DivertR"
            
            IFoo foo2 = provider.GetRequiredService<IFoo>();
            Console.WriteLine(foo2.GetMessage("Foo2")); // "Foo2 DivertR"

            fooVia.Reset();
  
            Console.WriteLine(foo.GetMessage("Hello")); // "Hello original"
            Console.WriteLine(foo2.GetMessage("Foo2")); // "Foo2 original"
            
            IFoo next = fooVia.Relay.Next;
            fooVia
                .Redirect(x => x.GetMessage(Is<string>.Any))
                .To((string input) =>
                {
                    // run test code before
                    // ...

                    // call original instance
                    var message = next.GetMessage(input);
    
                    // run test code after
                    // ...
    
                    return $"Redirected: {message}";
                });
  
            Console.WriteLine(foo.GetMessage("Hello")); // "Redirected: Hello original"
            
            var mock = new Mock<IFoo>();
            mock
                .Setup(x => x.GetMessage(It.IsAny<string>()))
                .Returns((string input) => $"Mocked: {next.GetMessage(input)}");

            fooVia
                .Redirect() // Default matches all calls
                .To(mock.Object);
    
            Console.WriteLine(foo.GetMessage("Hello")); // Mocked: Redirected: Hello original
            
            IFoo original = fooVia.Relay.Original;
            fooVia
                .Redirect(x => x.GetMessage(Is<string>.Any))
                .To((string input) => $"Skipped: {original.GetMessage(input)}");
  
            Console.WriteLine(foo.GetMessage("Hello")); // "Skipped: Hello original"

            fooVia
                .Reset()
                .Redirect(x => x.GetMessageAsync(Is<string>.Any))
                .To(async (string input) => $"Redirected: {await next.GetMessageAsync(input)}");
            
            Console.WriteLine(await foo.GetMessageAsync("Hello")); // "Redirected: Hello async"
        }
    }
}