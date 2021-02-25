using System;
using System.Reflection;

namespace nwrk.app
{
    public static class AppSettings
    {
        public static INWrkReader GetReader()
        {
            var section = bm.common.CommonStartUp.Configuration.GetSection("reader");
            if (section == null)
                return null;
            var type = section["type"];
            var reader = (INWrkReader)Activator.CreateInstance($"nwrk.app", $"nwrk.app.{type}").Unwrap();
            reader.ReadConfig(section);
            return reader;
        }

        public static INWrkWriter GetWriter()
        {
            var section = bm.common.CommonStartUp.Configuration.GetSection("writer");
            if (section == null)
                return null;
            var type = section["type"];
            var writer = (INWrkWriter)Activator.CreateInstance($"nwrk.app", $"nwrk.app.{type}").Unwrap();
            writer.ReadConfig(section);
            return writer;
        }

        public static INWrkWorker GetWorker()
        {
            var section = bm.common.CommonStartUp.Configuration.GetSection("worker");
            if (section == null)
                return null;
            var type = section["type"];
            var worker = (INWrkWorker)Activator.CreateInstance($"nwrk.app", $"nwrk.app.{type}").Unwrap();
            worker.ReadConfig(section);
            return  worker;
        }
    }
}
