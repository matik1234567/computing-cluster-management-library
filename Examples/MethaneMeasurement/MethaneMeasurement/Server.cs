using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace MethaneMeasurement
{
    public class Server : ServerFront, IServer
    {
        int summary = 0;

        ulong receivedAvgTasks = 0;
        ulong receivedVarTasks = 0;

        private Dictionary<string, (double, double?)> resultsByHour = new Dictionary<string, (double, double?)>();
        private Dictionary<string, VarTask> varianceTasks = new Dictionary<string, VarTask>();

        private Stopwatch stopwatch = new Stopwatch();

        public void OnStart()
        {
            stopwatch.Start();
            using (StreamReader sr = new StreamReader(GetGlobalAttribute("dataDirectory")))
            {
                string currentLine;
                bool first = true;

                List<double> dailyMethane252 = new List<double>();
                string prevDate = "";

                while ((currentLine = sr.ReadLine()) != null)
                {
                    if (first) // skip header
                    {
                        first = false;
                    }
                    else
                    {
                        string[] values = currentLine.Split(',');
                        string month_day_hour = $"{values[1]}-{values[2]}-{values[3]}";//month-day-hour
                       
                        if (prevDate != month_day_hour)
                        {
                            if (dailyMethane252.Count > 0)
                            {
                                varianceTasks.Add(prevDate, new VarTask(prevDate, dailyMethane252)); //need avg value, wait untill avgTask finish
                                SendTask(new AvgTask(prevDate, dailyMethane252));
                            }
                            dailyMethane252 = new List<double>();
                            prevDate = month_day_hour;
                        }
                        string methane252 = values[15];
                        double m = Double.Parse(methane252, CultureInfo.InvariantCulture);
                        dailyMethane252.Add(m);
                    }
                }
            }
            
        }

        public void OnReceive(NodeTask clusterTask)
        {
    
            if (clusterTask is AvgTask avgTask)
            {
                resultsByHour.Add(avgTask.MonthDayHour, (avgTask.Avg, null));
                VarTask varTask = varianceTasks[avgTask.MonthDayHour];
                varTask.Avg = avgTask.Avg;
                SendTask(varTask);
                receivedAvgTasks++;
            }
            else if(clusterTask is VarTask varTask)
            {
                var res = resultsByHour[varTask.MonthDayHour];
                resultsByHour[varTask.MonthDayHour] = (res.Item1, varTask.Var);
                receivedVarTasks++;
            }
        }

        public void OnStop()
        {
            stopwatch.Stop();
            Console.WriteLine(stopwatch.ElapsedMilliseconds);
            foreach (var item in resultsByHour)
            {
                Print(item.Key, item.Value.Item1, item.Value.Item2);
            }

        }

        public bool StopCondition()
        {
            if (GetEmitedTasksCount() == receivedAvgTasks + receivedVarTasks)
            {
                return StopSafe();
            }
            return false;
        }
    }
}
