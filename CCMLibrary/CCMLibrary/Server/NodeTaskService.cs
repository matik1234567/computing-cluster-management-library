using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class hold NodeTask object, queue incoming tasks and release proceeded 
    /// </summary>
    public class NodeTaskService
    {
        private ConcurrentQueue<NodeTask> _clusterTaskQueue = new ConcurrentQueue<NodeTask>();

        private OutcomingNodeTaskController _outNodeTaskController = new OutcomingNodeTaskController();
        private IncomingNodeTaskController _inNodeTaskController = new IncomingNodeTaskController();

        private ulong _taskLastId = 0;
        private Object _locker = new();
        
        public NodeTaskService() {}

        private ulong GetId()
        {
            lock (_locker)
            {
                return _taskLastId++;
            }
            
        }

        public ulong AddTask(NodeTask clusterTask)
        {
            if (clusterTask == null)
            {
                return 0;
            }
            clusterTask.SetId(GetId());
            _clusterTaskQueue.Enqueue(clusterTask);
            return 1;
        }

        public ulong AddTaskRange(List<NodeTask> clusterTasks)
        {
            ulong nulls = 0;
            foreach (NodeTask clusterTask in clusterTasks)
            {
                if(clusterTask == null)
                {
                    nulls++;
                    continue;
                }
                clusterTask.SetId(GetId());
                _clusterTaskQueue.Enqueue(clusterTask); 
            }
            return (ulong)clusterTasks.Count - nulls;
        }

        /// <summary>
        /// Method called by server on client tasks request
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<NodeTask> GetTaskRange(int range) 
        { 
            List<NodeTask> clusterTasks = new List<NodeTask>();
            while (range != 0 && _clusterTaskQueue.Count > 0)
            {
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                NodeTask task = null;
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                while(!_clusterTaskQueue.TryDequeue(out task)){ ; };
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
                _outNodeTaskController.Add(task);

                clusterTasks.Add(task);
                range--;
            }
            return clusterTasks;
        }

        /// <summary>
        /// Method called by server when tasks are returned from client
        /// </summary>
        /// <param name="nodeTasks"></param>
        public void ReturnTaskRange(List<NodeTask>nodeTasks)
        {
            foreach (NodeTask nodeTask in nodeTasks)
            {
                _inNodeTaskController.Add(nodeTask);
                _outNodeTaskController.Remove(nodeTask);
            }
        }

        public NodeTask TakeTask()
        {
            return _inNodeTaskController.Take();
        }

        public bool IsOutcomingEmpty()
        {
            return _outNodeTaskController.GetCount() == 0;    
        }

        public bool IsEmpty()
        {
            return _clusterTaskQueue.Count == 0;
        }

        public void ResendTasks(ulong[] iaskIds)
        {
            foreach(ulong i in iaskIds)
            {
                AddTask(_outNodeTaskController.Take(i));
            }
        }

        public bool IsNoTasks()
        {
            return _inNodeTaskController.IsEmpty() && _outNodeTaskController.IsEmpty() && this.IsEmpty();
        }

        public ulong GetTotalOutTaskCount()
        {
            return _outNodeTaskController.GetTotal();
        }

        public ulong GetTotalInTasksCount()
        {
            return _inNodeTaskController.GetTotal();
        }

        public void Reset()
        {
            _taskLastId = 0;
            _clusterTaskQueue.Clear();
            _inNodeTaskController.Reset();
            _outNodeTaskController.Reset();
        }
    }
}
