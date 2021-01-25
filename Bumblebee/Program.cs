using Microsoft.AspNetCore.NodeServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Bumblebee
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Enter \"1\" to transform source json or \"2\" to process destination json:");
            Variables.Step = Console.ReadLine();

            var services = new ServiceCollection();
            services.AddTransient<Bumblebee>();
            services.AddNodeServices();

            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<Bumblebee>().Run(serviceProvider.GetService<INodeServices>());
        }
    }
}