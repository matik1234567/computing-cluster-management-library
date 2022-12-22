using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary.Enums
{
    /// <summary>
    /// Enum class define possible server response message header
    /// </summary>
    public enum ServerResponse
    {
        ProjectGlobalAttributes,
        EnrollmentCanceled,
        ProjectTaskData,
        Poison,
        RequestUnavailable,
        SendClientConfig,
        Null,
        Wait,
        HearbeatNoticed,
        Virtual
    }
}
