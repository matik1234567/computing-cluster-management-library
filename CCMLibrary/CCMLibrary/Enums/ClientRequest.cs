using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCMLibrary.Enums
{
    /// <summary>
    /// Enum class defne message header on client TCP request
    /// </summary>
    public enum ClientRequest
    {
        EnrollProject,
        CancelEnrollment,
        TaskRequest,
        TaskResultsReturnRequest,
        HearbeatConfirmation,
        None,
        Virtual
    }
}
