using System;
namespace nwrk.app
{
    public interface INWrkWriter : IDisposable, IAppSetting
    {
        public int WriteLine(string[] fields);
    }
}
