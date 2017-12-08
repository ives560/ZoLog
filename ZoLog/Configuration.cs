using System;
using System.Diagnostics.CodeAnalysis;

namespace ZoLog
{
    /// <summary>
    /// Optional configurations that can be used when initializing the logger.
    /// </summary>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct Configuration
    {
        /// <summary>
        /// True to use UTC time rather than local time.
        /// Defaults to false.
        /// </summary>
        public bool UseUtcTime { get; set; }

        /// <summary>
        /// If other than null it sets to delete any file in the log folder that is older than the specified time.
        /// Defaults to null.
        /// </summary>
        public TimeSpan DeleteOldFiles { get; set; }

        /// <summary>
        /// Format string to use when calling DateTime.Format.
        /// Defaults to "yyyy-MM-dd HH:mm:ss".
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Directory where to create the log files.
        /// Defaults to a local "logs" directory.
        /// </summary>
        public string Directory { get; set; }

    }
}
