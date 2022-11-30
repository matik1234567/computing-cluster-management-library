using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class represent single task handled by client,
    /// provide necessery attributes and methods to execute task,
    /// </summary>
    public class NodeTask
    {
        public ulong _id = 0; // unregistered  task
#pragma warning disable CS0414 // The field 'NodeTask._poison' is assigned but its value is never used
        private bool _poison = true;
#pragma warning restore CS0414 // The field 'NodeTask._poison' is assigned but its value is never used

        public long time = 0;

        public void SetId(ulong Id)
        {
            _id = Id;
        }

        public ulong GetId()
        {
            return _id;
        }
        
        
    }
}
