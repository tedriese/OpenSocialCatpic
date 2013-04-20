using System;

namespace Catpic.Utils.Diagnostic
{
    /// <summary>
    /// Represents a tracer for tracing subsystem
    /// </summary>
    public interface ITrace : IDisposable
    {
        /// <summary>
        /// Level of tracing
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        /// <param name="message"></param>
        void Debug(string message);

        /// <summary>
        /// Writes message to trace using category provided
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        void Debug(string category, string message);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        /// <param name="record"></param>
        void Debug(TraceRecord record);

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        /// <param name="message"></param>
        void Info(string message);

        /// <summary>
        /// Writes message to trace using category provided
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        void Info(string category, string message);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        /// <param name="record"></param>
        void Info(TraceRecord record);

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        /// <param name="message"></param>
        void Warn(string message);

        /// <summary>
        /// Writes message to trace using category provided
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        void Warn(string category, string message);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        /// <param name="record"></param>
        void Warn(TraceRecord record);

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(string message, Exception exception);

        /// <summary>
        /// Writes message to trace
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Error(string category, string message, Exception exception);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        /// <param name="record"></param>
        void Error(TraceRecord record);

        /// <summary>
        /// Writes message to trace using default tracer category
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(string message, Exception exception);

        /// <summary>
        /// Writes message to trace
        /// </summary>
        /// <param name="category"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        void Fatal(string category, string message, Exception exception);

        /// <summary>
        /// Writes record to trace
        /// </summary>
        /// <param name="record"></param>
        void Fatal(TraceRecord record);

        /// <summary>
        /// Returns the storage of messages
        /// </summary>
        /// <returns></returns>
        object GetUnderlyingStorage();

        /// <summary>
        /// Shows whether trace is initialized
        /// </summary>
        bool IsInitialized { get; }

    }
}
