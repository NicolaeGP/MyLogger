using System;

namespace LogComponent
{
    public interface ILogger
    {
        /// <summary>
        /// Stop the logging. Any outstanding logs will not be written to Log.
        /// </summary>
        void Stop();

        /// <summary>
        /// Stop the logging. The call will not return until all all logs have been written to Log.
        /// </summary>
        void StopAndFlush();

        /// <summary>
        /// Log a message to the Log.
        /// </summary>
        /// <param name="message">The message to written to the log</param>
        /// <param name="logLevel">The Log level of the log message</param>
        void Log(LogLevel logLevel, string message);


        /// <summary>
        /// Log an error and message to the Log.
        /// </summary>
        /// <param name="message">The message to written to the log</param>
        /// <param name="ex">The error to written to the log</param>
        /// <param name="logLevel">The Log level of the log message</param>
        void Log(LogLevel logLevel, string message, Exception ex);
    }
}
