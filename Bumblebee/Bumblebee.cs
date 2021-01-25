using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Bumblebee
{
    public class Bumblebee
    {
        public async Task Run(INodeServices nodeServices)
        {
            if (Variables.Step == "1")
            {
                Console.WriteLine("Enter source json path:");
                Variables.SourceJsonPath = Console.ReadLine();

                var enterNextStateDiagramPair = "y";

                while (enterNextStateDiagramPair != "n")
                {
                    if (enterNextStateDiagramPair == "y")
                    {
                        Console.WriteLine("Enter source state diagram id:");
                        var sourceStateDiagramId = Console.ReadLine();
                        Console.WriteLine("Enter destination state diagram id:");
                        var destinationStateDiagramId = Console.ReadLine();

                        Variables.StateDiagramIds.Add(new StateDiagramIdModel() { SourceStateDiagramId = sourceStateDiagramId, DestinationStateDiagramId = destinationStateDiagramId });
                    }

                    Console.WriteLine("Are there more state diagram ids? y/n");
                    enterNextStateDiagramPair = Console.ReadLine();
                }


                var data = await nodeServices.InvokeAsync<string>("transform", File.ReadAllText(Variables.SourceJsonPath), File.ReadAllText("./processes.json"), Variables.StateDiagramIds);

                File.WriteAllText("Content.json", data);

                Variables.Step = "2";
            }

            if (Variables.Step == "2")
            {
                Console.WriteLine("Enter destination json path:");
                Variables.DestinationJsonPath = Console.ReadLine();

                dynamic data = JsonConvert.DeserializeObject(await nodeServices.InvokeAsync<string>("process", File.ReadAllText(Variables.DestinationJsonPath), File.ReadAllText("./processes.json")));

                File.WriteAllText("NewProcesses.json", JsonConvert.SerializeObject(data.processes, Formatting.Indented));
                File.WriteAllText("ProcessNames.json", JsonConvert.SerializeObject(data.processNames, Formatting.Indented));
            }

            Console.WriteLine("Done");
        }
    }
}