using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Services
{
    public class GenericLogService : Provisioning.VSTools.Services.ILogService
    {
        public void Info(string message)
        {
            System.Diagnostics.Trace.TraceInformation(message);
        }
        public void Warn(string message)
        {
            System.Diagnostics.Trace.TraceWarning(message);
        }
        public void Error(string message)
        {
            System.Diagnostics.Trace.TraceError(message);
        }

        public void Warn(string message, params object[] args)
        {
            System.Diagnostics.Trace.TraceWarning(message, args);
        }

        public void Error(string message, params object[] args)
        {
            System.Diagnostics.Trace.TraceError(message, args);
        }

        public void Info(string message, params object[] args)
        {
            System.Diagnostics.Trace.TraceInformation(message, args);
        }

        public void Exception(string message, Exception ex)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("{0}, exception details: {1}, {2}", message, ex.Message, ex.StackTrace));
        }
    }
}
