using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace CCMLibrary
{
    /// <summary>
    /// Class hold object of client process and allow to manipulate them on runtime
    /// </summary>
    public class ClientRuntime : Runtime
    {
        private Client? _client;
        private ClientConnectionData? _connectionData;

        public ProjectData? ProjectData;
        public ClientData ClientData;
        public dynamic? ClientFront;
        public Log? Log;
        public LogUser? LogUser;

        private bool _isRunning = false;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="log">user defined Log class</param>
        /// <param name="logUser">user defined UserLog class</param>
        public ClientRuntime(Log? log, LogUser? logUser)
        {
            if(log == null)
            {
                Log = GetDefaultLog();
            }
            else
            {
                Log = log;
            }

            if(logUser == null)
            {
                LogUser = GetDefaultLogUser();
            }
            else
            {
                LogUser = logUser;
            }

            ClientData = new ClientData();
        }

        /// <summary>
        /// Constructor create ClientRuntime with default logging
        /// </summary>
        public ClientRuntime()
        {
            Log = GetDefaultLog();
            LogUser = GetDefaultLogUser();
            ClientData = new ClientData();
        }

        /// <summary>
        /// Create client for both TCP and Virtual Modes
        /// </summary>
        /// <param name="serverIp"></param>
        /// <param name="serverPort"></param>
        public void Client(string serverIp, Int16 serverPort)
        {
            if (Runtime.runtimeMode == RuntimeMode.Virtual)
            {
                Log?.Write(LogType.Information, "client", "Virtual mode set");
                Client();
            }
            else
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _connectionData = new ClientConnectionData { ClientSocket = socket, ServerHostName = serverIp, ServerPort = serverPort };
                _client = new Client(_connectionData, this);
            }
           
        }

        /// <summary>
        /// Create cleint for Virtual Mode
        /// </summary>
        /// <exception cref="RuntimeException"></exception>
        public void Client()
        {
            if (Runtime.runtimeMode == RuntimeMode.TCP)
            {
                Log?.Write(LogType.Exception, "client", "TCP mode set, consider change to Virtual");
                throw new RuntimeException("TCP runtime called as Virtual");

            }
            _ = new VirtualConnection();
            _connectionData = null;
            _client = new Client(this);
        }

        /// <summary>
        /// Find defined TaskRule for CleintFront class
        /// </summary>
        /// <returns>TaskRuleAttribute if exist else default</returns>
        public TaskRuleAttribute GetTasksRule()
        {
            Attribute[] attrs = Attribute.GetCustomAttributes(this.ClientFront?.GetType());

            foreach (Attribute attr in attrs)
            {
                if (attr is TaskRuleAttribute)
                {
                    return (TaskRuleAttribute)attrs[0];
                }
            }
            return new TaskRuleAttribute();
        }

        /// <summary>
        /// Enroll to project asynchonously
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RuntimeException"></exception>
        public async Task EnrollToProjectAsync()
        {
            if (_client == null)
            {
                throw new RuntimeException(RuntimeException.ClientMissingEror);
            }

            await Task.Run(() =>
            {
                try
                {
                    _client?.EnrollToProject();
                }
                catch (Exception)
                {
                    throw;
                }

            });
        }

        /// <summary>
        /// Enroll to project 
        /// </summary>
        /// <exception cref="RuntimeException"></exception>
        public void EnrollToProject()
        {
            if (_client == null)
            {
                throw new RuntimeException(RuntimeException.ClientMissingEror);
            }

            try
            {
                _client?.EnrollToProject();
            }
            catch (Exception)
            {
                throw;
            }  
        }

        /// <summary>
        /// Start Client requesting process
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RuntimeException"></exception>
        public async Task RunClient()
        {
            
            if (_client == null)
            {
                throw new RuntimeException(RuntimeException.ClientMissingEror);
            }

#pragma warning disable CS8604 // Possible null reference argument for parameter 'type' in 'object? Activator.CreateInstance(Type type)'.
            ClientFront = Activator.CreateInstance(Runtime.clientType);
#pragma warning restore CS8604 // Possible null reference argument for parameter 'type' in 'object? Activator.CreateInstance(Type type)'.
            ClientFront?.Init(this);
           
            _isRunning = true;

#pragma warning disable CS0168 // The variable 'e' is declared but never used
            try
            {
                await Task.Run(() => { ClientFront?.OnInit(); });
                await Task.Run(() => { _client?.RunClient(); });
                
            }
            catch(Exception e)
            {
                return;
            }
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            _isRunning = false;
        }

        /// <summary>
        /// Stop client process
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RuntimeException"></exception>
        public async Task StopClientAsync()
        {
            if (_client == null)
            {
                throw new RuntimeException(RuntimeException.ClientMissingEror);
            }

            _client.RequestCancellation();
            _connectionData?.ClientSocket.Close();
            await Task.Run(() => { while (_isRunning) { ; } });
        }

        /// <summary>
        /// Stop client process
        /// </summary>
        /// <returns></returns>
        /// <exception cref="RuntimeException"></exception>
        public void StopClient()
        {
            if (_client == null)
            {
                throw new RuntimeException(RuntimeException.ClientMissingEror);
            }

            _client.RequestCancellation();
            _connectionData?.ClientSocket.Close();
            while (_isRunning) {; }
        }

    }
}
