using System;
using System.Diagnostics.CodeAnalysis;

namespace ZoLog
{
    /// <summary>
    /// 初始化日志时的配置
    /// </summary>
    public struct Configuration
    {

        /// <summary>
        /// 日志文件夹保留文件的最大时间
        /// </summary>
        public TimeSpan DeleteOldFiles { get; set; }

        /// <summary>
        /// 调用DateTime.Format时的格式.
        /// 默认是 "yyyy-MM-dd HH:mm:ss".
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// 日志文件存放的目录.
        /// 默认为当前目录的"logs"文件夹.
        /// </summary>
        public string Directory { get; set; }

    }
}
