using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Bumblebee
{
    public class Bumblebee
    {
        public async Task Run(INodeServices nodeServices)
        {
            if (Variables.Step == "1")
            {
                await TransformProcesses(nodeServices);
                await GenerateProcessNames(nodeServices);
            }

            if (Variables.Step == "2")
            {
                await GenerateProcessNames(nodeServices);
            }

            if (Variables.Step == "3")
            {
                await GenerateProcessNames(nodeServices);
                await AddNewParamToAll(nodeServices);
            }

            Console.WriteLine("Done");
        }

        public async Task TransformProcesses(INodeServices nodeServices)
        {
            Console.WriteLine();
            Console.WriteLine();
            var data = await nodeServices.InvokeAsync<string>("transform", File.ReadAllText(Variables.SourceJsonPath), File.ReadAllText("./processes.json"), Variables.SourceStateDiagramId, Variables.DestinationStateDiagramId);

            File.WriteAllText("Content.json", data);
        }

        public async Task GenerateProcessNames(INodeServices nodeServices)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Enter destination json path");
            Variables.DestinationJsonPath = Console.ReadLine();

            dynamic data = JsonConvert.DeserializeObject(await nodeServices.InvokeAsync<string>("process", File.ReadAllText(Variables.DestinationJsonPath), File.ReadAllText("./processes.json")));

            File.WriteAllText("NewProcesses.json", JsonConvert.SerializeObject(data.processes, Formatting.Indented));
            File.WriteAllText("ProcessNames.json", JsonConvert.SerializeObject(data.processNames, Formatting.Indented));

            Thread.Sleep(1000);
        }

        public async Task AddNewParamToAll(INodeServices nodeServices)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Enter parameter source (Headers/Request/Empty)");
            Variables.NewParameterSource = Console.ReadLine();
            Console.WriteLine("Enter parameter name (UpperCamelCase)");
            Variables.NewParameterName = Console.ReadLine();
            Console.WriteLine("Send new parameter to apis in Headers (y/n)");
            Variables.NewParameterSendToApis = Console.ReadLine() == "y" ? true : false;

            dynamic data = JsonConvert.DeserializeObject(await nodeServices.InvokeAsync<string>("newParameterInAllProcesses", 
                                                                                                File.ReadAllText(Variables.DestinationJsonPath), 
                                                                                                File.ReadAllText("NewProcesses.json"),
                                                                                                Variables.NewParameterSource,
                                                                                                Variables.NewParameterName,
                                                                                                Variables.NewParameterSendToApis));

            File.WriteAllText("ParameterAdded.json", JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}