using System;
using System.Diagnostics;

namespace Catpic.Utils.Diagnostic
{
    /// <summary>
    /// Empty trace. Used if another trace isn't provided or hasn't created
    /// </summary>
    public class EmptyTrace : ITrace
    {
        public int Level { get; set; }

        public void Debug(string message)
        {

        }

        public void Debug(string category, string message)
        {

        }

        public void Debug(TraceRecord record)
        {

        }

        public void Info(string message)
        {

        }

        public void Info(string category, string message)
        {

        }

        public void Info(TraceRecord record)
        {

        }

        public void Warn(string message)
        {

        }

        public void Warn(string category, string message)
        {

        }

        public void Warn(TraceRecord record)
        {

        }

        public void Error(string message, Exception exception)
        {

        }

        public void Error(string category, string message, Exception exception)
        {

        }

        public void Error(TraceRecord record)
        {

        }

        public void Fatal(string message, Exception exception)
        {

        }

        public void Fatal(string category, string message, Exception exception)
        {

        }

        public void Fatal(TraceRecord record)
        {

        }

        public object GetUnderlyingStorage()
        {
            return null;
        }

        public bool IsInitialized { get; set; }

        public void Dispose()
        {

        }
    }
}
