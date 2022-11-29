using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    public enum RuntimeMode { TCP, Virtual };

    public class Runtime
    {
        public static RuntimeMode runtimeMode = RuntimeMode.TCP;

        protected static Type? clientType;
        protected static Type? serverType;

        public static void RegisterClientClass(Type clientClass)
        {
            if(clientClass.GetInterface(nameof(IClient)) == null)
            {
                throw new ArgumentException("IClient is not implemented");
            }
            clientType = clientClass;
            //var instance = Activator.CreateInstance(nodeTask);
        }

        public static void RegisterServerClass(Type serverClass)
        {
            if (serverClass.GetInterface(nameof(IServer)) == null)
            {
                throw new ArgumentException("IServer is not implemented");
            }
            serverType = serverClass;
        }

        public static void SetMode(RuntimeMode mode)
        {
            runtimeMode = mode;
        }

        protected Log GetDefaultLog()
        {
            Log log = new Log((LogType type, string address, string time, string arg) =>
            {
                 Console.WriteLine(String.Join("\t", new string[] { type.ToString(), time, address, arg }));
            });
            return log;
        }

        protected LogUser GetDefaultLogUser()
        {
            LogUser logUser = new LogUser((object[] val) =>
            {

                string[] strings = new string[val.Length];
                for (int i = 0; i < val.Length; i++)
                {
                    try
                    {
#pragma warning disable CS8601 // Possible null reference assignment.
                        strings[i] = Convert.ToString(val[i]);
#pragma warning restore CS8601 // Possible null reference assignment.
                    }
                    catch (Exception)
                    {
                        strings[i] = "[parse error]";
                    }
                }
                Console.WriteLine(String.Join("\t", strings));
            });
            return logUser;
        }
    }
}
