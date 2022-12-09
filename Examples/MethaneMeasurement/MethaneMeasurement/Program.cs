using CCMLibrary;

namespace MethaneMeasurement
{
    class Program
    {
        public static void StartClient(string ip, Int16 port)
        {
            ClientRuntime clientRuntime = new ClientRuntime();
            clientRuntime.Client(ip, port);
            clientRuntime.EnrollToProject();
            _ = clientRuntime.RunClient();
        }

        public static void Main(string[] args)
        {
            int mode = Convert.ToInt32(Console.ReadLine());

            Runtime.SetMode(RuntimeMode.TCP);
            Runtime.IntroduceFronted(typeof(Server), typeof(Client));

            string serverIp = "192.168.0.165";
            Int16 serverPort = 1302;

            if (mode == 0) // run server and local client
            {
                //define otput for result
                ResultLogCsv resultLog = new ResultLogCsv();
                GlobalVariables globalVariables = new GlobalVariables(new Dictionary<string, dynamic> {
                    { "dataDirectory", @"C:\Users\machu\Desktop\methane_data.csv" } 
                });
                
                ServerRuntime serverRuntime = new ServerRuntime(globalVariables, null, resultLog);
                serverRuntime.Server(serverPort);
                _ = serverRuntime.RunServerAsync();
                serverRuntime.Invite();

                // start local client
                StartClient(serverIp, serverPort); 

                // wait for clients to join
                Console.ReadLine();

                serverRuntime.RunTask();
                resultLog.SaveResults();
                serverRuntime.Stop();
            }
            else // run remote client
            {
                StartClient(serverIp, serverPort);
            }

            var r = Console.ReadLine();
        }
    }
}