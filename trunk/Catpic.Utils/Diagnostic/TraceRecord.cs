using System;

namespace Catpic.Utils.Diagnostic
{
    /// <summary>
    /// Default implementation of DefaultTraceRecord
    /// </summary>
    public sealed class TraceRecord
    {
        public string TransactionId { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string Source { get; set; }
        public Exception Exception { get; set; }
        //public Type SourceType { get; set; }

        public TraceRecord()
        {
            Date = DateTime.MinValue;
        }
    }

}
