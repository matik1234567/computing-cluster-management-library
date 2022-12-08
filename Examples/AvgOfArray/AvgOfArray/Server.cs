using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvgOfArray
{
    public class Server : ServerFront, IServer
    {
        private ulong received = 0;
        private double avg = 0;
        private Stopwatch stopwatch= new Stopwatch();

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
            stopwatch.Start();
            Random rnd = new Random();
            List<int> list = new List<int>();
            for(int i = 0; i < 1000000; i++)
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
            stopwatch.Stop();
            Print("time: ", stopwatch.ElapsedMilliseconds);
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
