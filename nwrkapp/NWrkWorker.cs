﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Configuration;

namespace nwrk.app
{
    public interface INWrkWorker: IDisposable, IAppSetting
    {
        public int Run();
    }

    public abstract class NWrkWorker : INWrkWorker
    {
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(NWrkWorker));

        public INWrkReader Reader { get; set; }
        public INWrkWriter Writer { get; set; }
        public INWrkMonitor NWrkMonitor { get; set; } = new NWrkMonitor();
        public int WorkerCount { get; set; } = 50;

        public NWrkWorker()
        {

        }

        protected abstract Task<string[]> ExecuteReader(string[] record);

        protected virtual void OnExecuteError(string[] input, Exception ex)
        {
            Console.WriteLine($"OnExecuteError:{ex.GetBaseException().Message}");
        }

        protected virtual bool OnRecordExecuted(string[] input, int count)
        {
            if (count >= 1000)
            {
                _log.Info($"{count} records processed");
                return true;
            }
            return false;
        }
        
        public int Run()
        {
            if (Reader == null)
            {
                _log.Error("reader can't be null");
                throw new NullReferenceException("reader can't be null");
            }
            if (Writer == null)
            {
                _log.Error("writer can't be null");
                throw new NullReferenceException("writer can't be null");
            }

            if (WorkerCount < 1 || WorkerCount > 2000)
            {
                _log.Error($"worker count must be 1-2000");
                throw new ArgumentOutOfRangeException(nameof(WorkerCount));
            }

            _log.Info($"run start, reader:{Reader}, writer:{Writer}, monitor:{NWrkMonitor}, workers:{WorkerCount}");
            var c = 0;
            object lockTmp = new object();
            var tasks = new List<Thread>();
            var count = 0;
            NWrkMonitor.Start();
            for (int i = 0; i < WorkerCount; i++)
            {
                var task = new Thread(() =>
                {
                    string[] record = null;
                    while ((record = Reader.ReadLine()) != null)
                    {
                        string[] repRecord = null;
                        var rowKey = NWrkMonitor.RequestBegin(record);
                        try
                        {
                            repRecord = ExecuteReader(record).Result;
                            NWrkMonitor.RequestSuccess(rowKey);
                        }
                        catch (Exception ex)
                        {
                            OnExecuteError(record, ex);
                            NWrkMonitor.RequestFaild(rowKey);
                        }
                        if (repRecord != null)
                        {
                            Writer.WriteLine(repRecord);
                        }

                        lock (lockTmp)
                        {
                            c++;
                            count++;
                            if (OnRecordExecuted(record, c))
                            {
                                c = 0;
                            }
                        }
                    }
                });
                task.Start();
                tasks.Add(task);
            }
            _log.Debug($"{tasks.Count} tasks add completed, wait to exit");
            tasks.ForEach(p => p.Join());
            _log.Debug($"run end");
            NWrkMonitor.End();
            //flush writer
            Writer.Dispose();

            NWrkMonitor.Report();
            return 1;
        }

        public virtual void Dispose()
        {
            Reader?.Dispose();
            Writer?.Dispose();
        }

        public virtual void ReadConfig(IConfigurationSection section)
        {
            if (!int.TryParse(section["workerCount"], out int count))
            {
                count = 50;
            }
            WorkerCount = count;
        }
    }
}
