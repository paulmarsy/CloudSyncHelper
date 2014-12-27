using System;
using System.Diagnostics;
using System.IO;

namespace CloudSyncHelper
{
    using System.Linq;

    public abstract class Item
    {
        public enum States
        {
            NotRun,
            Succeeded,
            Failed
        }

        private readonly string _processName;

        protected Item(string executable)
        {
            _processName = Path.GetFileNameWithoutExtension(executable);
            State = States.NotRun;
        }

        public States State { get; private set; }

        public void UpdateState(States newState)
        {
            State = newState;
        }

        public bool IsExecutableRunning()
        {
            return Process.GetProcesses(".").Any(x => x.ProcessName == _processName);
        }

        protected string ProcessPath(string path)
        {
            return Environment.ExpandEnvironmentVariables(path);
        }

        public abstract bool PerformAction();
    }
}
