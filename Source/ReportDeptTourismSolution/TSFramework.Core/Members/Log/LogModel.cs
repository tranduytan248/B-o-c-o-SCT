using System;

namespace TSFramework.Core.Members.Log
{
    public class LogModel
    {
        /// <summary>
        ///     Content of log
        /// </summary>
        private readonly string _logMessage;

        /// <summary>
        ///     Log timestamp
        /// </summary>
        private readonly DateTime _logTime;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="message">Logged message</param>
        public LogModel(string message)
        {
            _logMessage = message;
            _logTime = DateTime.Now;
        }

        /// <summary>
        ///     Log message accessor
        /// </summary>
        /// <returns>Log message</returns>
        public string GetMessage()
        {
            return _logMessage;
        }

        /// <summary>
        ///     Get the time from log timestamp
        /// </summary>
        /// <returns>Time</returns>
        public string GetTime()
        {
            return _logTime.ToString("hh:mm:ss.fff tt");
        }

        /// <summary>
        ///     Get the date from log timestamp
        /// </summary>
        /// <returns>Date</returns>
        public string GetDate()
        {
            return _logTime.ToString("yyyy-MM-dd");
        }
    }
}