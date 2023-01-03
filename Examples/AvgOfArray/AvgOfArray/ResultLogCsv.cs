using CCMLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvgOfArray
{
    internal class ResultLogCsv : LoggerFront
    {
        private StringBuilder _stringBuilder = new StringBuilder();

        public ResultLogCsv() {}

        public override void Print(params dynamic[] values)
        {
            _stringBuilder.AppendLine($"{values[0]};{values[1]}");
        }

        public void SaveResults()
        {
            File.WriteAllText("results.csv", _stringBuilder.ToString());
        }
    }
}
