using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Collections.Concurrent;
using System.Windows.Threading;
using SearchQueryTool.Helpers;

namespace SearchQueryTool.Model
{
    public class SearchQueryDebug : INotifyPropertyChanged
    {
        private string _query;
        private string _template;

        //todo sort it by timestamp.
        private SafeObservable<string> _queryTemplateHelper;

        private string _queryExpanded1;
        private string _queryExpanded2;
        private string _personalResults;
        private string _relevantResults;
        private string _refinerResults;
        private string _hiddenConstraint;
        private string _siteSubscriptionId;
        private string _correlation;
        private string _querySuggestion;
        private string _queryExpanded3;

        //todo sort it by timestamp.
        private SafeObservable<String> _boundVariables;

        public SearchQueryDebug(string correlation, Dispatcher dispatcher)
        {
            Correlation = correlation;
            _boundVariables = new SafeObservable<string>(dispatcher);
            _queryTemplateHelper = new SafeObservable<string>(dispatcher);
        }

        public string Query
        {
            get { return _query; }
            set { _query = value; OnPropertyChanged("Query"); }
        }

        public string Template
        {
            get { return _template; }
            set { _template = value; OnPropertyChanged("Template"); }
        }

        public SafeObservable<string> QueryTemplateHelper
        {
            get { return _queryTemplateHelper; }
            set { _queryTemplateHelper = value; }
        }

        public string QueryExpanded1
        {
            get { return _queryExpanded1; }
            set { _queryExpanded1 = value; OnPropertyChanged("QueryExpanded1"); }
        }

        public string QueryExpanded2
        {
            get { return _queryExpanded2; }
            set { _queryExpanded2 = value; OnPropertyChanged("QueryExpanded2"); OnPropertyChanged("QueryExpanded2And3"); }
        }

        public string PersonalResults
        {
            get { return _personalResults; }
            set { _personalResults = value; OnPropertyChanged("PersonalResults"); }
        }

        public string RelevantResults
        {
            get { return _relevantResults; }
            set { _relevantResults = value; OnPropertyChanged("RelevantResults"); }
        }

        public string RefinerResults
        {
            get { return _refinerResults; }
            set { _refinerResults = value; OnPropertyChanged("RefinerResults"); }
        }

        public string HiddenConstraint
        {
            get { return _hiddenConstraint; }
            set { _hiddenConstraint = value; OnPropertyChanged("HiddenConstraint"); }
        }

        public string SiteSubscriptionId
        {
            get { return _siteSubscriptionId; }
            set { _siteSubscriptionId = value; OnPropertyChanged("SiteSubscriptionId"); }
        }

        public string Correlation
        {
            get { return _correlation; }
            set { _correlation = value; OnPropertyChanged("Correlation"); }
        }

        public string QuerySuggestion
        {
            get { return _querySuggestion; }
            set { _querySuggestion = value; OnPropertyChanged("QuerySuggestion"); }
        }

        public string QueryExpanded3
        {
            get { return _queryExpanded3; }
            set { _queryExpanded3 = value; OnPropertyChanged("QueryExpanded3"); OnPropertyChanged("QueryExpanded2And3"); }
        }

        public string QueryExpanded2And3
        {
            get { return string.Format("{0}{1}", QueryExpanded2, QueryExpanded3); }
            set { throw new InvalidOperationException("Set not allowed");}
        }

        public SafeObservable<String> BoundVariables
        {
            get { return _boundVariables; }
            set { _boundVariables = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Query: {0} \r\n", Query);
            sb.AppendFormat("Template: {0}\r\n", Template);
            sb.AppendFormat("Hidden Constraint: {0}\r\n", HiddenConstraint);
            sb.AppendFormat("SiteSubscriptionId: {0}\r\n", SiteSubscriptionId);
            sb.AppendFormat("Relevant Results: {0}\r\n", RelevantResults);
            sb.AppendFormat("Personal Results: {0}\r\n", PersonalResults);
            sb.AppendFormat("Refiner Results: {0}\r\n", RefinerResults);
            sb.AppendFormat("Query Suggestions: {0}\r\n", QuerySuggestion);
            sb.AppendFormat("Query Expansion 1: {0}\r\n", QueryExpanded1);
            sb.AppendFormat("Query Expansion 2: {0}{1}\r\n", QueryExpanded2, QueryExpanded3);

            return sb.ToString();
        }
    }
}
