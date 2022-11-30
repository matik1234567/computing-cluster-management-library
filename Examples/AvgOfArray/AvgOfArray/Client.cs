using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvgOfArray
{
    [TaskRule(TaskExecution.Async, Cores.Max, 5)]
    public class Client : ClientFront, IClient
    {
        public void OnInit()
        {
            
        }

        public void OnTasksReceive(ref List<NodeTask> nodeTasks)
        {
            foreach(NodeTask task in nodeTasks)
            {
                if(task is AvgTask avgTask)
                {
                    avgTask.Run();
                }
            }
        }

        public void OnTasksReceiveAsync(NodeTask nodeTask)
        {
            if (nodeTask is AvgTask avgTask)
            {
                avgTask.Run();
                Finish(avgTask);
            }
        }
    }
}
