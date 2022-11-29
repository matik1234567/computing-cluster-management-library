using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Interface define necessery methods for client front implementation
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Called once on client front start
        /// </summary>
        public void OnInit();

        /// <summary>
        /// Called each time new task is received and execution rule is set to sync
        /// </summary>
        /// <param name="nodeTasks"></param>
        public void OnTasksReceive(ref List<NodeTask> nodeTasks);

        /// <summary>
        /// Called each time new task is received and execution rule is set to async
        /// </summary>
        /// <param name="nodeTask"></param>
        public void OnTasksReceiveAsync(NodeTask nodeTask);
    }
}
