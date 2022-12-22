using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CCMLibrary.Enums;

namespace CCMLibrary
{
    /// <summary>
    /// Class implements server behaviour
    /// </summary>
    internal class BrokerServer
    {
        private volatile IPAddress? _ipAddress;
        private volatile Socket? _serverSocket;
        private volatile Int16 _port;

        private volatile ServerPhase _phase = ServerPhase.Idle;
        private volatile DecisionTable<ServerPhase, ClientRequest> _driver;

        private volatile static int _clientWaitOnTaskAvailabiltyMili = 50;
        private volatile ServerRuntime _serverRuntime;
        private volatile List<Socket> _clientSockets = new List<Socket>();

        private volatile string _machineIPAddress;

        private volatile bool _cancelVirtualConTask = false;

        /// <summary>
        /// Constructor for TCP runtime mode
        /// </summary>
        /// <param name="port">socket port</param>
        /// <param name="serverRuntime">associated with process</param>
        public BrokerServer(Int16 port, ServerRuntime serverRuntime)
        {
            _ipAddress = IPAddress.Any;
            _port = port;
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverRuntime = serverRuntime;
            _driver = DriverService.GetServerDriver(this);

            string hostName = Dns.GetHostName(); // Retrive the Name of HOST
            _machineIPAddress = Dns.GetHostEntry(hostName).AddressList[^1].ToString();
        }

        /// <summary>
        /// Constructor for Virtual runtime mode
        /// </summary>
        /// <param name="serverRuntime">Associate with process</param>
        public BrokerServer(ServerRuntime serverRuntime)
        {
            _port = 0;
            _serverRuntime = serverRuntime;
            _driver = DriverService.GetServerDriver(this);
            _machineIPAddress = "";
        }

