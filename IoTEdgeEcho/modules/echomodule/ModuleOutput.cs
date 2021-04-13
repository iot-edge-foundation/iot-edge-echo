namespace echomodule
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Newtonsoft.Json;

    public class ModuleOutput
    {
        private ModuleClient _ioTHubModuleClient;

        public ModuleOutput(string name, ModuleClient ioTHubModuleClient) 
        {
            Initialize(name,  ioTHubModuleClient);
        }

        public ModuleOutput(string name, ModuleClient ioTHubModuleClient, string context) 
        {
            Context = context;

            Initialize(name,  ioTHubModuleClient);
        }

        public string Name { get; private set; } = string.Empty;

        public string Context {get; private set;} = string.Empty;

        public Dictionary<string, string> Properties { get; private set; }


        public async Task SendMessage(object messageBody)
        {
            var jsonMessage = JsonConvert.SerializeObject(messageBody);

            var messageBytes = Encoding.UTF8.GetBytes(jsonMessage);

            using (var message = new Message(messageBytes))
            {
                // Set message body type and content encoding for routing using decoded body values.
                message.ContentEncoding = "utf-8";
                message.ContentType = "application/json";

                foreach (var property in Properties)
                {
                    message.Properties.Add(property);
                }

                await _ioTHubModuleClient.SendEventAsync(Name, message);

                var size = CalculateSize(messageBytes, Properties);

                Console.WriteLine($"Message with size {size} bytes sent.");
            }
        }

        private static int CalculateSize(byte[] messageBytes, Dictionary<string, string> properties)
        {
            using (var message = new Message(messageBytes))
            {
                message.ContentEncoding = "utf-8";
                message.ContentType = "application/json";

                var result = message.GetBytes().Length;

                foreach (var p in properties)
                {
                    result = result + p.Key.Length + p.Value.Length;
                }

                return result;
            }
        }

        private void Initialize(string name, ModuleClient ioTHubModuleClient) 
        {
            Name = name;

            _ioTHubModuleClient = ioTHubModuleClient;

            CreateProperties();
        }

        private void CreateProperties()
        {
            Properties = new Dictionary<string, string>();

            var contentType = $"application/edge";

            if (Name != string.Empty)
            {
                contentType += $"-{Name}";
            }

            if (Context != string.Empty)
            {
                contentType += $"-{Context}";
            }

            contentType += "-json";

            // Set a property as a fingerprint for this module
            Properties.Add("content-type", contentType);
        }
    }
}