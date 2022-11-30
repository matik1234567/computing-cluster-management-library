using CCMLibrary;

namespace AvgOfArray
{
    class Program
    {
        public static void Main(string[] args)
        {
            Runtime.SetMode(RuntimeMode.Virtual);
            Runtime.RegisterServerClass(typeof(Server));
            Runtime.RegisterClientClass(typeof(Client));
          
            ServerRuntime serverRuntime = new ServerRuntime();
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