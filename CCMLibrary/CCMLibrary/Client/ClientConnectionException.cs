using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class inherit from Exception class and define exception for client when server connections are performed
    /// </summary>
    internal class ClientConnectionException : Exception
    {
        public ClientConnectionException(string message) : base(message) {}
    }
}
