using System;
namespace nwrk.app
{
    public interface INWrkWriter : IDisposable
    {
        public int WriteLine(string[] fields);
    }
}
