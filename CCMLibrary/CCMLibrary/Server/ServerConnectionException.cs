using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class inherit from Exception class, define connection exception on server side
    /// </summary>
    internal class ServerConnectionException : Exception 
    {
        public ServerConnectionException(string message) : base(message)
        {
        }
    }
}
