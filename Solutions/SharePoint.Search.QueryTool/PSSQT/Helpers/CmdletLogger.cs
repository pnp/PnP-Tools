using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace PSSQT.Helpers
{
    class CmdletLogger : ILogger
    {
        public PSCmdlet Cmdlet { get; set; }

        public CmdletLogger(PSCmdlet cmdlet)
        {
            Cmdlet = cmdlet;
        }

        // throws an exception if loglevel is error
        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                 case LogLevel.Debug:
                    Cmdlet.WriteDebug(message);
                    break;
                case LogLevel.Verbose:
                    Cmdlet.WriteVerbose(message);
                    break;
                case LogLevel.Warning:
                    Cmdlet.WriteWarning(message);
                    break;
                case LogLevel.Error:
                    throw new Exception(message);

                default:
                    break;
            }
        }
    }
}
