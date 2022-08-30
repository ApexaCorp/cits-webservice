
using System.Configuration;
using System.Linq;

namespace Apexa.CITS.WCF.Test
{
    [ConfigurationCollection(typeof(CITSCredentials), AddItemName = "citscredential", CollectionType = ConfigurationElementCollectionType.BasicMap)]
    public class CITSCredentialsCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new CITSCredentials();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (CITSCredentials)element;
        }

        public CITSCredentials GetByName(string name)
        {
            var items = BaseGetAllKeys();
            var element = from CITSCredentials ea in BaseGetAllKeys()
                          where ea.Name == name
                          select ea;

            return element.SingleOrDefault();
        }
    }

    public class CITSCredentialsSection : ConfigurationSection
    {
        [ConfigurationProperty("", IsRequired = true, Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public CITSCredentialsCollection Credentials
        {
            get { return (CITSCredentialsCollection)this[""]; }
        }
    }
}
