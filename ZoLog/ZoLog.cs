using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ZoLog
{
    class ZoLog
    {

        private Configuration _configuration;

        private OpenStreams _openStreams;

        private FolderCleaner _cleaner;

        private object _lock;

        private int _longestLabel;

        private bool _disposed;


        /// <summary>
        /// Constructs the logger using the given configuration.
        /// </summary>
        /// <param name="configuration">Configuration to use</param>
        public ZoLog(Configuration configuration)
        {
            _configuration = configuration;

            if (string.IsNullOrEmpty(_configuration.Directory))
                _configuration.Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            _openStreams = new OpenStreams(_configuration.Directory);

            if (_configuration.DeleteOldFiles.TotalDays<8)
            {
                _configuration.DeleteOldFiles = new TimeSpan(8, 0, 0, 0);
            }

            TimeSpan cleanUpTime = new TimeSpan(1, 0, 0, 0);

            _cleaner = new FolderCleaner(_configuration.Directory, _openStreams, _configuration.DeleteOldFiles, cleanUpTime);

            if (string.IsNullOrEmpty(_configuration.DateTimeFormat))
                _configuration.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


            _lock = new object();
            _longestLabel = 5;
        }

        /// <summary>
        /// Formats the given information and logs it.
        /// </summary>
        /// <param name="label">Label to use when logging</param>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods",Justification = "The called function validates it.")]
        public void Log(Enum label, string content) => Log(label.ToString(), content);

        /// <summary>
        /// Formats the given information and logs it.
        /// </summary>
        /// <param name="label">Label to use when logging</param>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public void Log(string label, object content)
        {

            var date = DateTime.Now;
            var formattedDate = date.ToString(_configuration.DateTimeFormat, CultureInfo.InvariantCulture);
            var padding = new string(' ', _longestLabel - label.Length);

            var line = string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}{3}", formattedDate, label, padding, content);

            lock (_lock)
            {
                _openStreams.Append(date, line);
            }
        }

        /// <summary>
        /// Formats the given information and logs it with DEBUG label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public void LogDebug(object content) => Log("DEBUG", content);

        /// <summary>
        /// Formats the given information and logs it with INFO label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public void LogInfo(object content) => Log("INFO", content);

        /// <summary>
        /// Formats the given information and logs it with WARN label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public void LogWarn(object content) => Log("WARN", content);

        /// <summary>
        /// Formats the given information and logs it with ERROR label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public void LogError(object content) => Log("ERROR", content);

        /// <summary>
        /// Formats the given information and logs it with FATAL label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public void LogFatal(object content) => Log("FATAL", content);

        /// <summary>
        /// Disposes the file writer and the directory cleaner used by this instance.
        /// </summary>
        public void Dispose()
        {
            lock (_lock)
            {
                if (_disposed)
                    return;

                if (_openStreams != null)
                    _openStreams.Dispose();

                if (_cleaner != null)
                    _cleaner.Dispose();

                _disposed = true;
            }
        }
    }
}
