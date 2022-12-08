using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethaneMeasurement
{
    //[TaskRule(TaskExecution.Async, 2, 10)]
    [TaskRule(TaskExecution.Sync,50)]
    public class Client : ClientFront, IClient
    {
        public void OnInit() {}

        public void OnTasksReceive(ref List<NodeTask> nodeTasks)
        {
            foreach (NodeTask task in nodeTasks)
            {
                if (task is AvgTask avgTask)
                {
                    avgTask.Run();
                }
                else if(task is VarTask varTask)
                {
                    varTask.Run();
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
            else if(nodeTask is VarTask varTask)
            {
                varTask.Run();
                Finish(varTask);
            }
        }
    }
}
