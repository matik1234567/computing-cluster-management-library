using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    public enum LogLevel { Info, Debug};

    public enum LogType { Exception, Information }

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