        public ServerPhase GetServerPhase()
        {
            return _phase;
        }
        private object l = new object();
        private (ServerResponse, object) HandleResponse(ClientRequest request, object requestData, object senderIP)
        {
            try
            {
                lock (l)
                {
                    return ((ServerResponse, object))_driver.GetActionResults(_phase, request, requestData, senderIP);
                }
                
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void AcceptCallback(IAsyncResult AR) 
        {
            Socket socket;
            try
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                socket = _serverSocket.EndAccept(AR);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
            catch(Exception)
            {
                return;
            }
            
            _clientSockets.Add(socket);
            byte[] buffer = new byte[1];
            socket.BeginReceive(buffer, 0, 0, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void ReceiveCallbackVirtual()
        {
            while (_cancelVirtualConTask!=true)
            {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                (ClientRequest requestHeader, object? requestData) = Runtime.virtualConnection.ReceiveRequest();
                if (requestHeader == ClientRequest.None)
                {
                    break;
                }
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument for parameter 'requestData' in '(ServerResponse, object) BrokerServer.HandleResponse(ClientRequest request, object requestData, object senderIP)'.
                (ServerResponse responseHeader, object? responseData) = HandleResponse((ClientRequest)requestHeader, requestData, "client");
#pragma warning restore CS8604 // Possible null reference argument for parameter 'requestData' in '(ServerResponse, object) BrokerServer.HandleResponse(ClientRequest request, object requestData, object senderIP)'.
                Runtime.virtualConnection.SendResponse(responseHeader, ref responseData);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            if (AR.AsyncState == null)
            {
                throw new ClusterSystemException("Server null at ReceiveCallback");
            }

            Socket socket = (Socket)AR.AsyncState;
            IPEndPoint? remoteIpEndPoint;
            try
            {
                remoteIpEndPoint = socket.RemoteEndPoint as IPEndPoint;
            }
            catch(Exception)// socket may close
            {
                return;
            }
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            string ipAddress = remoteIpEndPoint.Address.ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.

            byte[] buffer = new byte[4];
#pragma warning disable CS0168 // The variable 'dataBuffer' is declared but never used
            byte[] dataBuffer;
#pragma warning restore CS0168 // The variable 'dataBuffer' is declared but never used
            string json = "";
            Dictionary<byte[], char[]> buffers = new Dictionary<byte[], char[]>();

#pragma warning disable CS0168 // The variable 'e' is declared but never used
            MemoryStream ms;
            try
            {
                socket.Receive(buffer, 0, buffer.Length, 0); // get size of message
                int size = BitConverter.ToInt32(buffer, 0);

                ms = new MemoryStream();
                byte[] bufferTemp;
                while (size>0)
                {
                    if (size < socket.ReceiveBufferSize)
                    {
                        bufferTemp = new byte[size];
                    }
                    else
                    {
                        bufferTemp = new byte[socket.ReceiveBufferSize];
                    }

                    int rec = socket.Receive(bufferTemp, 0, bufferTemp.Length, 0);
                    size -= rec;
       
                    ms.Write(bufferTemp, 0, rec);

                   // json += PackageHandler.GetStringFromBytes(ref bufferTemp);
                }
                socket.EndReceive(AR);
            }
            catch(Exception e)
            {
                socket.Close();
                return;
            }
            if (ms.Length == 0)
            {
                socket.Close();
                return;
            }
#pragma warning restore CS0168 // The variable 'e' is declared but never used
           
            ServerResponse responseHeader;
            object responseData;
            byte[] data;
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Enum requestHeader = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            
            try
            {
                (requestHeader, object? requestData) = PackageHandler.DeserializeObject(ref ms);
                ms.Dispose();
                (responseHeader, responseData) = HandleResponse((ClientRequest)requestHeader, requestData, ipAddress);
                data = PackageHandler.SerializeObject(responseHeader, responseData);  
            }
            catch(Exception e)
            {

#pragma warning disable CS8605 // Unboxing a possibly null value.
                ClientRequest r = (ClientRequest)requestHeader;
#pragma warning restore CS8605 // Unboxing a possibly null value.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _serverRuntime.Log?.Write(LogType.Exception, _ipAddress.ToString(), r.ToString() + ":" + _phase.ToString());
#pragma warning restore CS8602 // Dereference of a possibly null reference.

                throw new ClusterSystemException(e.Message);
            }

            if(_serverRuntime.Log?.LogLevel == LogLevel.Debug)
            {
                _serverRuntime.Log?.Write(LogType.Information, _ipAddress.ToString(), $"[request]: {requestHeader.ToString()}");
                _serverRuntime.Log?.Write(LogType.Information, _ipAddress.ToString(), $"[response]: {responseHeader.ToString()}");
            }

            try
            {
                socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            }
            catch(Exception e)
            {
                throw new ClusterSystemException(e.Message);
            }
           
            buffer = new byte[1];
            socket.BeginReceive(buffer, 0, 0, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }

        private void SendCallback(IAsyncResult AR)
        {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            Socket socket = (Socket)AR.AsyncState;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            socket.EndSend(AR);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }

        public void StartServer()
        {
            if(Runtime.runtimeMode == RuntimeMode.Virtual)
            {
                _cancelVirtualConTask = false;
                _ =  Task.Run(()=> { ReceiveCallbackVirtual(); });
            }
            else
            {
                _serverSocket?.Bind(new IPEndPoint(IPAddress.Any, _port));
                _serverSocket?.Listen(0);
                _serverSocket?.BeginAccept(new AsyncCallback(AcceptCallback), null);
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                _serverRuntime.Log?.Write(LogType.Information, _machineIPAddress, $"[port]: {_port} Server start" );
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            }
        }

        public void CloseAllSockets()
        {
            foreach (var socket in _clientSockets)
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }

            _serverSocket?.Close();
            _cancelVirtualConTask = true;
            Runtime.virtualConnection?.Stop();
            _serverRuntime.Log?.Write(LogType.Information, _machineIPAddress, "Server stop");
        }

        // Set states
        public void SendingProjectInvitation()
        {
            this._phase = ServerPhase.SendingInvitation;
            _clientWaitOnTaskAvailabiltyMili = 10;
            _serverRuntime.Log?.Write(LogType.Information, _machineIPAddress, "Invitations phase");
        }

        public void ProjectInProgress()
        {
            this._phase = ServerPhase.InProgress;
            _clientWaitOnTaskAvailabiltyMili = 1;
            _serverRuntime.Log?.Write(LogType.Information, _machineIPAddress,"Tasks progress phase");
        }


        public void ProjectCancelation()
        {
            this._phase = ServerPhase.Cancelation;
            _cancelVirtualConTask = true;
            _serverRuntime.Log?.Write(LogType.Information, _machineIPAddress, "Cancellation phase");
        }

        public void ProjectFreeze()
        {
            this._phase = ServerPhase.Freeze;
            _clientWaitOnTaskAvailabiltyMili = 10;
            _serverRuntime.Log?.Write(LogType.Information, _machineIPAddress, "Freeze phase");
        }


        //End states

        //Delegates
        public (ServerResponse, object?) ProjectInfoData(object ob, object senderIP)
        {
            ClientData clientData = (ClientData)ob;
            _serverRuntime.WorkflowService.AddClient(clientData, (string)senderIP);
            _serverRuntime.Log?.Write(LogType.Information, _machineIPAddress, $"[new client ip]: {senderIP}");
            return (ServerResponse.ProjectGlobalAttributes, _serverRuntime.ProjectData);
        }

        public (ServerResponse, object?) EnrollmentCanceled(object ob, object senderIP)
        {
            _serverRuntime.WorkflowService.RemoveClient((string)senderIP);
            return (ServerResponse.EnrollmentCanceled, null);
        }

        public (ServerResponse, object) Wait(object ob, object senderIP)
        {
            return (ServerResponse.Wait, 1);
        }

        public (ServerResponse, object?) HearbeatNoticed(object ob, object senderIP)
        {
            _serverRuntime.WorkflowService.AliveConfirmation((string)senderIP);
            return (ServerResponse.HearbeatNoticed, null);
        }

        public (ServerResponse, object?) Poison(object ob, object senderIP)
        {
            _serverRuntime.WorkflowService.RemoveClient((string)senderIP);
            return (ServerResponse.Poison, "project finished");
        }

       // private object lockerTasks = new object();
        public (ServerResponse, object?) NodeTaskData(object ob, object senderIP)
        {
           // lock (lockerTasks)
           // {

                if (_serverRuntime.NodeTaskService.IsEmpty())
                {

                    return (ServerResponse.Wait, _clientWaitOnTaskAvailabiltyMili);
                }

                var tasks = _serverRuntime.NodeTaskService.GetTaskRange((int)ob);

                ulong[] ids = tasks.Select(a => a.GetId()).ToArray();
                _serverRuntime.WorkflowService.SetInProgressTasks((string)senderIP, ids);

                // Console.WriteLine("end node task data" + (string)senderIP);
                return (ServerResponse.ProjectTaskData, tasks);
           // }
        }

        // incoming results from previos and send new
        public (ServerResponse, object?) NodeTaskDataNext(object ob, object senderIP)
        {
            //lock (lockerTasks)
           // {
                //Console.WriteLine("node task data" + (string)senderIP);
                //ob could be task response chek it here
                (List<NodeTask> nodeTask, int taskCountRequest) = ((List<NodeTask>, int))ob;
                if (nodeTask.Count > 0 && _serverRuntime.WorkflowService.IsValidReturn((string)senderIP))
                {
                    _serverRuntime.NodeTaskService.ReturnTaskRange(nodeTask);
                    _serverRuntime.WorkflowService.ReleaseInProgressTasks((string)senderIP);
                }

                if (_serverRuntime.NodeTaskService.IsEmpty())
                {
                    return (ServerResponse.Wait, _clientWaitOnTaskAvailabiltyMili);
                }
                var tasks = _serverRuntime.NodeTaskService.GetTaskRange(taskCountRequest);

                ulong[] ids = tasks.Select(a => a.GetId()).ToArray();
                _serverRuntime.WorkflowService.SetInProgressTasks((string)senderIP, ids);

                // Console.WriteLine("end node task data" + (string)senderIP);
                return (ServerResponse.ProjectTaskData, tasks);
           // }
        }

        public (ServerResponse, object?) ProjectPoisoning(object ob, object senderIP)
        {
            // remove from worklfow service
            _serverRuntime.WorkflowService.RemoveClient((string)senderIP);
            return (ServerResponse.Poison, "project finished");
        }

        public (ServerResponse, object?) RequestUnavailable(object ob, object senderIP)
        {
            return (ServerResponse.RequestUnavailable, _phase);
        }

        // end deleagtes
    }
}
