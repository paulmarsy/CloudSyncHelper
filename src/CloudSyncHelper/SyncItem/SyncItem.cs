namespace CloudSyncHelper.SyncItem
{
    using System.Diagnostics;

    public class SyncItem : Item
    {
        private readonly string _sourceDirectory;
        private readonly string _targetDirectory;
        private readonly int _timeout;

        public SyncItem(string item, string sourceDirectory, string targetDirectory, int timeout)
            : base(item)
        {
            _sourceDirectory = ProcessPath(sourceDirectory);
            _targetDirectory = ProcessPath(targetDirectory);
            _timeout = timeout;
        }

        protected override bool InternalPerformAction()
        {
            var robocopy = Process.Start("robocopy.exe",
                string.Format("\"{0}\" \"{1}\" /MIR /W:1 /R:1 /NDL /NS",
                    _sourceDirectory,
                    _targetDirectory));

            return (robocopy.WaitForExit(_timeout) || robocopy.ExitCode != 0);
        }
    }
}
