// need to add packaged  to project  before   use it  command if we using vs code
// dotnet add package Microsoft.Azure.Devices.Client
// dotnet add package Microsoft.Extensions.Configuration
// dotnet add package Microsoft.Extensions.Configuration.FileExtensions
// dotnet add package Microsoft.Extensions.Configuration.Json 
using System;
using System.IO;
using Microsoft.Azure.Devices.Client;

using System.Text;
using System.Threading.Tasks;
using System.Net.Http; //  modules for HTTP call
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Devices.Shared;  //  needs for device twin
using Microsoft.Extensions.Configuration.FileExtensions;
using Microsoft.Extensions.Configuration.Json;


namespace SensirionConsole
{
   
    class SensirionDevice
    {
        // The device connection string to authenticate the device with your IoT hub.
        private static string s_connectionString = "HostName=bsziothub.azure-devices.net;DeviceId=bszfuzzahT;SharedAccessKey=us1DMwtu4xtJgzqo+h86UMeyeINnRCJr1XbsUzmSnbg=";
        private static string gatewayname = "SOSDEMO1";
        private static string sensirionIp ="https://bszpfapp.azurewebsites.net/api/HttpTriggerT3";
        static SensirionConnect sc;
        // Async method to send simulated telemetry

        private static void Main(string[] args)
        {
            /// read config from appsettings
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            sensirionIp = config["sensirion"] ;   //https://bszpfapp.azurewebsites.net/api/HttpTriggerT3
            gatewayname = config["gatewayname"];
            s_connectionString = config["deviceConnectionsting"];

/// start the application
            Console.WriteLine("IoT Hub Quickstarts - Simulated device. Ctrl-C to exit.\n");
            Console.WriteLine(s_connectionString);
            Console.WriteLine("Sensirionip: {0} , Gateway name:{1}",sensirionIp,gatewayname);

            sc = new SensirionConnect(sensirionIp,s_connectionString);  //sensirion test
                        // Connect to the IoT hub using the MQTT protocol
            
            sc.Gatewayname = gatewayname;
            sc.RegisterMethods().Wait(); //  or  sc.RunSampleAsync().GetAwaiter().GetResult();
            sc.DeviceTwinHandler();
            sc.SendDeviceToCloudMessagesAsync();
            Console.ReadLine();
        }

    }

