using System;
using System.IO;

namespace CloudSyncHelper.DeleteItem
{
    public class DeleteItem : Item
    {
        private readonly string _deleteDirectory;

        public DeleteItem(string executable, string deleteDirectory) : base(executable)
        {
            _deleteDirectory = ProcessPath(deleteDirectory);
        }

        public override bool PerformAction()
        {
            try
            {
                File.SetAttributes(_deleteDirectory, FileAttributes.Normal);
                Directory.Delete(_deleteDirectory, true);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
