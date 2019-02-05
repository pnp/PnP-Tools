using SearchQueryTool.Model;
using System;
using System.Threading;

namespace PSSQT.Helpers
{
    public interface ExceptionHandler
    {
        int MaxAttempts { get; }

        bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest);

        int SleepBetweenAttemps(int attempt);   // in milliseconds
    }

    public static class ExceptionHandlerFactory
    {
        public static ExceptionHandler Create(SearchSPIndexCmdlet cmdlet, Exception ex)
        {
            ExceptionHandler handler = null;

            if (ex.Message.Contains("HTTP 401"))
            {
                handler = new HTTP401ExceptionHandler(cmdlet);
            }
            else if (ex.Message.Contains("An existing connection was forcibly closed by the remote host"))
            {
                handler = new ForciblyClosedExceptionHandler(cmdlet);
            }
            else
            {
                handler = new DefaultExceptionHandler();
            }

            return handler;
        }
    }

    public abstract class AbstractExceptionHandler : ExceptionHandler
    {
        public virtual int MaxAttempts =>  1;

        public virtual int SleepBetweenAttemps(int attempt)
        {
            return 0;
        }

        public abstract bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest);
    }

    public class DefaultExceptionHandler : AbstractExceptionHandler
    {
        public override bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {
            return false;
        }
    }

    public abstract class BaseExceptionHandler : AbstractExceptionHandler
    {
        protected SearchSPIndexCmdlet Cmdlet { get; private set; }

        public BaseExceptionHandler(SearchSPIndexCmdlet cmdlet)
        {
            Cmdlet = cmdlet;
        }

    }

    public class RetryHandler : BaseExceptionHandler
    {
        private int attempt = 0;
 
        public ExceptionHandler DelegateHandler { get; set; }

        public RetryHandler(SearchSPIndexCmdlet cmdlet) : base(cmdlet)
        {
        }

        public override bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {
            if (DelegateHandler != null)
            {
                if (attempt++ < DelegateHandler.MaxAttempts)
                {
                    int sleepFor = DelegateHandler.SleepBetweenAttemps(attempt);

                    if (sleepFor > 0)
                    {
                        Thread.Sleep(sleepFor);
                    }

                    return DelegateHandler.HandleException(ex, searchQueryRequest);
                }
                else
                {
                    RetryExhausted(ex);
                }

            }

            return false;
        }



        protected virtual void RetryExhausted(Exception ex)
        {
            Cmdlet.WriteVerbose("Max number of attempts exhausted. Giving up.");
        }

    }

    public class HTTP401ExceptionHandler : BaseExceptionHandler
    {
        public HTTP401ExceptionHandler(SearchSPIndexCmdlet cmdlet) : base(cmdlet)
        {
        }

        public override bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {

            switch (searchQueryRequest.AuthenticationType)
            {
                case AuthenticationType.SPO:
                    UserNotification();
                    Cmdlet.SPOLegacyLogin(searchQueryRequest);
                    break;

                case AuthenticationType.SPOManagement:
                    UserNotification();
                    SearchSPIndexCmdlet.AdalLogin(searchQueryRequest, false);
                    break;

                default:
                    return false;
            }

            return true;
        }

        protected virtual void UserNotification()
        {
            Cmdlet.WriteVerbose("Received HTTP 401. Retrying authentication...");
        }
    }

    public class ForciblyClosedExceptionHandler : BaseExceptionHandler
    {
        private static readonly int sleepMs = 30000;

        public ForciblyClosedExceptionHandler(SearchSPIndexCmdlet cmdlet) : base(cmdlet)
        {
        }

        public override bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {
            return true;
        }

        public override int MaxAttempts => 3;

        public override int SleepBetweenAttemps(int attempt)
        {
            return attempt * sleepMs;
        }

        //cmdlet.WriteWarning($"Connection forcibly closed by remote host. Backing off {sleepFor} ms before retrying. ({RetryAttempDescription})");

    }
}
