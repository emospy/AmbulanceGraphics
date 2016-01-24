using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Zora.Core.Exceptions
{
    public class ZoraEventLog
    {
        public static void WriteEventLog(Exception exc, string message = "", string eventLogName = Constants.EventLogName)
        {

            try
            {
                if (EventLog.SourceExists(Constants.EventLogName) == true)
                {
                    EventLog myLog = new EventLog();
                    myLog.Source = Constants.EventLogName;
                    myLog.WriteEntry(string.Format("Dev Message {0} ErrorMessage: {1}  \n\nStackTrace: {2}", message, exc.Message, exc.StackTrace), EventLogEntryType.Error);
                    myLog.Close();
                }


            }
            catch (Exception ex)
            {
            }

        }
    }
}
