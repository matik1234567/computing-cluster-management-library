using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class represent connected client data
    /// </summary>
    public class WorkflowData
    {
        public string Name { get; set; }
        public int ProcessorCount { get; set; }
        public string OsName { get; set; }
        public ulong[] TaskIdsInProgress { get; set; }
        public ConnectionStatus Status { get; set; }
        public int LastBeat { get; set; }

#pragma warning disable CS8618 // Non-nullable property 'Name' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning disable CS8618 // Non-nullable property 'OsName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        public WorkflowData(ClientData clientData)
#pragma warning restore CS8618 // Non-nullable property 'OsName' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
#pragma warning restore CS8618 // Non-nullable property 'Name' must contain a non-null value when exiting constructor. Consider declaring the property as nullable.
        {
            if (clientData == null)
            {
                return;
            }
            Name = clientData.Name;
            ProcessorCount = clientData.ProcessorCount;
            OsName = clientData.OsName;
            Status = ConnectionStatus.Undefined;
            LastBeat = 0;
            TaskIdsInProgress = new ulong[0];
        }


    }
}
