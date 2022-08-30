using Apexa.CITS.WCF.Test.CITSService;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.ServiceModel;
using System.Threading.Tasks;

namespace Apexa.CITS.WCF.Test.Tests.Contractor
{
    /// <summary>
    /// Summary description for Contractor
    /// </summary>
    [TestClass]
    public class GetContractorChanges
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
        /// Returns producer and appointments, without related advisors.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestUpdatedContractors()
        {
            using (var client = GetServiceClient())
            {
                var requestMessage = new TXLife_Type()
                {
                    Version = "2.35.00",
                    Items = new object[] {
                        new TXLifeRequest_Type() {
                            TransRefGUID = Guid.NewGuid().ToString(),
                            StartDate = DateTime.Now.Date.AddDays(-1),
                            StartDateSpecified = true,
                            EndDate = DateTime.Now.Date.AddDays(1),
                            EndDateSpecified = true,
                            TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
                            InquiryView = new InquiryView_Type() { InquiryViewCode = "ChangedProducerListing" },
                            TransExeDate = DateTime.Now.Date,
                            TransExeDateSpecified = true,
                            TransExeTime = new DateTime(DateTime.Now.TimeOfDay.Ticks),
                            TransExeTimeSpecified = true,
                        }
                    },
                };

                var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

                // Check for Success code
                Assert.AreEqual(((TXLifeResponse_Type)responseMessage.TXLife.Items[0]).TransResult.ResultCode.tc, "1");

                /* Serialize */
                CITSHelper.SerializeMessage(requestMessage);
                CITSHelper.SerializeMessage(responseMessage);
            }
        }

        /// <summary>
        /// Returns producer, appointments, and related advisors, by Id.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestContractorFullById()
        {
            using (var client = GetServiceClient())
            {
                var requestMessage = new TXLife_Type()
                {
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
                                        PartyKey = new PERSISTKEY() { Value = _contractorId, Persist = "Session" } // Value must be producer's APEXA ID
                                    }
                                }
                            },
                        }
                    },
                };
                var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

                // Check for Success code
                Assert.AreEqual(((TXLifeResponse_Type)responseMessage.TXLife.Items[0]).TransResult.ResultCode.tc, "1");

                /* Serialize */
                CITSHelper.SerializeMessage(requestMessage);
                CITSHelper.SerializeMessage(responseMessage);
            }
        }

        /// <summary>
        /// Returns producer details only.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestContractorOnlyById()
        {
            using (var client = GetServiceClient())
            {
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
                                        PartyKey = new PERSISTKEY() { Value = _contractorId, Persist = "Session" } // Value must be producer's APEXA ID
                                    }
                                }
                            },
                        }
                    },
                };
                var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

                // Check for Success code
                Assert.AreEqual(((TXLifeResponse_Type)responseMessage.TXLife.Items[0]).TransResult.ResultCode.tc, "1");

                /* Serialize */
                CITSHelper.SerializeMessage(requestMessage);
                CITSHelper.SerializeMessage(responseMessage);
            }
        }

        /// <summary>
        /// Returns producer and related advisors, without appointments.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestContractorAndRelatedById()
        {
            using (var client = GetServiceClient())
            {
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
                                    PartyKey = new PERSISTKEY() { Value = _contractorId, Persist = "Session" }, // Value must be producer's APEXA ID
                                    Item = new Person_Type() { FirstName = "Johnny Doe", HighestEducationLevel = new OLI_LU_EDULEVEL() {tc = "TMU"}}
                                },
                                //new Person_Type()
                                //{
                                //    //Education = new Education_Type[] { Major = "comp Sci" },
                                //    FirstName = "Johnny Doe"
                                //}
                            }
                        },
                    }
                },
                };
                var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

                // Check for Success code
                Assert.AreEqual(((TXLifeResponse_Type)responseMessage.TXLife.Items[0]).TransResult.ResultCode.tc, "1");

                /* Serialize */
                CITSHelper.SerializeMessage(requestMessage);
                CITSHelper.SerializeMessage(responseMessage);
            }
        }

        /// <summary>
        /// Returns producer and appointments, without related advisors.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task RequestContractorWithAppointmentsById()
        {
            using (var client = GetServiceClient())
            {
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
                                        PartyKey = new PERSISTKEY() { Value = _contractorId, Persist = "Session" } // Value must be producer's APEXA ID
                                    }
                                }
                            },
                        }
                    },
                };
                var responseMessage = await client.ProcessMessageAsync(new ProcessMessageRequest(requestMessage));

                // Check for Success code
                Assert.AreEqual(((TXLifeResponse_Type)responseMessage.TXLife.Items[0]).TransResult.ResultCode.tc, "1");

                /* Serialize */
                CITSHelper.SerializeMessage(requestMessage);
                CITSHelper.SerializeMessage(responseMessage);
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
