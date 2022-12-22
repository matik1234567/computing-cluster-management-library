using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    internal class OutcomingNodeTaskController
    {
        private Dictionary<ulong, NodeTask> _outcomingTasks = new Dictionary<ulong, NodeTask>();

        private static object locker = new Object();
        private ulong _totalTaskCount = 0;

        public OutcomingNodeTaskController() { }

        public void Add(NodeTask nodeTask)
        {
            lock (locker)
            {
                _outcomingTasks.Add(nodeTask.GetId(), nodeTask);
                _totalTaskCount++;  
            }
        }

        public void Remove(NodeTask nodeTask)
        {
            lock (locker)
            {
                _outcomingTasks.Remove(nodeTask.GetId());
            }
        }

        public NodeTask Take(ulong id)
        {
            lock (locker)
            {
                NodeTask nodeTask = _outcomingTasks[id];
                _outcomingTasks.Remove(id);
                return nodeTask;
            }
        }

        public int GetCount()
        {
            lock (locker)
            {
                return _outcomingTasks.Count;
            }
        }

        public bool IsEmpty()
        {
            lock (locker)
            {
                return !_outcomingTasks.Any();
            }
        }

        public ulong GetTotal()
        {
            lock (locker)
            {
                return _totalTaskCount;
            }
        }

        public void Reset()
        {
            lock (locker)
            {
                _totalTaskCount = 0;
                _outcomingTasks.Clear();
            }
        }
    }
}
