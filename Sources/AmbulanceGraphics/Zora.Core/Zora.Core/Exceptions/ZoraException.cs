using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zora.Core.Exceptions
{
    public class ZoraException : Exception
    {
        public ZoraResult Result {get;set;}
        public ZoraException(ZoraResult res)
        {
            Result = res;
        }
    }
}
