using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace bm.common
{
    public class CommonStartUp
    {
        public static IConfiguration Configuration;
        public static IServiceCollection Services;

        public CommonStartUp()
        {
            Init();
        }

        public int Start<T>() where T : BaseController
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory());
            if (HasFile("appsettings.json"))
            {
                builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);
            }
            Configuration = builder.Build();

            Init();

            Services = new ServiceCollection();
            ConfigureServices(Services);
            BaseController.ServiceProvider = Services.BuildServiceProvider();

            return GetController<T>().Run();
        }

        bool HasFile(string name)
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            return File.Exists(Path.Combine(root, name));
        }

        public void Init()
        {
            if (HasFile("log4net.config"))
            {
                var repository = log4net.LogManager.CreateRepository(System.Reflection.Assembly.GetEntryAssembly(), typeof(log4net.Repository.Hierarchy.Hierarchy));
                log4net.Config.XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
            }
        }


        public virtual void ConfigureServices(IServiceCollection services)
        {
            
        }

        public BaseController GetController<T>() where T : BaseController
        {
            var type = typeof(T);
            var func_con = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)[0];
            var func_pas = func_con.GetParameters();
            var inst_pas = new object[func_pas.Length];
            for (int i = 0; i < func_pas.Length; i++)
            {
                inst_pas[i] = BaseController.ServiceProvider.GetService(func_pas[i].ParameterType);
            }
            return (BaseController)Activator.CreateInstance(type, inst_pas);
        }
    }
}
