using System;
using System.Net;

namespace CCMLibrary
{

    public class LogDefault : Logger
    {

        private static string _logFormat = "yyyy/MM/dd HH:mm:ss.ff";

        public LogDefault(LogLevel logLevel) : base(logLevel) { }

        private string GetTimestamp()
        {
            return DateTime.Now.ToString(_logFormat);
        }

        public override void Write(LogType type, string source, string message)
        {
            Console.WriteLine(String.Join("\t", new string[] { type.ToString(), GetTimestamp(), source, message }));
        }

    }
}
