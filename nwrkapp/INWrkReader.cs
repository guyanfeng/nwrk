using System;

namespace nwrk.app
{
    public interface INWrkReader : IDisposable, IAppSetting
    {   
        public string[] ReadLine();
    }
}
