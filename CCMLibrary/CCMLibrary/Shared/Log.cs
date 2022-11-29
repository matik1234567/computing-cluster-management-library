using System;
using System.Net;

namespace CCMLibrary
{
    public enum LogType { Exception, Information }

    public class Log
    {
        protected Action<LogType, string, string, string> _onWriteActiom;

        protected static string _logFormat = "yyyy/MM/dd HH:mm:ss.ff";

        public Log(Action<LogType, string, string, string> onWrite) 
        {
            _onWriteActiom = onWrite;
        }

        protected string GetTimestamp()
        {
            return DateTime.Now.ToString(_logFormat);
        }

        public void Write(LogType type, string source, string message)
        {
            _onWriteActiom(type, source, GetTimestamp(), message);
        }

        public  void Write(LogType type, string source, string[] message)
        {
            _onWriteActiom(type, source, GetTimestamp(), String.Join(";", message));
        }

    }
}
