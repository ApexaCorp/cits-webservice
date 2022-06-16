using System.Configuration;
using System.Linq;

namespace Apexa.CITS.WCF.Test
{
    public class CITSEnvironment : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("uri", IsRequired = true)]
        public string Uri
        {
            get { return (string)this["uri"]; }
        }

        public static CITSEnvironment Get(string env)
        {
            return ((CITSEnvironmentSection)ConfigurationManager.GetSection("citsenvironments")).Environments.GetByName(env);
        }
    }


    [ConfigurationCollection(typeof(CITSEnvironment), AddItemName = "citsenvironment", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class CITSEnvironmentCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CITSEnvironment();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (CITSEnvironment)element;
        }

        public CITSEnvironment GetByName(string name)
        {
            var items = BaseGetAllKeys();
            var element = from CITSEnvironment ea in BaseGetAllKeys()
                          where ea.Name == name
                          select ea;

            return element.SingleOrDefault();
        }
    }

    public class CITSEnvironmentSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public CITSEnvironmentCollection Environments
        {
            get { return (CITSEnvironmentCollection)this[""]; }
        }
    }
}
