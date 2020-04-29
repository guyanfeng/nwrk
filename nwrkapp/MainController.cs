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
            var reader = new NWrkCsvReader("/Users/michael/Projects/netcore/nwrk/testdata/source.txt");
            var writer = new NWrkDBWriter();

            var conn = new Npgsql.NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=michael;Integrated Security=true;");
            conn.Open();

            var cmd = conn.CreateCommand();
            cmd.CommandText = "insert into nwrk values(@id,@name,@result)";
            cmd.CommandTimeout = 5000;
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@id", NpgsqlTypes.NpgsqlDbType.Varchar));
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@name", NpgsqlTypes.NpgsqlDbType.Varchar));
            cmd.Parameters.Add(new Npgsql.NpgsqlParameter("@result", NpgsqlTypes.NpgsqlDbType.Varchar));

            writer.Command = cmd;
            
            var worker = new NWrkHttpTestWorker()
            {
                BaseUrl = new Uri("http://127.0.0.1:5002"),
                RelUrl = "/nwrk",
                Method = "POST",
                Reader = reader,
                Writer = writer,
                WorkerCount = 500
            };

            worker.Run();
            return 1;
        }
    }
}
