using Microsoft.AspNetCore.NodeServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bumblebee
{
    public class Bumblebee
    {
        public async Task Run(INodeServices nodeServices)
        {
            var result = Variables.Step != "1" || await Download(nodeServices, Variables.SourceFolderId, Variables.SourceCorezoidApiKeySecret, Variables.SourceCorezoidUrl, Variables.SourceCorezoidApiKeyLogin, 1);

            if (result)
            {
                if (Variables.Step == "1")
                {
                    Console.WriteLine("Downloaded from source");
                }

                Console.WriteLine("Enter destination folder id");
                Variables.DestinationFolderId = Console.ReadLine();

                await Download(nodeServices, Variables.DestinationFolderId, Variables.DestinationCorezoidApiKeySecret, Variables.DestinationCorezoidUrl, Variables.DestinationCorezoidApiKeyLogin, 2);

                Console.WriteLine("Done");
            }
        }

        private async Task<bool> Download(INodeServices nodeServices, string sourceFolderId, string sourceCorezoidApiKeySecret, string sourceCorezoidUrl, string sourceCorezoidApiKeyLogin, int step)
        {
            Console.WriteLine("Working...");

            using (var handler = new HttpClientHandler())
            {
                handler.ServerCertificateCustomValidationCallback = (message, certificate, chain, errors) => true;

                using (var client = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(10) })
                {
                    ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;

                    dynamic operation = new ExpandoObject();
                    operation.type = "get";
                    operation.obj_type = "folder";
                    operation.obj = "obj_scheme";
                    operation.obj_id = sourceFolderId;

                    dynamic obj = new { ops = new[] { new ExpandoObject() } };
                    obj.ops[0] = operation;

                    var json = JsonConvert.SerializeObject(obj, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

                    try
                    {
                        using (var sha1 = SHA1.Create())
                        {
                            var timeStamp = ToUnixTimeSeconds(DateTime.Now);
                            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes($"{timeStamp}{sourceCorezoidApiKeySecret}{json}{sourceCorezoidApiKeySecret}"));
                            var signature = BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                            var response = await client.PostAsync($"{sourceCorezoidUrl}/api/2/json/{sourceCorezoidApiKeyLogin}/{timeStamp}/{signature}", new StringContent(json, Encoding.UTF8, "application/json"));

                            if (!response.IsSuccessStatusCode)
                            {
                                throw new Exception(response.ToString());
                            }

                            var content = await response.Content.ReadAsStringAsync();
                            JObject jObject;

                            try
                            {
                                jObject = JObject.Parse(content);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"{ex}. {content}");
                            }

                            if (jObject.SelectToken("request_proc")?.Value<string>() != "ok" || jObject.SelectToken("ops")?[0].SelectToken("proc")?.Value<string>() != "ok")
                            {
                                var errors = jObject.SelectToken("ops")?[0].SelectToken("errors")?.Value<JArray>();

                                if (errors != null)
                                {
                                    for (int i = 0; i < errors.Count; i++)
                                    {
                                        Console.WriteLine(errors[i].SelectToken("description")?.Value<string>());
                                    }
                                }

                                var description = jObject.SelectToken("ops")?[0].SelectToken("description")?.Value<string>();

                                if (description != null)
                                {
                                    Console.WriteLine(description);
                                }

                                return false;
                            }

                            if (step == 1)
                            {
                                var processes = File.ReadAllText("./processes.json");
                                var data = await nodeServices.InvokeAsync<string>("transform", content, processes, Variables.SourceStateDiagramId, Variables.DestinationStateDiagramId);

                                File.WriteAllText("Content.json", data);
                            }

                            if (step == 2)
                            {
                                var processes = File.ReadAllText("./processes.json");
                                dynamic data = JsonConvert.DeserializeObject(await nodeServices.InvokeAsync<string>("process", content, processes));

                                File.WriteAllText("NewProcesses.json", JsonConvert.SerializeObject(data, Formatting.Indented));
                            }

                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());

                        return false;
                    }
                }
            }

        }

        private long ToUnixTimeSeconds(DateTime dateTime)
        {
            return (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, 0)).TotalSeconds;
        }
    }
}