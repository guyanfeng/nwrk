using System;
using System.Collections.Generic;
using System.Net.Http;

namespace nwrk.app
{
    public class MainController : bm.common.BaseController
    {
        log4net.ILog _log = log4net.LogManager.GetLogger(typeof(MainController));

        public MainController()
        {
        }

        public override int Run()
        {
            _log.Debug("run start");

            var reader = new NWrkCsvReader("/Users/michael/Projects/netcore/nwrk/testdata/source.txt");
            var writer = new NWrkCsvWriter("/Users/michael/Projects/netcore/nwrk/testdata/result.txt");
            var worker = new NWrkHttpTestWorker()
            {
                BaseUrl = new Uri("http://127.0.0.1:5002"),
                RelUrl = "/nwrk",
                Method = "POST",
                Reader = reader,
                Writer = writer,
                WorkerCount = 200
            };

            worker.Run();
            return 1;
        }
    }
}
