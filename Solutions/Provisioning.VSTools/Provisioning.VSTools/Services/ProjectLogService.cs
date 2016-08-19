using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Provisioning.VSTools.Services
{
    public class ProjectLogService : Provisioning.VSTools.Services.ILogService
    {
        private const string MESSAGE_FORMAT = "({0}) {1}\n";

        private OutputWindowPane outputWindowPane { get; set; }

        /// <summary>
        /// Sets the reference to the visual studio output window pane so that the logger can output messages to this output pane.
        /// </summary>
        internal void SetOutputWindowPane(OutputWindowPane pane)
        {
            this.outputWindowPane = pane;
        }

        private bool validate()
        {
            if (outputWindowPane != null)
            {
                return true;
            }
            else
            {
                System.Diagnostics.Trace.TraceWarning("OutputWindowPane not initialized, cannot route output to Visual Studio output window.");
                return false;
            }
        }

        public void Info(string message)
        {
            if (validate())
            {
                string msg = string.Format(MESSAGE_FORMAT, "Info", message);
                WriteOutput(msg);
            }
        }

        public void Info(string message, params object[] args)
        {
            if (validate())
            {
                string msg = string.Format(MESSAGE_FORMAT, "Info", string.Format(message, args));
                WriteOutput(msg);
            }
        }

        public void Warn(string message)
        {
            if (validate())
            {
                string msg = string.Format(MESSAGE_FORMAT, "Warning", message);
                WriteOutput(msg);
            }
        }

        public void Warn(string message, params object[] args)
        {
            if (validate())
            {
                string msg = string.Format(MESSAGE_FORMAT, "Warning", string.Format(message, args));
                WriteOutput(msg);
            }
        }

        public void Error(string message)
        {
            if (validate())
            {
                string msg = string.Format(MESSAGE_FORMAT, "Error", message);
                WriteOutput(msg);
            }
        }

        public void Error(string message, params object[] args)
        {
            if (validate())
            {
                string msg = string.Format(MESSAGE_FORMAT, "Error", string.Format(message, args));
                WriteOutput(msg);
            }
        }

        public void Exception(string message, Exception ex)
        {
            string name = System.Reflection.Assembly.GetCallingAssembly().GetName().Name;

            System.Diagnostics.Debug.WriteLine(message, name);
            System.Diagnostics.Debug.WriteLine(string.Format("Exception: {0}\n{1}", ex.Message, ex.StackTrace), name);

#if DEBUG
            this.Error("{0}, exception details: {1}\n{2}", message, ex.Message, ex.StackTrace);
#else
            this.Error("{0}, exception details: {1}", message, ex.Message);
#endif
        }

        private void WriteOutput(string message)
        {
            try
            {
                if (outputWindowPane != null)
                {
                    outputWindowPane.OutputString(message);
                }
            }
            catch
            {
                System.Diagnostics.Trace.TraceWarning("(outputWindowPane not available) " + message);
            }
        }
    }
}
