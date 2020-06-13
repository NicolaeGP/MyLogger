using System;
using System.Collections.Generic;
using System.Text;

namespace LogComponent
{
    public class LogEvent
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public LogLevel LogLevel { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
