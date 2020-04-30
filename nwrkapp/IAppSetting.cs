using System;
using Microsoft.Extensions.Configuration;

namespace nwrk.app
{
    public interface IAppSetting
    {
        public void ReadConfig(IConfigurationSection section);
    }
}
