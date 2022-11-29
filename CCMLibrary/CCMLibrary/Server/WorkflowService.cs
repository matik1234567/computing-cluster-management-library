using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace CCMLibrary
{

    /// <summary>
    /// Enum class define client connection status over Tcp
    /// </summary>
    public enum ConnectionStatus { Normal, Fail, Restoring, Undefined }


    /// <summary>
    /// Monitor clients workflow by establishing connection status
    /// Hold information about claint fails
    /// Controll hearbeat mechanic
    /// </summary>
    public class WorkflowService
    {
        private Dictionary<string, WorkflowData> _clientsWorkflow = new Dictionary<string, WorkflowData>();

        private static object locker = new();

        private Timer? _heartbeatCheck;
        private static int _heartbeatTolerance = 1000;
        private Stopwatch _watch = new Stopwatch();

        private ServerRuntime _runtime;

        public WorkflowService(ServerRuntime serverRuntime)
        {
            _runtime = serverRuntime;
        }

        public Dictionary<string, WorkflowData> GetCollection()
        {
            return _clientsWorkflow;
        }

        public void AddClient(ClientData client, string iPAddress)
        {
            if(_clientsWorkflow.Count == 0 && Runtime.runtimeMode == RuntimeMode.TCP)
            {
                 StartHeartbeatController(); 
            }
            if (!_clientsWorkflow.ContainsKey(iPAddress))
            {
                WorkflowData workdlowData = new WorkflowData(client);
                _clientsWorkflow[iPAddress] = workdlowData;
                _clientsWorkflow[iPAddress].LastBeat = (int)_watch.ElapsedMilliseconds;
                _clientsWorkflow[iPAddress].Status = ConnectionStatus.Normal;

            }
        }

        private void StartHeartbeatController()
        {
            _heartbeatCheck = new Timer();
            _heartbeatCheck.Elapsed += delegate
            {
                int checkValue = (int)_watch.ElapsedMilliseconds;
                foreach(var c in _clientsWorkflow)
                {
                    if (checkValue - c.Value.LastBeat <= _runtime.ProjectData.GetHeartBeatFreq() * 1000 + _heartbeatTolerance)
                    {
                        _clientsWorkflow[c.Key].Status = ConnectionStatus.Normal;
                    }
                    else
                    {
                        ClientFailure(c.Key);
                    }
                }

            };
            _heartbeatCheck.Interval = _runtime.ProjectData.GetHeartBeatFreq()*1000;

            _watch.Restart();
            _heartbeatCheck.Start();
        }

        public void ClientFailure(string iPAddress)
        {
            _clientsWorkflow[iPAddress].Status = ConnectionStatus.Fail;
            if(_clientsWorkflow[iPAddress].TaskIdsInProgress != null)
            {
#pragma warning disable CS8604 // Possible null reference argument for parameter 'iaskIds' in 'void NodeTaskService.ResendTasks(ulong[] iaskIds)'.
                _runtime.NodeTaskService.ResendTasks(_clientsWorkflow[iPAddress].TaskIdsInProgress);
#pragma warning restore CS8604 // Possible null reference argument for parameter 'iaskIds' in 'void NodeTaskService.ResendTasks(ulong[] iaskIds)'.
                _clientsWorkflow[iPAddress].TaskIdsInProgress = null;
            }
        }

        public void RemoveClient(string iPAddress)
        {
            if (_clientsWorkflow.ContainsKey(iPAddress))
            {
                 ClientFailure(iPAddress);
                _clientsWorkflow.Remove(iPAddress);
                
            }
            if (_clientsWorkflow.Count == 0)
            {
                _heartbeatCheck?.Stop();
                _watch.Stop();  
            }

        }

        public void AliveConfirmation(string iPAddress)
        {
            _clientsWorkflow[iPAddress].LastBeat = (int)_watch.ElapsedMilliseconds;  
        }

        public void SetInProgressTasks(string iPAddress, ulong[] tasksIds)
        {
            _clientsWorkflow[iPAddress].TaskIdsInProgress = tasksIds;
        }

        public void ReleaseInProgressTasks(string iPAddress)
        {
            _clientsWorkflow[iPAddress].TaskIdsInProgress = null;
        }

        public int GetTaskCountFor(string iPAddress)
        {
            return _clientsWorkflow[iPAddress].ProcessorCount - 1;
        }

        public bool IsValidReturn(string iPAddress)
        {
            return _clientsWorkflow[iPAddress].TaskIdsInProgress != null;
        }

        public bool IsNoRunningTasks()
        {
            
            foreach(var clientW in _clientsWorkflow)
            {
                if(clientW.Value.TaskIdsInProgress != null)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsInProgressTasks()
        {
            lock (locker)
            {
                return _clientsWorkflow.Any((c) => c.Value.TaskIdsInProgress != null);
            }
        }

        public bool IsEmpty()
        {
            lock (locker)
            {
                return _clientsWorkflow.Count == 0;
            }
            
        }

        public void Reset()
        {
            foreach(var client in _clientsWorkflow)
            {
                client.Value.TaskIdsInProgress = null;
                client.Value.Status = ConnectionStatus.Undefined;
            }
        }

        public ConnectionStatus GetStatus(string iPAddess)
        {
            return _clientsWorkflow[iPAddess].Status;
        }

        public int GetCount(ConnectionStatus? connectionStatus)
        {
            if(connectionStatus == null)
            {
                return _clientsWorkflow.Count;
            }
            return _clientsWorkflow.Count((c) => c.Value.Status == connectionStatus);
        }
       
        public void Dispose()
        {
            _watch.Stop();
            _heartbeatCheck?.Dispose();
            _clientsWorkflow.Clear();
        }

    }
}
