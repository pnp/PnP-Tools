using PSSQT.Helpers;
using SearchQueryTool.Model;
using System;

namespace PSSQT.ResultProcessor
{
    public abstract class AbstractQueryResultProcessor : IQueryResultProcessor
    {

        protected RetryHandler retryHandler;

        public AbstractQueryResultProcessor(SearchSPIndexCmdlet cmdlet)
        {
            this.Cmdlet = cmdlet;
            retryHandler = new RetryHandler(Cmdlet);
        }

        public SearchSPIndexCmdlet Cmdlet { get; private set; }

        public abstract void Process(SearchQueryResult searchQueryResult);

        public virtual void Configure()
        {
            UserNotification();

            EnsurePropertiesPresent();
        }

        protected virtual void EnsurePropertiesPresent()
        {
            // do nothing by default
        }

        protected virtual void UserNotification()
        {
            // do nothing by default
        }

        public virtual bool HandleException(Exception ex, SearchQueryRequest searchQueryRequest)
        {
            ExceptionHandler handler = ExceptionHandlerFactory.Create(Cmdlet, ex);

            retryHandler.DelegateHandler = handler;

            return retryHandler.HandleException(ex, searchQueryRequest);
        }


        public bool IsEndOfPipeline()
        {
            return Cmdlet.MyInvocation.PipelinePosition == Cmdlet.MyInvocation.PipelineLength;
        }

    }

}
