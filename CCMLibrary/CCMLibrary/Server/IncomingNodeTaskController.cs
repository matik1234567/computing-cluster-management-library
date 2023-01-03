using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class represent incoming task from nodes
    /// </summary>
    internal class IncomingNodeTaskController
    {
        private Queue<NodeTask> _incomingTasks = new Queue<NodeTask>();

        private ulong _totalTaskCount = 0;

        private static object locker = new Object();


        public IncomingNodeTaskController() { }

        public void Add(NodeTask nodeTask)
        {
            lock (locker)
            {
                _incomingTasks.Enqueue(nodeTask);
                _totalTaskCount++;
            }
        }

        public NodeTask Take()
        {
            lock (locker)
            {
                if (_incomingTasks.Any())
                {
                    return _incomingTasks.Dequeue();
                }
            }
#pragma warning disable CS8603 // Possible null reference return.
            return null;
#pragma warning restore CS8603 // Possible null reference return.
        }

        public bool IsEmpty()
        {
            lock (locker)
            {
                return !_incomingTasks.Any();
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
                _incomingTasks.Clear();
            }
        }
    }
}
