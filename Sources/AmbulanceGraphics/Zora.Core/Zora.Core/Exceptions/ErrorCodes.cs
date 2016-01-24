using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Zora.Core.Exceptions
{
    public enum ErrorCodesCore
    {
        [Description("OK")]
        NoError = 0,
        [Description("Няма връзка с базата данни!")]
        NoDb = 1,

    }
}
