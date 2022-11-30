using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvgOfArray
{
    public class AvgTask : NodeTask
    {
        public List<int> Values;
        public double avg = 0;

        public AvgTask(List<int>values)
        {
            Values = values;
        }

        public void Run()
        {
            foreach(var item in Values)
            {
                avg += item;
            }
            avg /= Values.Count;
        }

    }
}
