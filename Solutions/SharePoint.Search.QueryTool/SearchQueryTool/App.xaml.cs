using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SearchQueryTool
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            AddExceptionEventHandler();
        }

        private void AddExceptionEventHandler()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            SetupExceptionHandling();
        }

        private void SetupExceptionHandling()
        {
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
                LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

            Dispatcher.UnhandledException += (s, e) =>
                LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");

            TaskScheduler.UnobservedTaskException += (s, e) =>
                LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
        }

        private void LogUnhandledException(Exception exception, string source)
        {            
            if (exception != null)
            {
                //string terminatingMsg = args.IsTerminating ? "\n\nThe application will be terminated." : "";
                MessageBox.Show("Error occured!\n\n" + exception.Message);

            }

            //string message = $"Unhandled exception ({source})";
            //try
            //{
            //    System.Reflection.AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            //    message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            //}
            //catch (Exception ex)
            //{
            //    _logger.Error(ex, "Exception in LogUnhandledException");
            //}
            //finally
            //{
            //    _logger.Error(exception, message);
            //}
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = args.ExceptionObject as Exception;
            if (e != null)
            {
                string terminatingMsg = args.IsTerminating ? "\n\nThe application will be terminated." : "";
                MessageBox.Show("Error occured!\n\n" + e.Message + terminatingMsg);

            }
        }
    }
}
