using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace nwrk.app
{
    public class NWrkDBWriter : INWrkWriter
    {
        public DbCommand Command { get; set; }

        object _lockWrite = new object();

        public NWrkDBWriter()
        {
        }

        public void Dispose()
        {
            if (Command != null)
            {
                Command.Connection.Dispose();
                Command.Dispose();
            }
        }

        public int WriteLine(string[] fields)
        {
            lock (_lockWrite)
            {
                var cmd = Command;
                for (int i = 0; i < cmd.Parameters.Count; i++)
                {
                    cmd.Parameters[i].Value = fields[i];
                }
                return cmd.ExecuteNonQuery();
            }
        }

        public void ReadConfig(IConfigurationSection section)
        {
            
        }
    }
}
