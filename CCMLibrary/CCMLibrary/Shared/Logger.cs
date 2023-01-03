using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Enum define level of log visiblity
    /// </summary>
    public enum LogLevel { Info, Debug};

    /// <summary>
    /// Enum define type of outcoming log
    /// </summary>
    public enum LogType { Exception, Information }

    /// <summary>
    /// Class define base class for log outputs
    /// </summary>
    public class Logger
    {
        public LogLevel LogLevel;

        public Logger(LogLevel logLevel)
        {
            LogLevel = logLevel;
        }

        public virtual void Write(LogType type, string source, string message) { }

    }
}
