using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary
{
    /// <summary>
    /// Enum define library working mode
    /// </summary>
    public enum RuntimeMode { TCP, Virtual };

    /// <summary>
    /// Class allows to parametrize library
    /// </summary>
    public class Runtime
    {
        public static RuntimeMode runtimeMode = RuntimeMode.TCP;
        public static VirtualConnection? virtualConnection;
        protected static Type? clientType;
        protected static Type? serverType;
        protected static TaskRuleAttribute? taskRuleAttribute;

        public static void IntroduceFronted(Type serverType, Type clientType) 
        {
            if (clientType.GetInterface(nameof(IClient)) == null)
            {
                throw new ArgumentException("IClient is not implemented");
            }
            if (serverType.GetInterface(nameof(IServer)) == null)
            {
                throw new ArgumentException("IServer is not implemented");
            }
            Runtime.serverType = serverType;
            Runtime.clientType = clientType;
        }

        public static void SetClientAttributeDynamiclly(TaskRuleAttribute taskRule) 
        {
            taskRuleAttribute = taskRule;
        }

        public static void SetMode(RuntimeMode mode)
        {
            runtimeMode = mode;
        }

        protected Logger GetDefaultLog()
        {
            return new LogDefault(LogLevel.Info);
        }

        protected LoggerFront GetDefaultLogUser()
        {
            return new LogFrontDefault();
        }
    }
}
