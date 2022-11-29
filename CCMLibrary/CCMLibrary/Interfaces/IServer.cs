using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Interface define necessery method for server front implementation
    /// </summary>
    public interface IServer
    {
        /// <summary>
        /// Called when server front process start
        /// </summary>
        public void OnStart();

        /// <summary>
        /// Called each time response for NodeTask child class is returned
        /// </summary>
        /// <param name="clusterTask">NodeTask child class</param>
        public void OnReceive(NodeTask clusterTask);

        /// <summary>
        /// Called when server stop conndition is meet
        /// </summary>
        public void OnStop();

        /// <summary>
        /// Define stop condition, check each time after task is returned
        /// </summary>
        /// <returns>true when server should stop</returns>
        public bool StopCondition();
    }
}
