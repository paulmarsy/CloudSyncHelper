namespace CloudSyncHelper
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Timers;

    public class CloudSyncHelper
    {
        private readonly Timer _timer = new Timer();
        private readonly IList<Item> _items = new List<Item>();

        public void Start()
        {
            foreach (var deleteConfigItem in DeleteItem.DeleteItemsConfig.GetAllConfigItems())
                _items.Add(new DeleteItem.DeleteItem(deleteConfigItem.Executable, deleteConfigItem.DeleteDirectory));
        
            foreach (var syncConfigItem in SyncItem.SyncItemsConfig.GetAllConfigItems())
                _items.Add(new SyncItem.SyncItem(syncConfigItem.Executable,
                                                 syncConfigItem.SourceDirectory,
                                                 syncConfigItem.TargetDirectory,
                                                 Program.SyncTimeout));

            _timer.Interval = TimeSpan.FromSeconds(double.Parse(ConfigurationManager.AppSettings["PollingIntervalInSeconds"])).TotalMilliseconds;
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        public void Continue()
        {
            _timer.Start();
        }

        public void Stop()
        {
            ShutDown();
            var allProcessess = Process.GetProcesses(".");
            foreach (var itemProcess in allProcessess.Where(x => _items.Select(y => y.ItemName).Contains(x.ProcessName)).Distinct())
                    itemProcess.Kill();

            foreach (var item in _items)
                item.PerformAction();

            _items.Clear();
        }

        public void Pause()
        {
            _timer.Stop();
        }

        public void ShutDown()
        {
            _timer.Stop();
            _timer.Dispose();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_timer)
            {
                Console.WriteLine("Sync Timer Elapsed");
                var allProcessess = Process.GetProcesses(".");
                foreach (var item in _items)
                    switch (item.State)
                    {
                        case Item.States.NotRun:
                        case Item.States.Pending:
                        case Item.States.Failed:
                        {
                            if (allProcessess.All(x => x.ProcessName != item.ItemName))
                                item.UpdateState(item.PerformAction() ? Item.States.Succeeded : Item.States.Failed);

                            break;
                        }
                        case Item.States.Succeeded:
                        {
                            if (allProcessess.Any(x => x.ProcessName == item.ItemName))
                                item.UpdateState(Item.States.Pending);

                            break;
                        }
                    }                    
            }
        }
    }
}