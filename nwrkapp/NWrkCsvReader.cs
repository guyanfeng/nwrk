using System;
using System.IO;
using CsvHelper;
using Microsoft.Extensions.Configuration;

namespace nwrk.app
{
    public class NWrkCsvReader : INWrkReader
    {
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(NWrkCsvReader));

        CsvReader _reader;
        string _path;
        object _lockRead = new object();

        public NWrkCsvReader()
        {
            
        }

        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                if (_path != value)
                {
                    _reader?.Dispose();
                    var conf = new CsvHelper.Configuration.CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)
                    {
                        BadDataFound = (context) =>
                        {
                            _log.Error($"bad csv data found. row index:{context.RawRow}, row data:{context.RawRecord}");
                        }
                    };

                    _reader = new CsvReader(new StreamReader(value), conf);
                    _path = value;
                }
            }
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _reader = null;
        }

        public void ReadConfig(IConfigurationSection section)
        {
            Path = section["path"];
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

        public override string ToString()
        {
            return $"csv reader, path:{Path}";
        }
    }
}
