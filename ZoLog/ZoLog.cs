using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace ZoLog
{
    public class ZoLog
    {

        private static Configuration _configuration;

        private static OpenStreams _openStreams;

        private static FolderCleaner _cleaner;

        private static object _lock;

        private static int _longestLabel;

        private static bool _disposed;

        private static bool _configed=false;


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

        public ZoLog()
        {

        }


        public void Configuration(Configuration configuration)
        {
            _configuration = configuration;

            if (string.IsNullOrEmpty(_configuration.Directory))
                _configuration.Directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");

            _openStreams = new OpenStreams(_configuration.Directory);

            if (_configuration.DeleteOldFiles.TotalDays < 8)
            {
                _configuration.DeleteOldFiles = new TimeSpan(8, 0, 0, 0);
            }

            TimeSpan cleanUpTime = new TimeSpan(1, 0, 0, 0);

            _cleaner = new FolderCleaner(_configuration.Directory, _openStreams, _configuration.DeleteOldFiles, cleanUpTime);

            if (string.IsNullOrEmpty(_configuration.DateTimeFormat))
                _configuration.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";


            _lock = new object();
            _longestLabel = 5;
            _configed = true;
        }

        /// <summary>
        /// Formats the given information and logs it.
        /// </summary>
        /// <param name="label">Label to use when logging</param>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        //[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods",Justification = "The called function validates it.")]
        public static void Log(Enum label, string content) => Log(label.ToString(), content);

        /// <summary>
        /// Formats the given information and logs it.
        /// </summary>
        /// <param name="label">Label to use when logging</param>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public static void Log(string label, object content)
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
        public static void LogDebug(object content) => Log("DEBUG", content);

        /// <summary>
        /// Formats the given information and logs it with INFO label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public static void LogInfo(object content) => Log("INFO", content);

        /// <summary>
        /// Formats the given information and logs it with WARN label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public static void LogWarn(object content) => Log("WARN", content);

        /// <summary>
        /// Formats the given information and logs it with ERROR label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public static void LogError(object content) => Log("ERROR", content);

        /// <summary>
        /// Formats the given information and logs it with FATAL label.
        /// </summary>
        /// <param name="content">A string with a message or an object to call ToString() on it</param>
        public static void LogFatal(object content) => Log("FATAL", content);

        /// <summary>
        /// Disposes the file writer and the directory cleaner used by this instance.
        /// </summary>
        public void Dispose()
        {
            if (_configed == false)
                return;

            lock (_lock)
            {
                if (_disposed)
                    return;

                if (_openStreams != null)
                    _openStreams.Dispose();

                if (_cleaner != null)
                    _cleaner.Dispose();

                _disposed = true;
                _configed = false;
            }
        }
    }
}
