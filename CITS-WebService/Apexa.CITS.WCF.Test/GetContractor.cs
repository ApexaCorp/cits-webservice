﻿using System;
using System.Collections.Generic;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Apexa.CITS.WCF.Test.CITSService;

namespace Apexa.CITS.WCF.Test
{
	[TestClass]
	public class GetContractor
	{
        /// <summary>
        /// Replace these with your credentials.
        /// </summary>
        private readonly string CLIENT_USERNAME = "cits.username@bluesun.ca"; // CLIENT_USERNAME
        private readonly string CLIENT_PASSWORD = "password"; // CLIENT_PASSWORD

        [TestInitialize]
		public void Initialize() {
		}
        
        /// <summary>
        /// Returns producer and appointments, without related advisors.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestUpdatedContractors()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;

            var requestMessage = new TXLife_Type()
            {
                Version = "2.35.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        StartDate = DateTime.Now.Date.AddDays(-6),
                        StartDateSpecified = true,
                        EndDate = DateTime.Now.Date.AddDays(1),
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

            var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage.TXLife);
            if (errorList.Count > 0)
            {
                Assert.Fail(Validate.DumpErrors(errorList));
            }

            /* Serialize */
            CITSHelper.SerializeMessage(requestMessage);
            CITSHelper.SerializeMessage(responseMessage);
        }

