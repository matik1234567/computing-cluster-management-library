using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class define default front log output
    /// </summary>
    public class LogFrontDefault : LoggerFront
    {

        public override void Print(params object[] values)
        {
            string[] strings = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                try
                {
#pragma warning disable CS8601 // Possible null reference assignment.
                    strings[i] = Convert.ToString(values[i]);
#pragma warning restore CS8601 // Possible null reference assignment.
                }
                catch (Exception)
                {
                    strings[i] = "[parse error]";
                }
            }
            Console.WriteLine(String.Join("\t", strings));
        }
    }
}
