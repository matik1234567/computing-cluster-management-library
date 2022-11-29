using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary.Enums
{
    /// <summary>
    /// Enum class define server phase depend on server runtime configuration
    /// </summary>
    internal enum ServerPhase
    {
        Idle,
        SendingInvitation,
        InProgress,
        Cancelation,
        Freeze
    }
}
