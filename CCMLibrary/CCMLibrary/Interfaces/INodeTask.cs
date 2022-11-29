using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Interface define neccesery method for NodeTask entry point
    /// </summary>
    public interface INodeTask
    {
        /// <summary>
        /// Define entry point for task execution
        /// </summary>
        public void Run();
    }
}
