using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Class implements client device behaviour on task receive
    /// </summary>
    public class ClientFront
    {

#pragma warning disable CS8618 // Non-nullable field '_runtime' must contain a non-null value when exiting constructor. Consider declaring the field as nullable.
        private ClientRuntime _runtime;
#pragma warning restore CS8618 // Non-nullable field '_runtime' must contain a non-null value when exiting constructor. Consider declaring the field as nullable.

        public void Init(ClientRuntime runtime)
        {
            _runtime = runtime;
        }

        /// <summary>
        /// Method return available processor count on machine
        /// </summary>
        /// <returns>Processor machine count</returns>
        protected int GetProcessorCount()
        {
            return _runtime.ClientData.ProcessorCount;
        }

        /// <summary>
        /// Method call LogUser class implementation to print data
        /// </summary>
        /// <param name="values">objects to be printed</param>
        protected void Print(params object[] values)
        {
            _runtime.LogUser?.Print(values);
        }

        /// <summary>
        /// Method return globally defined attribute by key
        /// </summary>
        /// <param name="key">Key of attribute</param>
        /// <returns></returns>
        protected dynamic? GetGlobalAttribute(string key)
        {
            return _runtime?.ProjectData?.GetValue(key);
        }

        /// <summary>
        /// Method repord completed task for client async execution rule
        /// </summary>
        /// <param name="nodeTask"></param>
        protected void Finish(NodeTask nodeTask)
        {
            TaskComposer.Release(nodeTask);
        }
       
    }
}
