using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    internal class ClientConnectionData
    {
#pragma warning disable CS8618 // Non-nullable property 'ServerHostName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public string ServerHostName { get; set; }
#pragma warning restore CS8618 // Non-nullable property 'ServerHostName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public Int16 ServerPort { get; set; }
#pragma warning disable CS8618 // Non-nullable property 'ClientSocket' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public Socket ClientSocket { get; set; }
#pragma warning restore CS8618 // Non-nullable property 'ClientSocket' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
    }
}
