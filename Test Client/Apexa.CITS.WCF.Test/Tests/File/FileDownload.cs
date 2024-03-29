﻿using Apexa.CITS.WCF.Test.CITSService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Apexa.CITS.WCF.Test.Tests.File_Request
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class FileDownload
    {
        #region Fields

        private const string EndpointConfigurationName = "BasicHttpBinding_ICITSService";
        private readonly string _username = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Username;
        private readonly string _password = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.Password;
        private readonly string _contractorId = CITSCredentials.Get(ConfigurationManager.AppSettings["env"])?.ContractorId;
        private readonly string _endpoint = CITSEnvironment.Get(ConfigurationManager.AppSettings["env"])?.Uri;

        #endregion

        #region Unit Tests

        /// <summary>
        /// Returns a file for a given APEXA file download url.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestFiles()
        {
            using (var client = GetServiceClient())
            {
                var eoCoverageUid = Guid.Parse("1F07D866-1F14-ED11-808E-8E89C4846C7A"); // Value must be valid Document Version Id

                var requestMessage = new FileRequest()
                {
                    items = new FileURL[]
                    {
                        new FileURL() { url = $"{client.Endpoint.Address}/documents/eocoverage/{_contractorId}/{eoCoverageUid}" }
                    }
                };

                var response = await client.ProcessFileRequestsAsync(requestMessage);

                Assert.IsNotNull(response.ProcessFileRequestsResult);
                Assert.AreEqual(response.ProcessFileRequestsResult.itemsField.Length, requestMessage.items.Length);
                byte[] fileArray = response.ProcessFileRequestsResult.itemsField[0].byteArray;
                Assert.IsTrue(fileArray.Length > 0);

                string path = null;
                using (var file = System.IO.File.Create(response.ProcessFileRequestsResult.itemsField[0].filename))
                {
                    path = file.Name;
                    file.Write(fileArray, 0, fileArray.Length);
                }

                Assert.IsTrue(System.IO.File.Exists(path));

                /* Serialize */
                CITSHelper.SerializeMessage(requestMessage);
                CITSHelper.SerializeMessage(response.ProcessFileRequestsResult);
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
