using Apexa.CITS.WCF.Test.CITSService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.ServiceModel;
using System.ServiceModel.Security;
using System.Threading.Tasks;

namespace Apexa.CITS.WCF.Test.Tests.Access
{
    /// <summary>
    /// Summary description for Authentication
    /// </summary>
    [TestClass]
    public class Authentication
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
        public async Task UnauthorizedRequest()
        {
            var client = new CITSServiceClient(endpointConfigurationName, new EndpointAddress(endpoint));
            client.ClientCredentials.UserName.UserName = Guid.NewGuid().ToString();
            client.ClientCredentials.UserName.Password = Guid.NewGuid().ToString();

            var requestMessage = new TXLife_Type()
            {
                Version = "2.35.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        StartDate = DateTime.Now.Date.AddDays(-7),
                        StartDateSpecified = true,
                        EndDate = DateTime.Now.Date,
                        EndDateSpecified = true,
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ChangedProducerListing" },
                        TransExeDate = DateTime.Now.Date,
                        TransExeDateSpecified = true,
                        TransExeTime = new DateTime(DateTime.Now.TimeOfDay.Ticks),
                        TransExeTimeSpecified = true
                    }
                },
            };

            try
            {
                await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));
                client.Close();

                Assert.Fail("Should have failed with unauthorized.");
            }
            catch (SecurityAccessDeniedException ex)
            {
                Assert.AreEqual(ex.Message, "Access is denied.");
            }
            catch (MessageSecurityException ex)
            {
                FaultException fx = ex.InnerException as FaultException;
                Assert.AreEqual(fx?.Message, "Authentication Failed.");
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
