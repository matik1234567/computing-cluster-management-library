using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    internal class ClusterSystemException : Exception
    {
        public ClusterSystemException() :base() { }

        public ClusterSystemException(string message) : base(message) { }
    }
}
