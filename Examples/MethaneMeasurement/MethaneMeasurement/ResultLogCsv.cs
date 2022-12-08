using CCMLibrary;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MethaneMeasurement
{
    internal class ResultLogCsv : LoggerFront
    {
        private StringBuilder _stringBuilder = new StringBuilder();
        public ResultLogCsv()
        {
            _stringBuilder.AppendLine("month-hour-day;average;variance");
        }

        public override void Print(params dynamic[] values)
        {
            _stringBuilder.AppendLine($"{values[0]};{values[1]};{values[2]}");
        }

        public void SaveResults()
        {
            File.WriteAllText("resuklts.csv", _stringBuilder.ToString());
        }
    }
}
