using System;
namespace nwrk.app
{
    public interface INWrkMonitor : IAppSetting
    {
        public string RequestBegin(string[] input);
        public void RequestSuccess(string rowKey);
        public void RequestFaild(string rowKey);
        public int TotalRequest { get; }
        public TimeSpan TotalElapsed { get; }
        public void Start();
        public void End();
        public string Report();
    }
}
