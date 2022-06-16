using Apexa.CITS.WCF.Test.CITSService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Apexa.CITS.WCF.Test.Tests.ContractCodes
{
    /// <summary>
    /// Summary description for ContractCodes
    /// </summary>
    [TestClass]
    public class ContractCodes
    {
        #region Fields

        private static readonly string endpointConfigurationName = "BasicHttpBinding_ICITSService";
        private readonly string username = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Username;
        private readonly string password = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Password;
        private readonly string clientId = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.ClientId;
        private readonly string endpoint = CITSEnvironment.Get(ConfigurationManager.AppSettings["env"])?.Uri;

        #endregion

        #region Unit Tests

        [TestMethod]
        public async Task AddContractCode()
        {
            using (var client = GetServiceClient())
            {
                var request = new ContractCodeRequest()
                {
                    action = "Add",
                    code = "ABCXYZ",
                    carrierAppointmentId = "1000040",
                    type = "selling",
                    status = "active",
                    description = "Sample Contract Code"
                };

                client.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                var response = await client.ProcessContractCodeAsync(request);

                if (!response.ProcessContractCodeResult)
                {
                    Assert.Fail("Add ContractCode failed.");
                }
            }
        }

        [TestMethod]
        public async Task UpdateContractCode()
        {
            using (var client = GetServiceClient())
            {
                var request = new ContractCodeRequest()
                {
                    action = "Update",
                    originalCode = "ABCXYZ",
                    carrierAppointmentId = "1000040",
                    code = "DEFGHI",
                    type = "selling",
                    status = "active",
                    description = "Updated Sample Contract Code"
                };

                client.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                var response = await client.ProcessContractCodeAsync(request);

                if (!response.ProcessContractCodeResult)
                {
                    Assert.Fail("Update ContractCode failed.");
                }
            }
        }

        [TestMethod]
        public async Task DeleteContractCode()
        {
            using (var client = GetServiceClient())
            {
                var request = new ContractCodeRequest()
                {
                    action = "Delete",
                    carrierAppointmentId = "1000040",
                    code = "DEFGHI",
                };

                client.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                var response = await client.ProcessContractCodeAsync(request);

                if (!response.ProcessContractCodeResult)
                {
                    Assert.Fail("Delete ContractCode failed.");
                }
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