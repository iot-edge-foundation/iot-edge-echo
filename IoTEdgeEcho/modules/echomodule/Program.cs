namespace echomodule
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Newtonsoft.Json;

    internal class Program
    {
        private static int counter;

        private static ModuleOutputList _moduleOutputs;

        private static void Main(string[] args)
        {
            Init().Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Initializes the ModuleClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        private static async Task Init()
        {
            Console.WriteLine("      _                         ___      _____   ___     _");
            Console.WriteLine("     /_\\   ___ _  _  _ _  ___  |_ _| ___|_   _| | __| __| | __ _  ___  ");
            Console.WriteLine("    / _ \\ |_ /| || || '_|/ -_)  | | / _ \\ | |   | _| / _` |/ _` |/ -_)");
            Console.WriteLine("   /_/ \\_\\/__| \\_,_||_|  \\___| |___|\\___/ |_|   |___|\\__,_|\\__, |\\___|");
            Console.WriteLine("                                                           |___/");
            Console.WriteLine("    ___    _        ");
            Console.WriteLine("   | __|__| |_  ___ ");
            Console.WriteLine("   | _|/ _| ' \\/ _ \\");
            Console.WriteLine("   |___\\__|_||_\\___/");
            Console.WriteLine(" ");
            Console.WriteLine("   Copyright © 2019 - IoT Edge Foundation");
            Console.WriteLine(" ");

            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            var ioTHubModuleClient = await ModuleClient.CreateFromEnvironmentAsync(settings);
            await ioTHubModuleClient.OpenAsync();

            AddOutputs(ioTHubModuleClient);

            _moduleOutputs.WriteOutputInfo();

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", PipeMessageInputOne, ioTHubModuleClient);

            Console.WriteLine("This module uses input 'input1'");

            var variables = Environment.GetEnvironmentVariables();

            Console.WriteLine($"Found {variables.Count} Environment variables:");

            foreach (DictionaryEntry variable in variables)
            {
                Console.WriteLine($"Environment variable '{variable.Key}': '{variable.Value}'");
            }

            Console.WriteLine("--------------------------------");
        }

        private static void AddOutputs(ModuleClient ioTHubModuleClient)
        {
            _moduleOutputs = new ModuleOutputList();

            var addedOutput1 = _moduleOutputs.Add(new ModuleOutput("output1", ioTHubModuleClient, "echo"));
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub.
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        private static async Task<MessageResponse> PipeMessageInputOne(Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref counter);

            var moduleClient = userContext as ModuleClient;
            if (moduleClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            var connectionDeviceId = message.ConnectionDeviceId;

            var connectionModuleId = message.ConnectionModuleId;

            byte[] messageBytes = message.GetBytes();
            string messageString = Encoding.UTF8.GetString(messageBytes);

            Console.WriteLine($"-> Received echo message: {counterValue}, Body: '{messageString}'");

            if (!string.IsNullOrEmpty(messageString))
            {
                var messageBody = JsonConvert.DeserializeObject(messageString);

                var moduleOutput = _moduleOutputs.GetModuleOutput("output1");

                if (moduleOutput != null)
                {
                    moduleOutput.Properties.Clear();

                    foreach (var prop in message.Properties)
                    {
                        moduleOutput.Properties.Add(prop.Key, prop.Value);

                        Console.WriteLine($"Property added: key:'{prop.Key}' value:'{prop.Value}'");
                    }

                    if (!string.IsNullOrEmpty(connectionDeviceId))
                    {
                        Console.WriteLine($"connectionDeviceId: '{connectionDeviceId}'");
                    }

                    if (!string.IsNullOrEmpty(connectionModuleId))
                    {
                        Console.WriteLine($"ConnectionModuleId: '{connectionModuleId}'");
                    }

                    await moduleOutput.SendMessage(messageBody);

                    Console.WriteLine("Received message echoed");
                }
            }

            return MessageResponse.Completed;
        }
    }
}