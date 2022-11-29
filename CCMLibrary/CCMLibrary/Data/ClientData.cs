using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class hold client machine data
    /// </summary>
    public class ClientData
    {
        public string Name { get; set; }
        public int ProcessorCount { get; set; }
        public string OsName { get; set; }

        public ClientData()
        {
            Name = Environment.MachineName;
            ProcessorCount = Environment.ProcessorCount;
            OsName = Environment.OSVersion.ToString();
        }
    }
}
