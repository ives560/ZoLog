using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading;

namespace ZoLog
{
    class OpenStreams
    {

        /// <summary>
        /// 日志文件完整路径
        /// </summary>
        string _filepath;

        private readonly Configuration _config;

        /// <summary>
        /// 写入日志文件的流
        /// </summary>
        private StreamWriter _streams;

        /// <summary>
        /// 
        /// </summary>
        private readonly object _lock;

        internal OpenStreams(Configuration config)
        {
            _config = config;
            _streams = null;
            _lock = new object();

        }

        internal void Append(DateTime date, string content)
        {
            lock (_lock)
            {
                _streams = GetStream(date.Date);
                _streams.WriteLine(content);
            }
        }


        private StreamWriter GetStream(DateTime date)
        {
            // Building stream's filepath
            var filename = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".log";
            var filepath = Path.Combine(_config.Directory, filename);

            if (filepath.Equals(_filepath) == true)
            {
                return _streams;
            }

            Dispose();//关闭当前日志文件

            Directory.CreateDirectory(_config.Directory);//创建日志文件夹

            FolderCleaner();//删除旧的日志文件

            // Opening the stream
            StreamWriter stream = new StreamWriter(
                                        new FileStream(
                                            filepath, FileMode.Append, FileSystemRights.AppendData, FileShare.ReadWrite, 4096,
                                            FileOptions.None));
            stream.AutoFlush = true;
            _filepath = filepath;
            return stream;
        }

        /// <summary>
        /// 删除旧的日志文件
        /// </summary>
        private void FolderCleaner()
        {
            lock (_lock)
            {
                try
                {
                    if (!Directory.Exists(_config.Directory))
                        return;

                    var now = DateTime.Now;

                    var files = Directory.GetFiles(_config.Directory);

                    foreach (var filepath in files)
                    {
                        var file = new FileInfo(filepath);
                        var lifetime = now - file.CreationTime;

                        if (lifetime >= _config.DeleteOldFiles)
                        {
                            file.Delete();//文件可能被占用无法删除
                        }

                    }
                }
                catch
                {

                }

            }
        }

        /// <summary>
        /// 关闭文件
        /// </summary>
        public void Dispose()
        {
            if(_streams != null)
            {
                _streams.Close();
                _streams.Dispose();
            }
            
        }
    }
}
