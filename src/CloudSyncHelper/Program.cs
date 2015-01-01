namespace CloudSyncHelper
{
    using System.Reflection;
    using System;
    using System.Configuration;
    using System.IO;
    using Topshelf;

    public static class Program
    {
        public static readonly int SyncTimeout = (int)TimeSpan.FromMinutes(double.Parse(ConfigurationManager.AppSettings["TimeoutInMinutes"])).TotalMilliseconds;
        public static readonly string AppLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static void Main(string[] args)
        {
            if (args != null && args.Length != 0 && args[0] == "backup")
                BackupExistingTargets();
            else
                HostFactory.Run(x =>
                {
                    x.Service<CloudSyncHelper>(csh =>
                    {
                        csh.ConstructUsing(() => new CloudSyncHelper());

                        // Service Start/Stop
                        csh.WhenStarted(cshStart => cshStart.Start());
                        csh.WhenStopped(cshStop => cshStop.Stop());

                        // Service Pause/Continue
                        csh.WhenPaused(cshPause => cshPause.Pause());
                        csh.WhenContinued(cshContinue => cshContinue.Continue());

                        // Service Shutdown
                        csh.WhenShutdown(cshShutdown => cshShutdown.ShutDown());
                    });
                    x.EnableServiceRecovery(serviceRecovery =>
                    {
                        serviceRecovery.RestartService(1);
                    });
                    x.RunAsPrompt();
                    x.StartAutomaticallyDelayed();
                    x.EnableShutdown();
                    x.SetDescription("A utility for backing up local files to cloud storage folders when processes exit");
                    x.SetDisplayName("Cloud Sync Helper");
                    x.SetServiceName("CloudSyncHelper");
                    x.BeforeInstall(BackupExistingTargets);
                });
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