    class SensirionConnect
    {
        public string Gatewayname {get;set;}
        HttpClient client;
        DeviceClient s_deviceClient;
        SensirionDataConverter sdconv = new SensirionDataConverter();
        public SensirionConnect(string url,string IotconnectionString)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            s_deviceClient = DeviceClient.CreateFromConnectionString(IotconnectionString, TransportType.Mqtt);

        }
        /// Read Data from sensirion ki using HTTP connection
        private string getSensirionData()
        {
            string   rets= "X";
            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.    
            try{      
                HttpResponseMessage response = client.GetAsync("").Result;  //no url parameter, Blocking call! Program will wait here until a response is received or a timeout occurs.
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body.
                    var dataObjects = response.Content.ReadAsStringAsync().Result;  //Make sure to add a reference to System.Net.Http.Formatting.dll
                    rets = dataObjects;
                    Console.WriteLine("Data read from sensirion:");
                    Console.WriteLine("{0}", dataObjects);         
                    Console.WriteLine();       
                }
                else
                {
                    Console.WriteLine("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                }
            } catch (Exception ex)
            {
                rets = ex.Message;
            }
            return rets;

        }

        public async void SendDeviceToCloudMessagesAsync()
        {
            Console.WriteLine("Start sending messages to IoT Hub");
            while (true)
            {        
                try{
                    //  string rd = getSensirionData();     // send data to normal IoT hub
   
                    string rd = sdconv.convertSensirionData(  getSensirionData());  // send plane data to IoT central

                    var message = new Message(Encoding.ASCII.GetBytes(rd));
                    // Add a custom application property to the message.
                    // An IoT hub can filter on these properties without access to the message body.
                    message.Properties.Add("GatewayName", Gatewayname);

                    // Send the telemetry message
                    await s_deviceClient.SendEventAsync(message).ConfigureAwait(false);
                    Console.WriteLine();
                    Console.WriteLine("{0} > Sending message: {1}", DateTime.Now, rd);
                    

                    await Task.Delay(2000).ConfigureAwait(false);
                   }catch(Exception ex)
                   {

                   }

            }
        }


        
    public class DeviceInfo
    {
        public DeviceInfo(string myName)
        {
            this.Name = myName;
        }

        public string Name
        {
            get; set;
        }
    }
//  Functions that handling DeviceTwin
    public void  DeviceTwinHandler()
        {
            //await 
            s_deviceClient.SetDesiredPropertyUpdateCallbackAsync(OnDesiredPropertyChanged, null).ConfigureAwait(false)
            .GetAwaiter().GetResult();

            Console.WriteLine("Retrieving twin...");
            Twin twin = s_deviceClient.GetTwinAsync().ConfigureAwait(false)
                        .GetAwaiter().GetResult();

            Console.WriteLine("\tInitial twin value received:");
            Console.WriteLine($"\t{twin.ToJson()}");
            Console.WriteLine("Desired Properties {0}", twin.Properties.Desired.ToString());


            Console.WriteLine("Sending sample start time as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["MyReportedProperty"] = DateTime.Now;

            s_deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false)
                        .GetAwaiter().GetResult();

        }

        private async Task OnDesiredPropertyChanged(TwinCollection desiredProperties, object userContext)
        {
            Console.WriteLine("\tDesired property changed:");
            Console.WriteLine($"\t{desiredProperties.ToJson()}");

            Console.WriteLine("\tSending current time as reported property");
            TwinCollection reportedProperties = new TwinCollection();
            reportedProperties["DateTimeLastDesiredPropertyChangeReceived"] = DateTime.Now;

            await s_deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
        }

//***  End device Twin Handler */



    //Functions needs to implement a  Coud to Device method

        private void ConnectionStatusChangeHandler(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {
            Console.WriteLine();
            Console.WriteLine("Connection Status Changed to {0}", status);
            Console.WriteLine("Connection Status Changed Reason is {0}", reason);
            Console.WriteLine();
        }

        private static Task<MethodResponse> WriteToConsole(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t *** {nameof(WriteToConsole)} was called.");

            Console.WriteLine();
            Console.WriteLine("\t{0}", methodRequest.DataAsJson);
            Console.WriteLine();

            return Task.FromResult(new MethodResponse(new byte[0], 200));
        }

        private static Task<MethodResponse> GetDeviceName(MethodRequest methodRequest, object userContext)
        {
            Console.WriteLine($"\t *** {nameof(GetDeviceName)} was called.");

            MethodResponse retValue;
            if (userContext == null)
            {
                retValue = new MethodResponse(new byte[0], 500);
            }
            else
            {
                var d = userContext as DeviceInfo;
                string result = "{\"name\":\"" + d.Name + "\"}";
                retValue = new MethodResponse(Encoding.UTF8.GetBytes(result), 200);
            }
            return Task.FromResult(retValue);
        }


        public async Task RegisterMethods()
        {
            s_deviceClient.SetConnectionStatusChangesHandler(ConnectionStatusChangeHandler);

            // Method Call processing will be enabled when the first method handler is added.
            // Setup a callback for the 'WriteToConsole' method.
            await s_deviceClient.SetMethodHandlerAsync(nameof(WriteToConsole), WriteToConsole, null).ConfigureAwait(false);

            // Setup a callback for the 'GetDeviceName' method.
            await s_deviceClient.SetMethodHandlerAsync(nameof(GetDeviceName), GetDeviceName, new DeviceInfo("DeviceClientMethodSample")).ConfigureAwait(false);

            //Console.WriteLine("Waiting 30 seconds for IoT Hub method calls ...");

            Console.WriteLine($"Use the IoT Hub Azure Portal to call methods {nameof(GetDeviceName)} or {nameof(WriteToConsole)} within this time.");
            //await Task.Delay(30 * 1000).ConfigureAwait(false);
        }

        // ***  End Method
    }
}


