using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZoLog
{
    class FolderCleaner
    {
        private readonly string _directory;

        private readonly TimeSpan _threshold;

        private readonly object _cleanLock;

        private readonly Timer _timer;

        internal FolderCleaner(string path, OpenStreams streams, TimeSpan threshold, TimeSpan interval)
        {
            _directory = path;
            _threshold = threshold;//保存文件的最长时间
            _cleanLock = new object();
            _timer = new Timer(Clean, null, TimeSpan.Zero, interval);
        }

        public void Dispose()
        {
            lock (_cleanLock)
            {
                _timer.Dispose();
            }
        }

        private void Clean(object ignored)
        {
            lock (_cleanLock)
            {
                if (!Directory.Exists(_directory))
                    return;

                var now = DateTime.Now;

                var files = Directory.GetFiles(_directory);

                foreach (var filepath in files)
                {
                    var file = new FileInfo(filepath);
                    var lifetime = now - file.CreationTime;

                    if (lifetime >= _threshold)
                        file.Delete();
                }
            }
        }
    }
}
