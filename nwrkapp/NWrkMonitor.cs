using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace nwrk.app
{
    public class NWrkMonitor : INWrkMonitor
    {
        protected DateTime _begin = DateTime.MinValue;
        protected DateTime _end = DateTime.MaxValue;
        protected object _lockRequest = new object();
        protected ConcurrentDictionary<string, RowStatus> _rows = new ConcurrentDictionary<string, RowStatus>();

        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(NWrkMonitor));
        

        public NWrkMonitor()
        {
            
        }

        public int TotalRequest
        {
            get
            {
                return _rows.Count;
            }
        }

        public TimeSpan TotalElapsed
        {
            get
            {
                return _end - _begin;
            }
        }

        public void End()
        {
            _end = DateTime.Now;
        }

        public void ReadConfig(IConfigurationSection section)
        {
            
        }

        public string Report()
        {
            var success = (from p in _rows.Keys
                           where _rows[p].IsSuccess
                           select p).Count();
            var faild = TotalRequest - success;
            _log.Info($"{TotalRequest} requests in {TotalElapsed.TotalSeconds} s, {success} success, {faild} faild, {(int)(TotalRequest / TotalElapsed.TotalSeconds)} Requests/s");

            for (int i = 0; i < 5; i++)
            {
                var start = i * 1000;
                var end = (i + 1) * 1000;
                var count = (from p in _rows.Keys
                             where _rows[p].Elapsed.TotalMilliseconds >= start && _rows[p].Elapsed.TotalMilliseconds < end
                             select p).Count();
                _log.InfoFormat("{0}-{1}ms, {2}, {3:P2}", start, end, count, (double)count / TotalRequest);
            }

            var c = (from p in _rows.Keys
                         where _rows[p].Elapsed.TotalMilliseconds > 5000
                         select p).Count();
            _log.InfoFormat(">5000ms, {0}, {1:P2}", c, (double)c / TotalRequest);
            return "OK";
        }

        public string RequestBegin(string[] input)
        {
            var rowKey = Guid.NewGuid().ToString();
            var row = new RowStatus
            {
                RowKey = rowKey,
                Input = input,
                Start = DateTime.Now
            };
            _rows[rowKey] = row;
            return rowKey;
        }

        public void RequestFaild(string rowKey)
        {
            if (_rows.TryGetValue(rowKey, out RowStatus row))
            {
                row.End = DateTime.Now;
                row.Elapsed = row.End - row.Start;
                row.IsSuccess = false;
            }
        }

        public void RequestSuccess(string rowKey)
        {
            if (_rows.TryGetValue(rowKey, out RowStatus row))
            {
                row.End = DateTime.Now;
                row.Elapsed = row.End - row.Start;
                row.IsSuccess = true;
            }
        }

        public void Start()
        {
            _begin = DateTime.Now;
        }

        public override string ToString()
        {
            return $"normal monitor";
        }
    }

    public class RowStatus
    {
        public string[] Input { get; set; }
        public string RowKey { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan Elapsed { get; set; }
        public bool IsSuccess { get; set; }
        public int Size { get; set; }
    }
}