        /// <summary>
        /// Returns producer, appointments, and related advisors, by Id.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
		public async Task RequestFullById()
		{
			var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;
            var requestMessage = new TXLife_Type() {
				Version = "2.35.00",
				Items = new object[] {
					new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "FullProducerWithAppointments" },
                        TransExeDate = DateTime.Now.Date,
                        TransExeDateSpecified = true,
                        TransExeTime = new DateTime(DateTime.Now.TimeOfDay.Ticks),
                        TransExeTimeSpecified = true,
                        OLifE = new OLifE_Type() {
                            Items = new object[] {
                                new Party_Type() {
                                    PartyKey = new PERSISTKEY() { Value = "6", Persist = "Session" } // Value must be producer's APEXA ID
                                }
                            }
                        },
                    }
				},
			};
            var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage.TXLife);
            if (errorList.Count > 0)
            {
                Assert.Fail(Validate.DumpErrors(errorList));
            }

			/* Serialize */
			CITSHelper.SerializeMessage(requestMessage);
			CITSHelper.SerializeMessage(responseMessage);
		}

        /// <summary>
        /// Returns producer and appointments, without related advisors.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestProducerWithAppointmentsById()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;
            var requestMessage = new TXLife_Type()
            {
                Version = "2.35.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ProducerWithAppointments" },
                        TransExeDate = DateTime.Now.Date,
                        TransExeDateSpecified = true,
                        TransExeTime = new DateTime(DateTime.Now.TimeOfDay.Ticks),
                        TransExeTimeSpecified = true,
                        OLifE = new OLifE_Type() {
                            Items = new object[] {
                                new Party_Type() {
                                    PartyKey = new PERSISTKEY() { Value = "6", Persist = "Session" } // Value must be producer's APEXA ID
                                }
                            }
                        },
                    }
                },
            };
            var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage.TXLife);
            if (errorList.Count > 0)
            {
                Assert.Fail(Validate.DumpErrors(errorList));
            }

            /* Serialize */
            CITSHelper.SerializeMessage(requestMessage);
            CITSHelper.SerializeMessage(responseMessage);
        }

        /// <summary>
        /// Returns producer details only.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestProducerOnlyById()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;

            var requestMessage = new TXLife_Type()
            {
                Version = "2.35.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ProducerOnly" },
                        TransExeDate = DateTime.Now.Date,
                        TransExeDateSpecified = true,
                        TransExeTime = new DateTime(DateTime.Now.TimeOfDay.Ticks),
                        TransExeTimeSpecified = true,
                        OLifE = new OLifE_Type() {
                            Items = new object[] {
                                new Party_Type() {
                                    PartyKey = new PERSISTKEY() { Value = "6", Persist = "Session" } // Value must be producer's APEXA ID
                                }
                            }
                        },
                    }
                },
            };
            var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage.TXLife);
            if (errorList.Count > 0)
            {
                Assert.Fail(Validate.DumpErrors(errorList));
            }

            /* Serialize */
            CITSHelper.SerializeMessage(requestMessage);
            CITSHelper.SerializeMessage(responseMessage);
        }

        /// <summary>
        /// Returns producer and related advisors, without appointments.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestProducerAndRelatedById()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;

            var requestMessage = new TXLife_Type()
            {
                Version = "2.35.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ProducerAndRelated" },
                        TransExeDate = DateTime.Now.Date,
                        TransExeDateSpecified = true,
                        TransExeTime = new DateTime(DateTime.Now.TimeOfDay.Ticks),
                        TransExeTimeSpecified = true,
                        OLifE = new OLifE_Type() {
                            Items = new object[] {
                                new Party_Type() {
                                    PartyKey = new PERSISTKEY() { Value = "6", Persist = "Session" } // Value must be producer's APEXA ID
                                }
                            }
                        },
                    }
                },
            };
            var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage.TXLife);
            if (errorList.Count > 0)
            {
                Assert.Fail(Validate.DumpErrors(errorList));
            }

            /* Serialize */
            CITSHelper.SerializeMessage(requestMessage);
            CITSHelper.SerializeMessage(responseMessage);
        }

        /// <summary>
        /// Returns producer APEXA Ids for a given internal Id.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestApexaIds()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;

            var request = new NonCITSRequest()
            {
                requestType = "RequestContractorApexaIds",
                id = "xyz2015"
            };

            var response = await client.ProcessNonCITSMessageAsync(request);

            List<Result> results = new List<Result>(response.ProcessNonCITSMessageResult.items);
            Assert.IsTrue(results.Find(n => n.id == "21") != null);

            /* Serialize */
            CITSHelper.SerializeMessage(request);
            CITSHelper.SerializeMessage(response);
        }

        /// <summary>
        /// Returns contractor Internal Ids for a given APEXA Id.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestInternalIds()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;

            var request = new NonCITSRequest()
            {
                requestType = "RequestContractorInternalIds",
                id = "21"
            };

            var response = await client.ProcessNonCITSMessageAsync(request);

            List<Result> results = new List<Result>(response.ProcessNonCITSMessageResult.items);
            Assert.IsTrue(results.Find(n => n.id == "xyz2015") != null);

            /* Serialize */
            CITSHelper.SerializeMessage(request);
            CITSHelper.SerializeMessage(response);
        }

        /// <summary>
        /// Returns producer, appointments, and related advisors, by Broker (Selling) Code.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestApexaIdsBySellingCode()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;

            var request = new NonCITSRequest()
            {
                requestType = "RequestContractorApexaIdsBySellingCode",
                id = "123"
            };

            var response = await client.ProcessNonCITSMessageAsync(request);

            List<Result> results = new List<Result>(response.ProcessNonCITSMessageResult.items);
            Assert.IsTrue(results.Find(n => n.id == "21") != null);

            /* Serialize */
            CITSHelper.SerializeMessage(request);
            CITSHelper.SerializeMessage(response);
        }

        /// <summary>
        /// Returns a file for a given APEXA file download url.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestFiles()
        {
            var client = new CITSService.CITSServiceClient();
            client.ClientCredentials.UserName.UserName = CLIENT_USERNAME;
            client.ClientCredentials.UserName.Password = CLIENT_PASSWORD;

            var requestMessage = new FileRequest()
            {
                items = new FileURL[]
                {
                    new FileURL() { url = "http://host:12345//documents/fulfillment/ABCDEFGH-1234-ABCD-1234-000000000001" },
                    new FileURL() { url = "http://host:12345//documents/banking/ABCDEFGH-1234-ABCD-1234-000000000002" }
                }
            };

            var response = await client.ProcessFileRequestsAsync(requestMessage);

            Assert.IsTrue(response.ProcessFileRequestsResult != null);
            Assert.IsTrue(response.ProcessFileRequestsResult.itemsField.Length == 2);
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
        
        [TestMethod]
		public async Task UnauthorizedRequest()
		{
			var client = new CITSService.CITSServiceClient();
			client.ClientCredentials.UserName.UserName = "cits.username@domain.ca";
			client.ClientCredentials.UserName.Password = "wrongpassword";

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
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ChangedProducerListing" }
                    }
                },
			};

            try
			{
                var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));
                var result = responseMessage.TXLife;
				Assert.Fail("Should have failed with unauthorized.");
			}
			catch (SecurityAccessDeniedException ex)
			{
				Assert.IsTrue(String.Equals(ex.Message, "Access is denied."));
			}
		}
	}
}
