namespace CloudSyncHelper.SyncItem
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;

    public static class SyncItemsConfig
    {
        public static IList<SyncObjectConfigElement> GetAllConfigItems()
        {
            var syncObjectConfigSection = (SyncItemsConfigSection)ConfigurationManager.GetSection("syncItemsConfig");

            return syncObjectConfigSection.SyncObjectConfigCollection
                .Cast<SyncObjectConfigElement>()
                .ToList();
        }
    }

    public class SyncItemsConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public SyncObjectConfigCollection SyncObjectConfigCollection
        {
            get { return (SyncObjectConfigCollection) this[""]; }
            set { this[""] = value; }
        }
    }

    public class SyncObjectConfigCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new SyncObjectConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SyncObjectConfigElement) element).Executable;
        }
    }

    public class SyncObjectConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("executable", IsKey=true, IsRequired=true)]
        public string Executable
        {
            get { return (string)base["executable"]; }
            set { base["executable"] = value; }
        }

        [ConfigurationProperty("sourceDirectory", IsRequired=true)]
        public string SourceDirectory
        {
            get { return (string)base["sourceDirectory"]; }
            set { base["sourceDirectory"] = value; }
        }

        [ConfigurationProperty("targetDirectory", IsRequired=true)]
        public string TargetDirectory
        {
            get { return (string)base["targetDirectory"]; }
            set { base["targetDirectory"] = value; }
        }
    }
}