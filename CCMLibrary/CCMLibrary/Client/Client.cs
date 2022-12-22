using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;
using CCMLibrary.Enums;
using System.Drawing;
using System.Net.Security;

namespace CCMLibrary
{
    /// <summary>
    /// Class Provide tcp communication between client and server
    /// </summary>
    internal class Client
    {
        private bool _forceStop = false;
        private bool _isRunning = false;
        private object _hearBeatSignalLocker = new();

        private Timer? _timer;
        private ClientRuntime _runtime;

        private Socket? _clientSocket;
        private string _serverIpAddress;
        private Int16 _serverPort;

        /// <summary>
        /// Constructor for TCP runtime mode
        /// </summary>
        /// <param name="connectionData"></param>
        /// <param name="runtime"></param>
        public Client(ClientConnectionData connectionData, ClientRuntime runtime)
        {
            _runtime = runtime;
            _clientSocket = connectionData.ClientSocket;
            _serverIpAddress = connectionData.ServerHostName;
            _serverPort = connectionData.ServerPort;
        }


        /// <summary>
        /// Constructor for Virtual runtime mode
        /// </summary>
        /// <param name="runtime"></param>
        public Client(ClientRuntime runtime)
        {
            _runtime = runtime;
            _clientSocket = null;
            _serverIpAddress = "";
            _serverPort = -1;
        }

