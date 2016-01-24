using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Zora.Core.Exceptions;

namespace Zora.Core.Logic
{
    public class CoreLogic
    {
        //protected DbContext data;
        public ZoraResult Result = new ZoraResult();

        public virtual string MessageInstruction
        {
            get { return messageInstruction; }
            set { messageInstruction = value; }
        }

        public virtual string MessageStatus
        {
            get { return messageStatus; }
            set { messageStatus = value; }
        }

        /// <summary>
        /// Throws exception of Zora Type
        /// </summary>
        /// <param name="erroCode"></param>
        /// <param name="result"></param>
        protected void ThrowZoraException(Enum erroCode, bool result = false, string overrideMessage = null, bool ShowBlockingMessageBox = false)
        {
            Result = new ZoraResult();
            Result.ErrorCode = erroCode;
            Result.Result = result;
            Result.ShowMessageBox = ShowBlockingMessageBox;
            if (overrideMessage != null)
            {
                Result.ErrorCodeMessage = overrideMessage;
            }
            throw new ZoraException(Result);
        }

        private string messageInstruction;
        private string messageStatus;
    }
}
