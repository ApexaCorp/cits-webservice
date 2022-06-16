using Apexa.CITS.WCF.Test.CITSService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Apexa.CITS.WCF.Test.Tests.Contractor
{
    /// <summary>
    /// Summary description for GetContractorBySellingCode
    /// </summary>
    [TestClass]
    public class GetContractorBySellingCode
    {
        #region Fields

        private static readonly string endpointConfigurationName = "BasicHttpBinding_ICITSService";
        private readonly string username = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Username;
        private readonly string password = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Password;
        private readonly string clientId = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.ClientId;
        private readonly string endpoint = CITSEnvironment.Get(ConfigurationManager.AppSettings["env"])?.Uri;

        #endregion

        #region Unit Tests

        /// <summary>
        /// Returns producer, appointments, and related advisors, by Broker (Selling) Code.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestApexaIdsBySellingCode()
        {
            using (var client = GetServiceClient())
            {
                var request = new NonCITSRequest()
                {
                    requestType = "RequestContractorApexaIdsBySellingCode",
                    id = "123" // SellingCode
                };

                var response = await client.ProcessNonCITSMessageAsync(request);

                if (response.ProcessNonCITSMessageResult.items != null)
                {
                    var results = new List<Result>(response.ProcessNonCITSMessageResult.items);
                    Assert.IsNotNull(results.Find(n => n.id == clientId));
                }
                else
                {
                    Assert.Fail("ProcessNonCITSMessageResult.items is null");
                }

                /* Serialize */
                CITSHelper.SerializeMessage(request);
                CITSHelper.SerializeMessage(response);
            }
        }

        #endregion

        #region Private Methods

        private CITSServiceClient GetServiceClient()
        {
            var client = new CITSServiceClient(endpointConfigurationName, new EndpointAddress(endpoint));
            client.ClientCredentials.UserName.UserName = username;
            client.ClientCredentials.UserName.Password = password;

            return client;
        }

        #endregion
    }
}
