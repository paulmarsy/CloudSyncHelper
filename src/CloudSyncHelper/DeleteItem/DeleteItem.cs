namespace CloudSyncHelper.DeleteItem
{
    using System.IO;
    using System.Diagnostics;
    using Microsoft.Win32;

    public class DeleteItem : Item
    {
        private static readonly string SdeletePath = Path.Combine(Program.AppLocation, "sdelete.exe");
        private readonly string _deleteDirectory;

        static DeleteItem()
        {
            Registry.CurrentUser.CreateSubKey(@"Software\Sysinternals\SDelete").SetValue("EulaAccepted", 1, RegistryValueKind.DWord);
        }

        public DeleteItem(string item, string deleteDirectory) 
            : base(item)
        {
            _deleteDirectory = ProcessPath(deleteDirectory);
        }

        protected override bool InternalPerformAction()
        {
            if (!Directory.Exists(_deleteDirectory))
                return true;

            File.SetAttributes(_deleteDirectory, FileAttributes.Normal);
            foreach (var entry in Directory.EnumerateFileSystemEntries(_deleteDirectory, "*", SearchOption.AllDirectories))
                File.SetAttributes(entry, FileAttributes.Normal);

            var sdeleteProcess = Process.Start(SdeletePath, string.Format("-s \"{0}\"", _deleteDirectory));
            sdeleteProcess.WaitForExit();

            return !Directory.Exists(_deleteDirectory);
        }
    }
}
