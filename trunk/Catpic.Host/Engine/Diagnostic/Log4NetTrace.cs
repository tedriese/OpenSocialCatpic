using System;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using Catpic.Utils.Configuration;
using Catpic.Utils.Diagnostic;

namespace Catpic.Host.Engine.Diagnostic
{
    /// <summary>
    /// Represents defaul trace which logs messages into local database
    /// </summary>
    public sealed class Log4NetTrace : ITrace
    {
        //private ConfigSection _config;
        private ILog _logger;
        private PatternLayout _layout = new PatternLayout();

        public Log4NetTrace()
        {
            Configure();
        }
   
        #region ITrace members

        public void Debug(string message)
        {
            WriteRecord(RecordType.Debug, new TraceRecord() { Message = message });
        }

        public void Debug(string category, string message)
        {
            WriteRecord(RecordType.Debug, new TraceRecord() { Category = category, Message = message });
        }

        public void Debug(TraceRecord record)
        {
            WriteRecord(RecordType.Debug, record);
        }

        public void Info(string message)
        {
            WriteRecord(RecordType.Info, new TraceRecord() { Message = message });
        }

        public void Info(string category, string message)
        {
            WriteRecord(RecordType.Info, new TraceRecord() { Category = category, Message = message });
        }

        public void Info(TraceRecord record)
        {
            WriteRecord(RecordType.Info, record);
        }

        public void Warn(string message)
        {
            WriteRecord(RecordType.Warn, new TraceRecord() { Message = message });
        }

        public void Warn(string category, string message)
        {
            WriteRecord(RecordType.Warn, new TraceRecord() { Category = category, Message = message });
        }

        public void Warn(TraceRecord record)
        {
            WriteRecord(RecordType.Warn, record);
        }

        public void Error(string message, Exception exception)
        {
            WriteRecord(RecordType.Error, new TraceRecord() { Message = message, Exception = exception });
        }

        public void Error(string category, string message, Exception exception)
        {
            WriteRecord(RecordType.Error, new TraceRecord() { Category = category, Message = message, Exception = exception });
        }

        public void Error(TraceRecord record)
        {
            WriteRecord(RecordType.Error, record);
        }

        public void Fatal(string message, Exception exception)
        {
            WriteRecord(RecordType.Fatal, new TraceRecord() { Message = message, Exception = exception });
        }

        public void Fatal(string category, string message, Exception exception)
        {
            WriteRecord(RecordType.Fatal, new TraceRecord() { Category = category, Message = message, Exception = exception });
        }

        public void Fatal(TraceRecord record)
        {
            WriteRecord(RecordType.Fatal, record);
        }

        /// <summary>
        /// Level of tracing
        /// </summary>
        public int Level { get; set; }


        public bool IsInitialized { get; set; }

        #endregion

        private void WriteRecord(RecordType type, TraceRecord record)
        {
            //level is higher than type of trace record
            if (Level > (int)type)
                return;
            //initialize storage
            //InitializeUnderlyingStorage();
            var message = FormatMessage(type, record);
            switch (type)
            {
                case RecordType.Debug:
                    _logger.Debug(message);
                    break;
                case RecordType.Info:
                    _logger.Info(message);
                    break;
                case RecordType.Warn:
                    _logger.Warn(message);
                    break;
                case RecordType.Error:
                    _logger.Error(message);
                    break;
                case RecordType.Fatal:
                    _logger.Fatal(message);
                    break;
            }

        }

        private string FormatMessage(RecordType type, TraceRecord record)
        {
            //TODO improve formatting
            return  String.Format("[{0}]:{1}",
                record.Category != null ? record.Category : "",
                record.Message + " "  + (record.Exception != null? record.Exception.ToString():""));

        }

        public void Dispose()
        {

        }

        public object GetUnderlyingStorage()
        {
            return null;
        }


        private void InitializeUnderlyingStorage()
        {
        }

        #region nested classes

        internal enum RecordType
        {
            Debug,
            Info,
            Warn,
            Error,
            Fatal
        }

        #endregion



        private void Configure()
        {
            //NOTE this configuration seems to afect all log4net loggers
            Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();
            TraceAppender tracer = new TraceAppender();
            PatternLayout patternLayout = new PatternLayout();

            patternLayout.ConversionPattern = "%d [%t] %-5p %m%n";
            patternLayout.ActivateOptions();

            tracer.Layout = patternLayout;
            tracer.ActivateOptions();
            hierarchy.Root.AddAppender(tracer);

            RollingFileAppender roller = new RollingFileAppender();
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.Layout = patternLayout;
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 4;
            roller.MaximumFileSize = "100KB";
            //roller.StaticLogFileName = true;
            roller.File = "Catpic.log.txt";
            roller.ActivateOptions();
            hierarchy.Root.AddAppender(roller);

            hierarchy.Root.Level = log4net.Core.Level.All;
            hierarchy.Configured = true;

            var name = "default";

            _logger = LogManager.GetLogger(name);
            
            Level = 0;
            IsInitialized = true;
        }
    }
}
