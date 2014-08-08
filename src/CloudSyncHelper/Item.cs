using System;
using System.Diagnostics;
using System.IO;

namespace CloudSyncHelper
{
    public abstract class Item
    {
        public enum States
        {
            NotRun,
            Pending,
            Succeeded,
            Failed
        }

        private readonly string _executable;
        private States _state;

        protected Item(string executable)
        {
            _executable = executable;
            _state = States.NotRun;
        }

        public void UpdateState(States newState)
        {
            _state = newState;
        }

        public bool IsExecutableRunning()
        {
            var processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(_executable));
            return processes.Length > 0;
        }

        public bool NeedsActionPerforming()
        {
            if (_state == States.Succeeded && !IsExecutableRunning())
                return false;

            return true;
        }

        protected string ProcessPath(string path)
        {
            return Environment.ExpandEnvironmentVariables(path);
        }

        public abstract bool PerformAction();
    }
}
