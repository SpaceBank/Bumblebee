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
            Console.WriteLine("Enter:\n\"1\" to transform source json\n\"2\" to process destination json\n\"3\" to add new parameter in all processes");
            Variables.Step = Console.ReadLine();

            if (Variables.Step == "1")
            {
                Console.WriteLine("Enter source json path");
                Variables.SourceJsonPath = Console.ReadLine();
                Console.WriteLine("Enter source state diagram id");
                Variables.SourceStateDiagramId = Console.ReadLine();
                Console.WriteLine("Enter destination state diagram id");
                Variables.DestinationStateDiagramId = Console.ReadLine();
            }

            var services = new ServiceCollection();
            services.AddTransient<Bumblebee>();
            services.AddNodeServices();

            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<Bumblebee>().Run(serviceProvider.GetService<INodeServices>());
        }
    }
}