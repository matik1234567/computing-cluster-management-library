using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MethaneMeasurement
{
    internal class VarTask : NodeTask
    {
        public List<double> Values;
        public string MonthDayHour;

        public double Avg;
        public double Var;

        public VarTask(string montDayHour, List<double>values) 
        { 
            Values = values;
            MonthDayHour = montDayHour;
        }

        public void Run()
        {
            double sum = 0;
            foreach (double value in Values)
            {
                sum += Math.Pow(value - Avg, 2);
            }
            Var = sum/ Values.Count;
            Values = null; // clear list after task finished to speed up package send over tcp
        }
    }
}
