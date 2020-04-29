using System;
using CsvHelper;
using System.IO;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace nwrk.app
{
    public class NWrkCsvWriter : INWrkWriter
    {
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(NWrkCsvWriter));

        protected CsvWriter _writer;
        ConcurrentStack<string[]> _buffer;

        //Task _writeBufferTask;

        bool _isDisposed;

        object _lockWrite = new object();

        public NWrkCsvWriter(string path)
        {
            var conf = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture);
            _writer = new CsvWriter(new StreamWriter(path, false, System.Text.Encoding.UTF8), conf);
            _buffer = new ConcurrentStack<string[]>();
            //_writeBufferTask = new Task(WriteBufferTask);
            //_writeBufferTask.Start();
        }

        public int WriteLine(string[] record)
        {
            _buffer.Push(record);
            lock (_lockWrite)
            {
                foreach (var field in record)
                {
                    _writer.WriteField(field);
                }
                _writer.NextRecord();
                return 1;
            }
        }

        public void WriteBufferTask()
        {
            while (_isDisposed)
            {
                if (_buffer.TryPop(out string[] record))
                {
                    foreach (var field in record)
                    {
                        _writer.WriteField(field);
                    }
                    _writer.NextRecord();
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;
            
            _writer?.Dispose();
        }
    }
}
