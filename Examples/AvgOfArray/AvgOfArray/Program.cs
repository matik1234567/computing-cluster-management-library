using CCMLibrary;

namespace AvgOfArray
{
    class Program
    {
        public static void Main(string[] args)
        {
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

            Console.ReadLine();
        }
    }

}