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
            Console.WriteLine("Enter \"1\" for export from source or \"2\" for export from destination");
            Variables.Step = Console.ReadLine();

            if (Variables.Step == "1")
            {
                Console.WriteLine("Enter source corezoid url");
                Variables.SourceCorezoidUrl = Console.ReadLine().TrimEnd('/');
                Console.WriteLine("Enter source corezoid api key login");
                Variables.SourceCorezoidApiKeyLogin = Console.ReadLine();
                Console.WriteLine("Enter source corezoid api key secret");
                Variables.SourceCorezoidApiKeySecret = Console.ReadLine();
                Console.WriteLine("Enter source folder id");
                Variables.SourceFolderId = Console.ReadLine();
                Console.WriteLine("Enter source state diagram id");
                Variables.SourceStateDiagramId = Console.ReadLine();
            }

            Console.WriteLine("Enter destination corezoid url");
            Variables.DestinationCorezoidUrl = Console.ReadLine().TrimEnd('/');
            Console.WriteLine("Enter destination corezoid api key login");
            Variables.DestinationCorezoidApiKeyLogin = Console.ReadLine();
            Console.WriteLine("Enter destination corezoid api key secret");
            Variables.DestinationCorezoidApiKeySecret = Console.ReadLine();
            Console.WriteLine("Enter destination state diagram id");
            Variables.DestinationStateDiagramId = Console.ReadLine();

            var services = new ServiceCollection();
            services.AddTransient<Bumblebee>();
            services.AddNodeServices();

            var serviceProvider = services.BuildServiceProvider();
            await serviceProvider.GetService<Bumblebee>().Run(serviceProvider.GetService<INodeServices>());
        }
    }
}