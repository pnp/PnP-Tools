using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Just create a very specific minimalistic logger interface so that I can log by accessing a static logger on the Cmdlet

namespace PSSQT.Helpers
{
    public enum LogLevel
    {
        Debug,
        Verbose,
        Warning,
        Error,
        Critical,
        None
    }

    interface ILogger
    {

        void Log(LogLevel level, string message);
    }
}
