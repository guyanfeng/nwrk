using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace bm.common
{
    public abstract class BaseController
    {
        public static IServiceProvider ServiceProvider;

        public abstract int Run();
    }
}