        /// <summary>
        /// Method initiate heartbeat signal with cyclic timer
        /// </summary>
        private void InitHearbeatSignal()
        {
            if(Runtime.runtimeMode == RuntimeMode.Virtual)
            {
                return;
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            _timer = new Timer(TimeSpan.FromSeconds(_runtime.ProjectData.GetHeartBeatFreq()).TotalMilliseconds);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            _timer.AutoReset = true;
#pragma warning disable CS8622 // Nullability of reference types in type of parameter 'sender' of 'void Client.EmitHearbeatSignal(object sender, ElapsedEventArgs e)' doesn't match the target delegate 'ElapsedEventHandler' (possibly because of nullability attributes).
            _timer.Elapsed += new ElapsedEventHandler(EmitHearbeatSignal);
#pragma warning restore CS8622 // Nullability of reference types in type of parameter 'sender' of 'void Client.EmitHearbeatSignal(object sender, ElapsedEventArgs e)' doesn't match the target delegate 'ElapsedEventHandler' (possibly because of nullability attributes).
            _timer.Start();
        }

        /// <summary>
        /// Method send heardbeat to server with safe threading implementation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EmitHearbeatSignal(object sender, ElapsedEventArgs e)
        {
            lock (_hearBeatSignalLocker)
            {
                try
                {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                    (ServerResponse responseHeader, object responseData) = SendToServer(ClientRequest.HearbeatConfirmation, null);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                }
                catch (ClientConnectionException)
                {
                    throw;
                }
            }
            
        }

        /// <summary>
        /// Method send data to server in Virtual runtime mode
        /// </summary>
        /// <param name="requestHeader"></param>
        /// <param name="requestData"></param>
        /// <returns></returns>
        private (ServerResponse, object?) SendToServerVirtual(ClientRequest requestHeader, object? requestData)
        {
            Runtime.virtualConnection?.SendRequest(requestHeader, ref requestData);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return Runtime.virtualConnection.ReceiveResponse();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        /// <summary>
        /// Method initiate first client server connection
        /// </summary>
        /// <exception cref="ClientConnectionException"></exception>
        /// <exception cref="Exception"></exception>
        public void EnrollToProject()
        {
            ServerResponse responseHeader;
            object? responseData;

            try
            {
                (responseHeader, responseData) = SendToServer(ClientRequest.EnrollProject, _runtime.ClientData);
            }
            catch (Exception)
            {
                _runtime.Log?.Write(LogType.Exception, "client", $"unable to connect with {_serverIpAddress}:{_serverPort}");
                throw new ClientConnectionException($"Unable to connect with {_serverIpAddress}:{_serverPort}");
            }
           
            if (responseHeader == ServerResponse.ProjectGlobalAttributes && responseData != null)
            {
                _runtime.ProjectData = (GlobalVariables)responseData;
                _runtime.Log?.Write(LogType.Information, "client", $"received global attributes");

                InitHearbeatSignal();
            }
            else
            {
                _runtime.Log?.Write(LogType.Exception, "client", $"unable to connect with {_serverIpAddress}:{_serverPort}");
                throw new Exception($"Cannot receive global attributes");
            }
        }

        private (ServerResponse, object?) SendToServer(ClientRequest requestHeader, object? requestData)
        {
            if(_runtime.Log?.LogLevel == LogLevel.Debug)
            {
                _runtime.Log?.Write(LogType.Information, "client", $"[request]: {requestHeader}");
            }
           
            ServerResponse serverResponse;
            object? responseData;
            if (_clientSocket == null)
            {
                (serverResponse, responseData) = SendToServerVirtual(requestHeader, requestData);
            }
            else
            {
                (serverResponse, responseData) = SendToServerTcp(requestHeader, requestData);
            }
            if (_runtime.Log?.LogLevel == LogLevel.Debug)
            {
                _runtime.Log?.Write(LogType.Information, "client", $"[response]: {serverResponse}");
            }
            return (serverResponse, responseData); 
        }

        /// <summary>
        /// Method send data to server in TCP runtime mode
        /// </summary>
        /// <param name="requestHeader"></param>
        /// <param name="requestData"></param>
        /// <returns></returns>
        /// <exception cref="ClusterSystemException"></exception>
        private (ServerResponse, object?) SendToServerTcp(ClientRequest requestHeader, object? requestData)
        {

            lock (_hearBeatSignalLocker)
            {

#pragma warning disable CS8602 // Dereference of a possibly null reference.
                if (!_clientSocket.Connected)
                {
                    try
                    {
                        _clientSocket.Connect(_serverIpAddress, _serverPort);
                        _runtime.Log?.Write(LogType.Information, "client", $"connected to {_serverIpAddress}:{_serverPort}" );
                    }
                    catch (Exception)
                    {
                        return (ServerResponse.Null, null);
                        // server close/damage
                    }
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                byte[] request = PackageHandler.SerializeObject(requestHeader, requestData);
                string json = "";
                try
                {
                    _clientSocket.Send(request);
                    byte[] temp = new byte[4];
                    _clientSocket.Receive(temp);
                    int size = BitConverter.ToInt32(temp, 0);
                    List<Task<string>> tasks = new List<Task<string>>();
                    while (size > 0)
                    {
                        byte[] bufferTemp;
                        
                        if (size < _clientSocket.ReceiveBufferSize)
                        {
                            bufferTemp = new byte[size];
                        }
                        else
                        {
                            bufferTemp = new byte[_clientSocket.ReceiveBufferSize];
                        }
                        bufferTemp = new byte[_clientSocket.ReceiveBufferSize];
                        int rec = _clientSocket.Receive(bufferTemp, 0, bufferTemp.Length, 0);
                        size -= rec;
                        Task<string> task = new Task<string>(() => { return PackageHandler.GetStringFromBytes(ref bufferTemp); });
                        tasks.Add(task);
                        tasks[^1].Start();
                    }
                    Task.WaitAll(tasks.ToArray());
                    StringBuilder stringBuilder1 = new StringBuilder();
                    foreach (var t in tasks)
                    {
                        stringBuilder1.Append(t.Result);
                    }
                    json = stringBuilder1.ToString();

                }
                catch (Exception)
                {
                    return (ServerResponse.Null, null);
                    // server close/damage
                }
                
                if (json == "")
                {
                    return (ServerResponse.Null, null);
                    // server close/damage
                }

                ServerResponse responseHeader;
                object responseData;
                try
                {
                    (responseHeader, responseData) = ((ServerResponse, object))PackageHandler.DeserializeObject(json);
                   // ms.Dispose();
                }
                catch (Exception e)
                {
                    throw new ClusterSystemException(e.Message);
                }
               
                return (responseHeader, responseData);
            }
        }

        /// <summary>
        /// Method start client process, maintain connection with server whole time
        /// </summary>
        public async void RunClient()
        {
            _forceStop = false;
            _isRunning = true;

            ServerResponse responseHeader = ServerResponse.Null;
            object? responseData = null;

            bool init = true;

            TaskRuleAttribute taskRule = _runtime.GetTasksRule();
            int taskCount = 1;
            int maxResources = 0;

            if(taskRule.TaskExecution == TaskExecution.Sync)
            {
                taskCount = taskRule.TasksCount;
            }
            else
            {
                maxResources = (int)(taskRule.Cores * _runtime.ClientData.ProcessorCount);
                if (taskRule.Cores != -1)
                {
                    taskCount = (taskRule.CoresMultiply* maxResources);
                }
                else
                {
                    taskCount = taskRule.TasksCount;
                }
                
            }

            while (!_forceStop)
            {
                if (init)
                {
                    try
                    {
                        (responseHeader, responseData) = SendToServer(ClientRequest.TaskRequest, taskCount);
                    }
                    catch (ClientConnectionException)
                    {
                        throw;
                    }
                }
                if (responseData == null)
                {
                    return;
                }
                switch (responseHeader)
                {
                    case ServerResponse.RequestUnavailable:
                        ServerPhase phase = (ServerPhase)responseData;
                        _runtime.Log?.Write(LogType.Exception, "client", $"RequestUnavailable server phase: {phase}");
                        return;
                    case ServerResponse.Poison:
                        _timer?.Dispose();
                        return;
                    case ServerResponse.Null:
                        _timer?.Dispose();
                        return;
                    case ServerResponse.Wait:
                        Thread.Sleep((int)responseData);
                        break;
                    case ServerResponse.ProjectTaskData:
                        List<NodeTask> tasks = (List<NodeTask>)responseData;
                        Stopwatch watchRunTasks  = Stopwatch.StartNew();

                        if (taskRule.TaskExecution == TaskExecution.Sync)
                        {
                            HandleTask(ref tasks);
                        }
                        else
                        {
                            HandleTaskAsync(tasks, maxResources);
                        }
                        watchRunTasks.Stop();

                        (responseHeader, responseData) = SendToServer(ClientRequest.TaskResultsReturnRequest,
                            (tasks, taskCount));
                        init = false;
                        continue;
                    default:
                        _runtime.Log?.Write(LogType.Exception, "client", "server stoped");
                        return;
                }

                init = true;
            }
            _timer?.Dispose();

            if (_forceStop)
            {
                (responseHeader, responseData) = SendToServer(ClientRequest.CancelEnrollment, null); 
            }
            _isRunning = false;
        }

        /// <summary>
        /// Called each time server return tasks and client rule is async execution
        /// </summary>
        /// <param name="nodeTasks"></param>
        /// <returns></returns>
        private List<NodeTask> HandleTaskAsync(List<NodeTask> nodeTasks, int maxResources)
        {
            TaskComposer.Init(maxResources);
            Task[] tasks = new Task[nodeTasks.Count];

            for (int i = 0; i < nodeTasks.Count; i++)
            {
                NodeTask tt = nodeTasks[i];
                Task task = new Task(() => { _runtime.ClientFront?.OnTasksReceiveAsync(tt); });
                tasks[i] = task;
                tasks[i].Start();

                TaskComposer.Hold();

                while (TaskComposer.GetAvailableResources() <= 0)
                {
                    ;
                }

            }
            Task.WaitAll(tasks);
            return TaskComposer.GetCompletedTasks();
          
        }

        /// <summary>
        /// Called each time server return tasks and client rule is sync execution
        /// </summary>
        /// <param name="nodeTasks"></param>
        private void HandleTask(ref List<NodeTask> nodeTasks)
        {
            _runtime.ClientFront?.OnTasksReceive(ref nodeTasks);
        }

        public void RequestCancellation()
        {
            _forceStop = true;
        }

        public bool IsRunning()
        {
            return _isRunning;
        }

    }
}
