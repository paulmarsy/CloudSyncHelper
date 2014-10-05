using System;
using System.Configuration;
using System.IO;
using Topshelf;

namespace CloudSyncHelper
{
    public static class Program
    {
        public static int SyncTimeout;

        public static void Main(string[] args)
        {
            SyncTimeout = (int)TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings["TimeoutInMinutes"])).TotalMilliseconds;

            if (args != null && args.Length != 0 && args[0] == "backup")
            {
                BackupExistingTargets();
            }
            else
            {
                HostFactory.Run(x =>
                {
                    x.Service<CloudSyncHelper>(csh =>
                    {
                        csh.ConstructUsing(() => new CloudSyncHelper());
                        csh.WhenStarted(cshStart => cshStart.Start(args));
                        csh.WhenStopped(cshStop => cshStop.Stop());
                        csh.WhenShutdown(cshShutdown => cshShutdown.ShutDown());
                    });
                    x.RunAsLocalSystem();
                    x.StartAutomaticallyDelayed();
                    x.EnableShutdown();
                    x.SetDescription("A utility for backing up local files to cloud storage folders when processes exit");
                    x.SetDisplayName("Cloud Sync Helper");
                    x.SetServiceName("CloudSyncHelper");
                    x.BeforeInstall(BackupExistingTargets);
                });
            }
        }

        private static void BackupExistingTargets()
        {
            foreach (var syncItemConfig in SyncItem.SyncItemsConfig.GetAllConfigItems())
            {
                var backupDirectory = Path.ChangeExtension(syncItemConfig.TargetDirectory, "backup");
                if (Directory.Exists(backupDirectory))
                    Directory.Delete(backupDirectory, true);
                var syncItem = new SyncItem.SyncItem(string.Empty, syncItemConfig.TargetDirectory,
                    backupDirectory, SyncTimeout);
                syncItem.PerformAction();
            }
        }
    }
}