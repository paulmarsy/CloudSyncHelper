namespace CloudSyncHelper
{
    using System;
    using System.IO;  

    public abstract class Item
    {
        public enum States
        {
            NotRun,
            Pending,
            Succeeded,
            Failed
        }

        public readonly string ItemName;

        protected Item(string item)
        {
            ItemName = Path.GetFileNameWithoutExtension(item);
            State = States.NotRun;
        }

        public States State { get; private set; }

        public void UpdateState(States newState)
        {
            Console.WriteLine("Updating {0} on {1} from {2} to {3}", this.GetType().Name, ItemName, State, newState);
            State = newState;
        }

        protected static string ProcessPath(string path)
        {
            return Environment.ExpandEnvironmentVariables(path).Replace("%CD%", Program.AppLocation);
        }

        public bool PerformAction()
        {
            Console.WriteLine("Performing {0} on {1}", this.GetType().Name, ItemName);
            try
            {
                return InternalPerformAction();
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                return false;
            }
        }

        protected abstract bool InternalPerformAction();
    }
}
