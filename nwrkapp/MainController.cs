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
            INWrkReader reader;
            INWrkWriter writer;
            NWrkWorker worker;
            try
            {
                reader = AppSettings.GetReader();
                writer = AppSettings.GetWriter();
                worker = (NWrkWorker)AppSettings.GetWorker();
            }
            catch (Exception ex)
            {
                _log.Error($"read config faild:{ex.Message}");
                return 0;
            }

            worker.Reader = reader;
            worker.Writer = writer;
            
            worker.Run();
            return 1;
        }
    }
}
