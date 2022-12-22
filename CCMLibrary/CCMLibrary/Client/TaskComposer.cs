using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class allow client to execute tasks when execution rule is set to async,
    /// Guard available threads count
    /// </summary>
    internal class TaskComposer
    {
        private volatile static int _availableResources = 0;
        private volatile static List<NodeTask> _tasks = new List<NodeTask>();
        private static object locker = new object();

        public static void Init(int maxResources)
        {
            _availableResources = maxResources;
            _tasks = new List<NodeTask>();
        }

        public static int GetAvailableResources()
        {
            //lock (locker)
            //{
                return _availableResources;
            //}
        }

        public static void Hold()
        {
            lock (locker)
            {
                _availableResources--;
            }
           
        }

        public static void Release(NodeTask nodeTask)
        {
            lock (locker)
            {
                _availableResources++;
                _tasks.Add(nodeTask);
            }
            
        }

        public static List<NodeTask> GetCompletedTasks()
        {
            return _tasks;
        }
    }
}
