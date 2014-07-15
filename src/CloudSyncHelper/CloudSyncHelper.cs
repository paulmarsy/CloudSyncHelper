using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;

namespace CloudSyncHelper
{
    public class CloudSyncHelper
    {
        private readonly Timer _timer = new Timer();
        private readonly IList<Item> _items = new List<Item>();

        public void Start(string[] args)
        {
            foreach (var deleteConfigItem in DeleteItem.DeleteItemsConfig.GetAllConfigItems())
            {
                _items.Add(new DeleteItem.DeleteItem(deleteConfigItem.Executable, deleteConfigItem.DeleteDirectory));
            }
        
            foreach (var syncConfigItem in SyncItem.SyncItemsConfig.GetAllConfigItems())
            {
                _items.Add(new SyncItem.SyncItem(syncConfigItem.Executable,
                                                 syncConfigItem.SourceDirectory,
                                                 syncConfigItem.TargetDirectory,
                                                 Program.SyncTimeout));
            }

            _timer.Interval = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["PollingIntervalInSeconds"])).TotalMilliseconds;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        public void Stop()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        public void ShutDown()
        {
            Stop();
            foreach (var syncConfigItem in SyncItem.SyncItemsConfig.GetAllConfigItems())
                foreach (var syncItemProcess in Process.GetProcessesByName(Path.GetFileNameWithoutExtension(syncConfigItem.Executable)))
                    syncItemProcess.Kill();

            foreach (var item in _items)
                item.PerformAction();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_timer)
            {
                foreach (var item in _items.Where(item => item.NeedsActionPerforming()))
                {
                    item.UpdateState(Item.States.Pending);

                    if (!item.IsExecutableRunning())
                    {
                        if (item.PerformAction())
                            item.UpdateState(Item.States.Succeeded);
                        else
                            item.UpdateState(Item.States.Failed);
                    }
                }
            }
        }
    }
}