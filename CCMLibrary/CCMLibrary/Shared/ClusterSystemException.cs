using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class inherit from Exception class, define unpredictable system behaviour
    /// Critical error
    /// </summary>
    internal class ClusterSystemException : Exception
    {
        public ClusterSystemException() :base() { }

        public ClusterSystemException(string message) : base(message) { }
    }
}
