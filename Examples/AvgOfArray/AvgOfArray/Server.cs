using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvgOfArray
{
    public class Server : ServerFront, IServer
    {
        private ulong received = 0;
        private double avg = 0;

        public void OnReceive(NodeTask clusterTask)
        {
            if(clusterTask is AvgTask avgTask)
            {
                received++;
                avg = (avg + avgTask.avg) / 2;
            }
        }

        public void OnStart()
        {
            Random rnd = new Random();
            List<int> list = new List<int>();
            for(int i = 0; i < 10000; i++)
            {
                list.Add(rnd.Next(20));
                if (i!=0 && i%100 == 0)
                {
                    SendTask(new AvgTask(list));
                    list = new List<int>();
                }
            }
        }

        public void OnStop()
        {
            Print("Avg: ", avg);
        }

        public bool StopCondition()
        {
            if(GetEmitedTasksCount() == received)
            {
                return StopSafe();
            }
            return false;
        }
    }
}
