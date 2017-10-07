using System;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Collections.Generic;

namespace ClientTools.LogProviders
{
    public class FileLogService : IFileLogService
    {
        private readonly ILogger<FileLogService> _logger;
        public bool IsEndabled { get; set; }
        public Dictionary<string, string> LogFilePaths { get; set; }

        public FileLogService(ILoggerFactory fileLogService)
        {
            _logger = fileLogService.CreateLogger<FileLogService>();
            LogFilePaths = new Dictionary<string, string>();
        }

        public void LogCritical(EventId eventId, string message, params object[] args)
        {
            WriteToLog(LogLevel.Critical, eventId, message, args);
        }

        public void LogCritical(EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteToLog(LogLevel.Critical, eventId, exception, message, args);
        }

        public void LogCritical(string message, params object[] args)
        {
            WriteToLog(LogLevel.Critical, message, args);
        }

        public void LogDebug(EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteToLog(LogLevel.Debug, eventId, exception, message, args);
        }

        public void LogDebug(string message, params object[] args)
        {
            WriteToLog(LogLevel.Debug, message, args);
        }

        public void LogDebug(EventId eventId, string message, params object[] args)
        {
            WriteToLog(LogLevel.Debug, eventId, message, args);
        }

        public void LogError(EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteToLog(LogLevel.Error, eventId, exception, message, args);
        }

        public void LogError(EventId eventId, string message, params object[] args)
        {
            WriteToLog(LogLevel.Error, eventId, message, args);
        }

        public void LogError(string message, params object[] args)
        {
            WriteToLog(LogLevel.Error, message, args);
        }

        public void LogInformation(string message, params object[] args)
        {
            WriteToLog(LogLevel.Information, message, args);
        }

        public void LogInformation(EventId eventId, string message, params object[] args)
        {
            WriteToLog(LogLevel.Information, eventId, message, args);
        }

        public void LogInformation(EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteToLog(LogLevel.Information, eventId, exception, message, args);
        }

        public void LogTrace(string message, params object[] args)
        {
            WriteToLog(LogLevel.Trace, message, args);
        }

        public void LogTrace(EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteToLog(LogLevel.Trace, eventId, exception, message, args);
        }

        public void LogTrace(EventId eventId, string message, params object[] args)
        {
            WriteToLog(LogLevel.Trace, eventId, message, args);
        }

        public void LogWarning(EventId eventId, string message, params object[] args)
        {
            WriteToLog(LogLevel.Warning, eventId, message, args);
        }

        public void LogWarning(string message, params object[] args)
        {
            WriteToLog(LogLevel.Warning, message, args);
        }

        public void LogWarning(EventId eventId, Exception exception, string message, params object[] args)
        {
            WriteToLog(LogLevel.Warning, eventId, exception, message, args);
        }

        private void WriteToLog(LogLevel logLevel, string message, params object[] args)
        {
            if (!IsEndabled)
            {
                return;
            }

            //TODO: Do something with args
            if (logLevel == LogLevel.None)
            {
                return;
            }
            string msg = $"{logLevel} | {DateTime.Now} | {message}";
            
            using (var writer = File.AppendText( GetLogPath(logLevel)))
            {
                writer.WriteLine(msg);
            }
        }

        private void WriteToLog(LogLevel logLevel, EventId eventId, string message, params object[] args)
        {
            if (!IsEndabled)
            {
                return;
            }

            //TODO: Do something with args
            if (logLevel == LogLevel.None)
            {
                return;
            }
            string msg = $"{logLevel} | {DateTime.Now} | EventId {eventId} | {message}";
            using (var writer = File.AppendText(GetLogPath(logLevel)))
            {
                writer.WriteLine(msg);
            }
        }
        private void WriteToLog(LogLevel logLevel, EventId eventId, Exception exception, string message, params object[] args)
        {

            if (!IsEndabled)
            {
                return;
            }

            //TODO: Do something with args
            if (logLevel == LogLevel.None)
            {
                return;
            }
            string msg = $"{logLevel} | {DateTime.Now} | EventId {eventId} | {message} | {exception.StackTrace}";
            using (var writer = File.AppendText(GetLogPath(logLevel)))
            {
                writer.WriteLine(msg);
            }
        }

        private string GetLogPath(LogLevel level)
        {
            string logPath = string.Empty;
            switch (level)
            {
                case LogLevel.Critical:
                    logPath = LogFilePaths["Log.Critical"];
                    break;
                case  LogLevel.Debug:
                    logPath = LogFilePaths["Log.Debug"];
                    break;
                case  LogLevel.Error:
                    logPath = LogFilePaths["Log.Error"];
                    break;
                case  LogLevel.Information:
                    logPath = LogFilePaths["Log.Information"];
                    break;
                case LogLevel.Trace:
                    logPath = LogFilePaths["Log.Trace"];
                    break;
                case  LogLevel.Warning:
                    logPath = LogFilePaths["Log.Warning"];
                    break;
                    // TODO: What about None?
            }

            return logPath;
        }
    }
}
