using CCMLibrary;

namespace AvgOfArray
{
    class Program
    {
        public static void ServerMode()
        {
            Dictionary<string, dynamic> map = new Dictionary<string, dynamic>();
            map.Add("heartBeatFrequency", 7);
            map.Add("listCount", 100000);
            GlobalVariables globalVariables = new GlobalVariables(map);
            ServerRuntime serverRuntime = new ServerRuntime(globalVariables);

            serverRuntime.Server(1302);
            _ = serverRuntime.RunServerAsync();
            serverRuntime.Invite();

            Console.WriteLine("Press any key to continue");
            Console.ReadLine();
            serverRuntime.RunTask();
        }

        public static void ClientMode()
        {
            ClientRuntime clientRuntime = new ClientRuntime();
            clientRuntime.Client("192.168.1.139", 1302);
            clientRuntime.EnrollToProject();
            clientRuntime.RunClient();
        }

        public static void Main(string[] args)
        {
            int mode = Convert.ToInt32(Console.ReadLine());
            Runtime.SetMode(RuntimeMode.TCP);
            Runtime.IntroduceFronted(typeof(Server), typeof(Client));
            if (mode == 0)
            {
                ServerMode();
            }
            else
            {
                ClientMode();
            }

            // Virtual mode
            /*
            Runtime.SetMode(RuntimeMode.Virtual);
            Runtime.IntroduceFronted(typeof(Server), typeof(Client));

            Dictionary<string, dynamic> map = new Dictionary<string, dynamic>();
            map.Add("heartBeatFrequency", 7);
            map.Add("listCount", 100000);
            GlobalVariables globalVariables= new GlobalVariables(map);
            ServerRuntime serverRuntime = new ServerRuntime(globalVariables);

            serverRuntime.Server();
            _ = serverRuntime.RunServerAsync();
            serverRuntime.Invite();

            ClientRuntime clientRuntime = new ClientRuntime();
            clientRuntime.Client();
            clientRuntime.EnrollToProject();
            _ = clientRuntime.RunClient();

            serverRuntime.RunTask();

            Console.ReadLine();*/
        }
    }

}