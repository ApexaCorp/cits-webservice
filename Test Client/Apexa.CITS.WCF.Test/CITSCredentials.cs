using System.Configuration;

namespace Apexa.CITS.WCF.Test
{
    public class CITSCredentials : ConfigurationElement
    {
        [ConfigurationProperty("username", IsRequired = true)]
        public string Username
        {
            get { return (string)this["username"]; }
        }

        [ConfigurationProperty("password", IsRequired = true)]
        public string Password
        {
            get { return (string)this["password"]; }
        }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
        }

        [ConfigurationProperty("clientId", IsRequired = true)]
        public string ClientId
        {
            get { return (string)this["clientId"]; }
        }


        public static CITSCredentials Get(string env)
        {
            return ((CITSCredentialsSection)ConfigurationManager.GetSection("citscredentials")).Credentials.GetByName(env);
        }
    }
}
