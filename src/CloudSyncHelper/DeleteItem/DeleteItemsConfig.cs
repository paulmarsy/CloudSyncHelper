using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace CloudSyncHelper.DeleteItem
{
    public static class DeleteItemsConfig
    {
        public static IList<DeleteObjectConfigElement> GetAllConfigItems()
        {
            var deleteObjectConfigSection = (DeleteItemsConfigSection)ConfigurationManager.GetSection("deleteItemsConfig");

            return deleteObjectConfigSection.DeleteObjectConfigCollection
                .Cast<DeleteObjectConfigElement>()
                .ToList();
        }
    }

    public class DeleteItemsConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, IsDefaultCollection = true)]
        public DeleteObjectConfigCollection DeleteObjectConfigCollection
        {
            get { return (DeleteObjectConfigCollection) this[""]; }
            set { this[""] = value; }
        }
    }

    public class DeleteObjectConfigCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DeleteObjectConfigElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DeleteObjectConfigElement) element).DeleteDirectory;
        }
    }

    public class DeleteObjectConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("executable", IsRequired=true)]
        public string Executable
        {
            get { return (string)base["executable"]; }
            set { base["executable"] = value; }
        }

        [ConfigurationProperty("deleteDirectory", IsRequired = true, IsKey=true)]
        public string DeleteDirectory
        {
            get { return (string)base["deleteDirectory"]; }
            set { base["deleteDirectory"] = value; }
        }
    }
}