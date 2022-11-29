using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    internal class ServerConnectionException : Exception 
    {
        public ServerConnectionException(string message) : base(message)
        {
            //ServerRuntime.Log.Write(LogType.Exception, "server", message);
        }
    }
}
