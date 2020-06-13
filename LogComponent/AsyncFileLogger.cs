using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LogComponent
{
    public class AsyncFileLogger : ILogger
    {
        LogFormatter _logFormatter;
        IDateTime _dateTimeProvider;
        string _folderPath;
        StreamWriter _writer;
        string _currentLogFilePath;
        bool _flush = false;
        bool _stop = false;
        DateTime _logDateTime;
        ConcurrentQueue<LogEvent> _logLines = new ConcurrentQueue<LogEvent>();
        Task _loggerTask;

        /// <summary>
        /// Create an async logger.
        /// </summary>
        public AsyncFileLogger(LogFormatter logFormatter = null, string folderPath = @"logs", IDateTime dateTimeProvider = null)
        {
            if (logFormatter != null)
                _logFormatter = logFormatter;
            else
                _logFormatter = DefaultLogFormatter;

            if (dateTimeProvider != null)
                _dateTimeProvider = dateTimeProvider;
            else
                _dateTimeProvider = new DateTimeWrapper();

            _logDateTime = _dateTimeProvider.Now();
            _folderPath = folderPath;
            _currentLogFilePath = BuildLogFilePath();
            CreateDirectoryPath();
            _writer = OpenLogFile();
            _loggerTask = Task.Factory.StartNew(Start, TaskCreationOptions.LongRunning);
        }
        public void Log(LogLevel logLevel, string message)
        {
            if (!_stop)
                _logLines.Enqueue(new LogEvent { LogLevel = logLevel, Message = message, Timestamp = _dateTimeProvider.Now() });
        }

        public void Log(LogLevel logLevel, string message, Exception ex)
        {
            if (!_stop)
                _logLines.Enqueue(new LogEvent { LogLevel = logLevel, Message = message, Exception = ex, Timestamp = _dateTimeProvider.Now() });
        }

        public void Stop()
        {
            _stop = true;
            _loggerTask.Wait();
        }

        public void StopAndFlush()
        {
            _flush = true;
            _stop = true;
            _loggerTask.Wait();
        }

        private void Start()
        {
            while (true)
            {
                if (_stop && !_flush)
                    break;
                if (_stop && _logLines.Count == 0)
                    break;

                LogEvent logEvent;
                var ok = _logLines.TryDequeue(out logEvent);
                if (!ok)
                    continue;
                if (IsDateChanged(logEvent.Timestamp))
                    HandleDayChange(logEvent.Timestamp);
                _writer.WriteLine(_logFormatter(logEvent));
                Thread.Sleep(50);
            }
            _writer.Flush();
            _writer.Close();
        }


        string DefaultLogFormatter(LogEvent logEvent)
        {
            string delimiter = "  ";
            StringBuilder sb = new StringBuilder();
            sb.Append(logEvent.Timestamp.ToString("yyyy-MM-dd HH:mm:ss:fff"));
            sb.Append(delimiter);
            sb.Append(logEvent.LogLevel);
            if (!string.IsNullOrEmpty(logEvent.Message))
            {
                sb.Append(delimiter);
                sb.Append(logEvent.Message);
            }
            if (logEvent.Exception != null)
            {
                sb.Append(Environment.NewLine);
                sb.Append(logEvent.Exception.ToString());
            }
            return sb.ToString();
        }

        void CreateDirectoryPath()
        {
            if (!Directory.Exists(_folderPath))
                Directory.CreateDirectory(_folderPath);
        }

        string BuildLogFilePath()
        {
            return Path.Combine(_folderPath, "Log-" + _logDateTime.ToString("yyyy-MM-dd") + ".log");
        }

        bool IsDateChanged(DateTime logEventDateTime)
        {
            if (logEventDateTime != _dateTimeProvider.Now().Date)
                return true;
            return false;
        }

        void HandleDayChange(DateTime logEventDateTime)
        {
            _writer.Flush();
            _writer.Close();
            _logDateTime = logEventDateTime;
            _currentLogFilePath = BuildLogFilePath();
            _writer = OpenLogFile();
        }

        StreamWriter OpenLogFile()
        {
            return File.AppendText(_currentLogFilePath);
        }

    }
}
