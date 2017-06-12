using System;
using System.Runtime.Serialization;

namespace PSSQT
{
    [Serializable]
    internal class RankDetailTooManyResults : Exception
    {
        public string QueryFilter { get; set; }

        public RankDetailTooManyResults()
        {
        }

        public RankDetailTooManyResults(string message) : base(message)
        {
        }

        public RankDetailTooManyResults(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected RankDetailTooManyResults(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}