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

        private readonly string _directory;

        //private StreamWriter _streams;

        private readonly object _lock;


        internal OpenStreams(string directory)
        {
            _directory = directory;
            //_streams = null;
            _lock = new object();

        }

        internal void Append(DateTime date, string content)
        {
            lock (_lock)
            {
                StreamWriter streams = GetStream(date.Date);
                streams.WriteLine(content);
                streams.Close();
                streams.Dispose();
            }
        }

        private void ClosePastStreams(object ignored)
        {
            lock (_lock)
            {
                //_streams.Close();
                //_streams.Dispose();
                //var today = DateTime.Today;
                //var past = _streams.Where(kvp => kvp.Key < today);

                //foreach (var kvp in past)
                //{
                //    kvp.Value.Dispose();
                //    _streams.Remove(kvp.Key);
                //}
            }
        }

        private StreamWriter GetStream(DateTime date)
        {
            // Building stream's filepath
            var filename = date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".log";
            var filepath = Path.Combine(_directory, filename);

            // Making sure the directory exists
            Directory.CreateDirectory(_directory);

            // Opening the stream
            var stream = new StreamWriter(
                new FileStream(
                    filepath, FileMode.Append, FileSystemRights.AppendData, FileShare.ReadWrite, 4096,
                    FileOptions.None
                )
            );
            stream.AutoFlush = true;

            // Storing the created stream
            //_streams = stream;

            return stream;
        }

        public void Dispose()
        {

        }
    }
}
