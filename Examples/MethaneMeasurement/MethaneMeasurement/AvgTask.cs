using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethaneMeasurement
{
    public class AvgTask : NodeTask
    {
        public string MonthDayHour;
        public List<double> Values;

        public double Avg = 0;

        public AvgTask(string monthDayHour, List<double> values)
        {
            this.MonthDayHour = monthDayHour;
            this.Values = values;
        }

        public void Run()
        {
            foreach (var item in Values)
            {
                Avg += item;
            }
            Avg /= Values.Count;
            Values = null; // clear list after task finished to speed up package send over tcp
        }
    }
}
