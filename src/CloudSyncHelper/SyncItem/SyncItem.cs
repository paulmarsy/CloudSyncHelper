using System;
using System.Diagnostics;

namespace CloudSyncHelper.SyncItem
{
    public class SyncItem : Item
    {
        private readonly string _sourceDirectory;
        private readonly string _targetDirectory;
        private readonly int _timeout;

        public SyncItem(string executable, string sourceDirectory, string targetDirectory, int timeout)
            : base(executable)
        {
            _sourceDirectory = ProcessPath(sourceDirectory);
            _targetDirectory = ProcessPath(targetDirectory);
            _timeout = timeout;
        }

        public override bool PerformAction()
        {
            var robocopy = Process.Start("robocopy.exe",
                                         Environment.ExpandEnvironmentVariables(string.Format("\"{0}\" \"{1}\" /MIR /W:1 /R:1 /NDL /NS",
                                                                                              _sourceDirectory,
                                                                                              _targetDirectory)));

            return (robocopy.WaitForExit(_timeout) || robocopy.ExitCode != 0);
        }
    }
}
