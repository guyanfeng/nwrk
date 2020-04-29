using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace nwrk.app
{
    public class StartUp : bm.common.CommonStartUp
    {
        public StartUp()
        {
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
        }
    }
}
