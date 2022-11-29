using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CCMLibrary.Enums;


namespace CCMLibrary
{
    public class ServerRuntime : Runtime
    {
        private BrokerServer? _brokerServer;
        public ProjectData ProjectData;
        public WorkflowService WorkflowService;
        public NodeTaskService NodeTaskService;
        public dynamic? ServerFront;
        public Log? Log;
        public LogUser? LogUser;

        private bool _onStartInProgress = false;

        public ServerRuntime(ProjectData projectData, Log? log, LogUser? logUser)
        {
            ProjectData = projectData;
            WorkflowService = new WorkflowService(this);
            NodeTaskService = new NodeTaskService();
            if(log != null)
            {
                Log = log;
            }
            else
            {
                Log = GetDefaultLog();
            }
            if(logUser != null)
            {
                LogUser = logUser;
            }
            else
            {
                LogUser = GetDefaultLogUser();
            }
        }

        public ServerRuntime(ProjectData projectData)
        {
            ProjectData = projectData;
            WorkflowService = new WorkflowService(this);
            NodeTaskService = new NodeTaskService();
            Log = GetDefaultLog();
            LogUser = GetDefaultLogUser();
        }

        public ServerRuntime()
        {
            ProjectData = new ProjectData();
            WorkflowService = new WorkflowService(this);
            NodeTaskService = new NodeTaskService();
            Log = GetDefaultLog();
            LogUser = GetDefaultLogUser();
        }

        public void Server(Int16 port)
        {
            if(Runtime.runtimeMode == RuntimeMode.Virtual)
            {
                Log?.Write(LogType.Information, "server", "Virtual mode set");
                _brokerServer = new BrokerServer(this);
            }
            else
            {
                _brokerServer = new BrokerServer(port, this);
            }
           
        }

        public void Server()
        {
            if(Runtime.runtimeMode == RuntimeMode.TCP)
            {
                Log?.Write(LogType.Exception, "server", "TCP mode set, consider change to Virtual");
                throw new RuntimeException("TCP runtime called as Virtual");
            }
            _brokerServer = new BrokerServer(this);
        }

        public async Task RunServerAsync()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

            await Task.Run(() => { _brokerServer.StartServer(); });
        }

        public void Invite()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

            _brokerServer.SendingProjectInvitation();
        }

        public async Task Invite(int expectedClients)
        {
            try
            {
                Invite();
            }
            catch (RuntimeException)
            {
                throw;
            }
            await Task.Run(() => {
                while (WorkflowService.GetCount(null)<expectedClients) { }
            });
        }

        public async Task RunTaskAsync()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

            _brokerServer.ProjectInProgress();
            await StartServerFrontAsync();
        }

        public void RunTask()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

            _brokerServer.ProjectInProgress();
            StartServerFront();
        }

        private async Task StartServerFrontInit()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            await Task.Run(() => { ServerFront.OnStart(); });
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            _onStartInProgress = false;
        }

        public bool IsOnStartRunning()
        {
            return _onStartInProgress;
        }

        private async Task StartServerFrontAsync()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

#pragma warning disable CS8604 // Possible null reference argument for parameter 'type' in 'object? Activator.CreateInstance(Type type)'.
            ServerFront = Activator.CreateInstance(Runtime.serverType);
#pragma warning restore CS8604 // Possible null reference argument for parameter 'type' in 'object? Activator.CreateInstance(Type type)'.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ServerFront.Init(this);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            // ServerFront = new ServerFront(this);
            var serverPhase = _brokerServer.GetServerPhase();
            if (serverPhase == ServerPhase.InProgress)
            {
                _onStartInProgress = true;
                _ = StartServerFrontInit();

                bool serverStoped = false;
                while (!serverStoped)
                {
                    if (_brokerServer.GetServerPhase() == ServerPhase.Freeze)
                    {
                        break;
                    }
                    NodeTask nodeTask = NodeTaskService.TakeTask();
                    if (nodeTask != null)
                    {
                        ServerFront.OnReceive(nodeTask);
                    }
                    serverStoped = ServerFront.StopCondition();
                }

                ServerFront.OnStop();
                await FreezeAsync();
            }
            else
            {
                throw new Exception($"Server in {serverPhase} phase");
            }

        }

        private void StartServerFront()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

#pragma warning disable CS8604 // Possible null reference argument for parameter 'type' in 'object? Activator.CreateInstance(Type type)'.
            ServerFront = Activator.CreateInstance(Runtime.serverType);
#pragma warning restore CS8604 // Possible null reference argument for parameter 'type' in 'object? Activator.CreateInstance(Type type)'.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            ServerFront.Init(this);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            // ServerFront = new ServerFront(this);
            var serverPhase = _brokerServer.GetServerPhase();
            if (serverPhase == ServerPhase.InProgress)
            {
                _onStartInProgress = true;
                _ = StartServerFrontInit();

                bool serverStoped = false;
                while (!serverStoped)
                {
                    if (_brokerServer.GetServerPhase() == ServerPhase.Freeze)
                    {
                        break;
                    }
                    NodeTask nodeTask = NodeTaskService.TakeTask();
                    if (nodeTask != null)
                    {
                        ServerFront.OnReceive(nodeTask);
                    }
                    serverStoped = ServerFront.StopCondition();
                }

                ServerFront.OnStop();
                Freeze();
            }
            else
            {
                throw new Exception($"Server in {serverPhase} phase");
            }

        }

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        public async Task StopAsync()
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

            _brokerServer.ProjectCancelation();
            /*
            await Task.Run(() => {
                while (!WorkflowService.IsEmpty())
                {
                    ;
                }
            });*/
            _brokerServer.CloseAllSockets();
            WorkflowService.Dispose();
        }


        public void Stop()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }

            _brokerServer.ProjectCancelation();
            _brokerServer.CloseAllSockets();
            WorkflowService.Dispose();
        }

        public async Task FreezeAsync()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }
            _brokerServer.ProjectFreeze();
            NodeTaskService.Reset();

            await Task.Run(() => {
                while (WorkflowService.IsInProgressTasks())
                {
                    ;
                }
            });
            WorkflowService.Reset();
        }

        public void Freeze()
        {
            if (_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }
            _brokerServer.ProjectFreeze();
            NodeTaskService.Reset();


            while (WorkflowService.IsInProgressTasks())
            {
                ;
            }
       
            WorkflowService.Reset();
        }

        public ulong GetEmitedTasksCount()
        {
            if(_brokerServer == null)
            {
                throw new RuntimeException(RuntimeException.ServerMissingEror);
            }
            if (ServerFront == null)
            {
                throw new RuntimeException("Server never used");
            }
            return ServerFront.GetEmitedTasksCount();
        }


    }
}
