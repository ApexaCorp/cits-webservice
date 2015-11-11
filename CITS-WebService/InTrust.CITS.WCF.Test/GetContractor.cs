using System;
using System.Collections.Generic;
using System.ServiceModel.Security;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using InTrust.CITS.WCF.Test.InTrustCITSService;

namespace InTrust.CITS.WCF.Test
{
	[TestClass]
	public class GetContractor
	{
		[TestInitialize]
		public void Initialize() {
		}

        /// <summary>
        /// Returns producer, appointments, and related advisors, by Id.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
		public async Task RequestFullById()
		{
			var client = new CITSServiceClient();
            client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
            client.ClientCredentials.UserName.Password = "tPjVt6waKBQR";
			var requestMessage = new TXLife_Type() { 
				Version = "2.34.00",
				Items = new object[] {
					new TXLifeRequest_Type() {
						TransRefGUID = Guid.NewGuid().ToString(),
						TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "FullProducerWithAppointments" },
                        OLifE = new OLifE_Type() {
							Items = new object[] {
								new Party_Type() {
									id = "C32",
								}
							}
						},
					}
				},
			};

            // Debug
            TXLife_Type responseMessage = null;
            try {
                responseMessage = await client.ProcessMessageAsync(requestMessage);
            } catch (Exception)
            {
                throw;
            }
            
            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage);
            if (errorList.Count > 0)
            {
                Assert.Fail(Validate.DumpErrors(errorList));
            }

            /* Serialize */
            CITSHelper.SerializeMessage(requestMessage);
			CITSHelper.SerializeMessage(responseMessage);
		}

        /// <summary>
        /// Returns producer, appointments, and related advisors, by Broker (Selling) Code.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
		public async Task RequestFullBySellingCode()
		{
			var client = new CITSServiceClient();
			client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
			client.ClientCredentials.UserName.Password = "password";
			var requestMessage = new TXLife_Type()
			{
				Version = "2.34.00",
				Items = new object[] {
					new TXLifeRequest_Type() {
						TransRefGUID = Guid.NewGuid().ToString(),
						TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "FullProducerWithAppointments" },
                        OLifE = new OLifE_Type() {
							Items = new object[] {
								new Party_Type() {
									Producer =  new Producer_Type() {
										CarrierAppointment = new CarrierAppointment_Type[] {
											new CarrierAppointment_Type() {
												CompanyProducerID = "2"
											}
										}
									}
								}
							}
						},
					}
				},
			};
			var responseMessage = await client.ProcessMessageAsync(requestMessage);

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage);
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
		public async Task RequestUpdatedContractors()
		{
			var client = new CITSServiceClient();
			client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
			client.ClientCredentials.UserName.Password = "password";
			var requestMessage = new TXLife_Type()
			{
				Version = "2.34.00",
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
			var responseMessage = await client.ProcessMessageAsync(requestMessage);

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage);
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
            var client = new CITSServiceClient();
            client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
            client.ClientCredentials.UserName.Password = "password";
            var requestMessage = new TXLife_Type()
            {
                Version = "2.34.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ProducerWithAppointments" },
                        OLifE = new OLifE_Type() {
                            Items = new object[] {
                                new Party_Type() {
                                    id = "C32",
								}
                            }
                        },
                    }
                },
            };
            var responseMessage = await client.ProcessMessageAsync(requestMessage);

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage);
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
            var client = new CITSServiceClient();
            client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
            client.ClientCredentials.UserName.Password = "password";
            var requestMessage = new TXLife_Type()
            {
                Version = "2.34.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ProducerOnly" },
                        OLifE = new OLifE_Type() {
                            Items = new object[] {
                                new Party_Type() {
                                    id = "C32",
								}
                            }
                        },
                    }
                },
            };
            var responseMessage = await client.ProcessMessageAsync(requestMessage);

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage);
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
            var client = new CITSServiceClient();
            client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
            client.ClientCredentials.UserName.Password = "password";
            var requestMessage = new TXLife_Type()
            {
                Version = "2.34.00",
                Items = new object[] {
                    new TXLifeRequest_Type() {
                        TransRefGUID = Guid.NewGuid().ToString(),
                        TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                        InquiryView = new InquiryView_Type() { InquiryViewCode = "ProducerAndRelated" },
                        OLifE = new OLifE_Type() {
                            Items = new object[] {
                                new Party_Type() {
                                    id = "C32",
								}
                            }
                        },
                    }
                },
            };
            var responseMessage = await client.ProcessMessageAsync(requestMessage);

            // Feed errors
            var errorList = Validate.GetFeedErrors(responseMessage);
            if (errorList.Count > 0)
            {
                Assert.Fail(Validate.DumpErrors(errorList));
            }

            /* Serialize */
            CITSHelper.SerializeMessage(requestMessage);
            CITSHelper.SerializeMessage(responseMessage);
        }


        [TestMethod]
		public async Task UnauthorizedRequest()
		{
			var client = new CITSServiceClient();
			client.ClientCredentials.UserName.UserName = "carrier@bluesun.ca";
			client.ClientCredentials.UserName.Password = "wrongpassword";
			var requestMessage = new TXLife_Type()
			{
				Version = "2.34.00",
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
            TXLife_Type result = null;
            try
            {
                result = await client.ProcessMessageAsync(requestMessage);
                Assert.Fail("Should have failed with unauthorized.");
            }
            catch (SecurityAccessDeniedException ex)
            {
                Assert.IsTrue(String.Equals(ex.Message, "Access is denied."));
            }
            catch (MessageSecurityException)
            {
                Assert.IsTrue(true);
            }
        }
	}
}
