using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class implements server device behaviour while cluster system is launched
    /// </summary>
    public class ServerFront
    {
        private ulong _emitedTasksCount = 0;
#pragma warning disable CS8618 // Non-nullable field '_runtime' must contain a non-null value when exiting constructor. Consider declaring the field as nullable.
        private ServerRuntime _runtime;
#pragma warning restore CS8618 // Non-nullable field '_runtime' must contain a non-null value when exiting constructor. Consider declaring the field as nullable.

        private object _locker = new();

        public void Init(ServerRuntime serverRuntime)
        {
            _emitedTasksCount = 0;
            _runtime = serverRuntime;
        }

        /// <summary>
        /// MEthod send task to outcoming tasks queue
        /// </summary>
        /// <param name="nodeTask"></param>
        protected void SendTask(NodeTask nodeTask)
        {
            ulong emited = _runtime.NodeTaskService.AddTask(nodeTask);
            lock (_locker)
            {
                _emitedTasksCount+=emited;
            }
           
        }

        /// <summary>
        /// Method send tasks to outcoming tasks queue
        /// </summary>
        /// <param name="nodeTasks"></param>
        protected void SendTask(List<NodeTask> nodeTasks)
        {
            ulong emited = _runtime.NodeTaskService.AddTaskRange(nodeTasks);
            lock (_locker)
            {
                _emitedTasksCount += emited;
            }
           
        }

        /// <summary>
        /// Method return overall emited tasks in system
        /// </summary>
        /// <returns></returns>
        public ulong GetEmitedTasksCount()
        {
            lock (_locker)
            {
                return _emitedTasksCount;
            }
        }

        /// <summary>
        /// Method check if safe stop for server is possible
        /// </summary>
        /// <returns>true if is possible</returns>
        protected bool StopSafe()
        {
            if (!_runtime.IsOnStartRunning())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method call LogUser class implementation to print data
        /// </summary>
        /// <param name="values">objects to be printed</param>
        protected void Print(params object[] values)
        {
            _runtime.LogUser?.Print(values);
        }

        /// <summary>
        /// Method return globally defined attribute by key
        /// </summary>
        /// <param name="key">Key of attribute</param>
        /// <returns></returns>
        protected dynamic? GetGlobalAttribute(string key)
        {
            return _runtime?.ProjectData?.GetValue(key);
        }

    }
}
