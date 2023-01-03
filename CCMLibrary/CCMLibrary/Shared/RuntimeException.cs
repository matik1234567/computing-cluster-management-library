using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class inherit from Exception class
    /// Define exception for Client and Server runtimes
    /// </summary>
    internal class RuntimeException : Exception
    {
        public static readonly string ClientMissingEror = "No client instance";
        public static readonly string ServerMissingEror = "No server instance";

        public RuntimeException() : base() {}

        public RuntimeException(string messager) : base(messager) {}
    }
}
