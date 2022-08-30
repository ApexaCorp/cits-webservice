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

        private const string EndpointConfigurationName = "BasicHttpBinding_ICITSService";
        private readonly string _username = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Username;
        private readonly string _password = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Password;
        private readonly string _contractorId = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.ContractorId;
        private readonly string _endpoint = CITSEnvironment.Get(ConfigurationManager.AppSettings["env"])?.Uri;

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
                    code = Guid.NewGuid().ToString(),
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
            #region Arrange

            //Arrange
            var contractToUpdate = Guid.NewGuid().ToString();
            using (var client = GetServiceClient())
            {
                var request = new ContractCodeRequest()
                {
                    action = "Add",
                    code = contractToUpdate,
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

            #endregion

            using (var client = GetServiceClient())
            {
                var request = new ContractCodeRequest()
                {
                    action = "Update",
                    originalCode = "ABCXYZ",
                    carrierAppointmentId = "1000040",
                    code = contractToUpdate,
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
            #region Arrange

            //Arrange
            var contractToDelete = Guid.NewGuid().ToString();
            using (var client = GetServiceClient())
            {
                var request = new ContractCodeRequest()
                {
                    action = "Add",
                    code = contractToDelete,
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

            #endregion

            using (var client = GetServiceClient())
            {
                var request = new ContractCodeRequest()
                {
                    action = "Delete",
                    carrierAppointmentId = "1000040",
                    code = contractToDelete,
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
            var client = new CITSServiceClient(EndpointConfigurationName, new EndpointAddress(_endpoint));
            client.ClientCredentials.UserName.UserName = _username;
            client.ClientCredentials.UserName.Password = _password;

            return client;
        }

        #endregion
    }
}