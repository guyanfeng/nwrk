# nwrk
一个简易压力测试框架



nwrk 主要由 **Reader**、**Writer**、**Worker** 和**Monitor** 四个组件组成。

从 reader 中一行一行读取数据，通过 worker 进行处理，最后用 writer 写入测试结果，在这个过程中 monitor 进行一些基础的统计工作。



# Reader

框架的读取器，只要实现 IReader 接口即可，返回包括多个字段的行数据

```c#
    public interface INWrkReader : IDisposable
    {   
        public string[] ReadLine();
    }
```

系统内置了一个 **NWrkCsvReader** 读取器， 可以从 csv 文件中读取数据。

注：读取器必须是***线程安全***的



# Writer

框架的写入器，只要实现 IWriter 接口即可，将包含多个字段的行数据写入指定的地方

```c#
    public interface INWrkWriter : IDisposable
    {
        public int WriteLine(string[] fields);
    }
```

系统内置了一个 **NWrkCsvWriter** 写入器，可以将结果写入到 csv 文件中。

注：写入器必须是***线程安全***的



# Worker

数据处理器，worker 中最重要的属性有

```c#
        public INWrkReader Reader { get; set; }
        public INWrkWriter Writer { get; set; }
        public INWrkMonitor NWrkMonitor { get; set; } = new NWrkMonitor();
        /// <summary>
        /// 线程数
        /// </summary>
        public int WorkerCount { get; set; } = 50;
```

最主要的方法是

```c#
        protected abstract Task<string[]> ExecuteReader(string[] record);
```

使用者必须继承该方法，以实现自己的处理过程。

系统内置了一个 **NWrkHttpWorker** 处理器，可以通过 HttpClient 调用第三方 API 接口。



# Monitor

监控器，保存每一个处理请求的时间、时长等数据，最终可生成如下的报告

```shell
2020-04-29 13:36:01,761 [1] INFO  nwrk.app.NWrkMonitor - 100000 requests in 53.865471 s, 99928 success, 72 faild, Requests/sec: 1856
2020-04-29 13:36:01,802 [1] INFO  nwrk.app.NWrkMonitor - 0-1000ms, 100000, 100.00 %
2020-04-29 13:36:01,835 [1] INFO  nwrk.app.NWrkMonitor - 1000-2000ms, 0, 0.00 %
2020-04-29 13:36:01,867 [1] INFO  nwrk.app.NWrkMonitor - 2000-3000ms, 0, 0.00 %
2020-04-29 13:36:01,897 [1] INFO  nwrk.app.NWrkMonitor - 3000-4000ms, 0, 0.00 %
2020-04-29 13:36:01,928 [1] INFO  nwrk.app.NWrkMonitor - 4000-5000ms, 0, 0.00 %
2020-04-29 13:36:01,960 [1] INFO  nwrk.app.NWrkMonitor - >5000ms, 0, 0.00 %
```

