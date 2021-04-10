using System;
using DivertR.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace DivertR.DemoApp
{
    public interface IFoo
    {
        string GetMessage(string input);
    }

    public class Foo : IFoo
    {
        public string GetMessage(string input)
        {
            return $"{input} original";
        }
    }
    
    class Program
    {
        static void Main(string[] args)
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
            
            diverter.ResetAll();
  
            Console.WriteLine(foo.GetMessage("Hello")); // "Hello original"
            
            IFoo next = fooVia.Relay.Next;
            fooVia
                .Redirect(x => x.GetMessage(Is<string>.Any))
                .To((string input) =>
                {
                    // run test code before
                    // ...

                    // call original instance
                    var original = next.GetMessage(input);
    
                    // run test code after
                    // ...
    
                    return $"Redirected: {original}";
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
        }
    }
}