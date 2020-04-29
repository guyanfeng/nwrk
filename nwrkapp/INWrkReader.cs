using System;

namespace nwrk.app
{
    public interface INWrkReader : IDisposable
    {   
        public string[] ReadLine();
    }
}
