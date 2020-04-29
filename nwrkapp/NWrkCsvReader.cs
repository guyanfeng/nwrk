using System;
using System.IO;
using CsvHelper;

namespace nwrk.app
{
    public class NWrkCsvReader : INWrkReader
    {
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(NWrkCsvReader));

        CsvReader _reader;
        object _lockRead = new object();

        public NWrkCsvReader(string path)
        {
            var conf = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
            {
                BadDataFound = (context) =>
                {
                    _log.Error($"bad csv data found. row index:{context.RawRow}, row data:{context.RawRecord}");
                }
            };

            _reader = new CsvReader(new StreamReader(path), conf);
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _reader = null;
        }

        public string[] ReadLine()
        {
            lock (_lockRead)
            {
                if (_reader.Read())
                    return _reader.Context.Record;
                return null;
            }
        }
    }
}
