using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zora.Core.Exceptions;

namespace Zora.Core.Exceptions
{
    public class ZoraResult
    {
        public Exception Exception { get; set; }
        public bool Success;
        public string Message;
        public string ErrorCodeMessage;
        public bool ShowMessageBox;
        public bool MakeErrorLog;
        public object Result;
        private Enum errorCode;
        //Exceptions.ErrorCodes errorCode;
        public bool WriteEventLog = false;
        public bool HasException
        {
            get
            {
                return (Exception != null);
            }
        }
        public Enum ErrorCode
        {
            get { return errorCode; }
            set
            {
                errorCode = value;
                ErrorCodeMessage = string.Format(" {0} Код на грешка {1}", EnumHelper.StringValueOf(errorCode), errorCode);
            }
        }
    }
}
