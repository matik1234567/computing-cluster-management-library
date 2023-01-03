using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class define based class for front logs
    /// </summary>
    public class LoggerFront
    {
        public virtual void Print(params dynamic[] values) { }
    }
}
