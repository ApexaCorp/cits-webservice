using System;
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
		[TestInitialize]
		public void Initialize() {
		}

		[TestMethod]
		public async Task RequestById()
		{
			var client = new CITSService.CITSServiceClient();
			client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
			client.ClientCredentials.UserName.Password = "password";
			var requestMessage = new TXLife_Type() { 
				Version = "2.34.00",
				Items = new object[] {
					new TXLifeRequest_Type() {
						TransRefGUID = Guid.NewGuid().ToString(),
						TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
						OLifE = new OLifE_Type() {
							Items = new object[] {
								new Party_Type() {
									id = "C1",
								}
							}
						},
					}
				},
			};
			var responseMessage = await client.ProcessMessageAsync(requestMessage);

			/* Serialize */
			CITSHelper.SerializeMessage(requestMessage);
			CITSHelper.SerializeMessage(responseMessage);
		}

		[TestMethod]
		public async Task RequestBySellingCode()
		{
			var client = new CITSService.CITSServiceClient();
			client.ClientCredentials.UserName.UserName = "cits.carrier@bluesun.ca";
			client.ClientCredentials.UserName.Password = "password";
			var requestMessage = new TXLife_Type()
			{
				Version = "2.34.00",
				Items = new object[] {
					new TXLifeRequest_Type() {
						TransRefGUID = Guid.NewGuid().ToString(),
						TransType = new OLI_LU_TRANS_TYPE_CODES() { tc = "228", Value = "Producer Inquiry" },
						OLifE = new OLifE_Type() {
							Items = new object[] {
								new Party_Type() {
									Producer =  new Producer_Type() {
										CarrierAppointment = new CarrierAppointment_Type[] {
											new CarrierAppointment_Type() {
												CompanyProducerID = "12345"
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

			/* Serialize */
			CITSHelper.SerializeMessage(requestMessage);
			CITSHelper.SerializeMessage(responseMessage);
		}


		[TestMethod]
		public async Task RequestUpdatedContractors()
		{
			var client = new CITSService.CITSServiceClient();
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
						TransSubType = new TRANS_SUBTYPE_CODES() { tc = "22817", Value = "Changed Producer Information Only" },
					}
				},
			};
			var responseMessage = await client.ProcessMessageAsync(requestMessage);

			/* Serialize */
			CITSHelper.SerializeMessage(requestMessage);
			CITSHelper.SerializeMessage(responseMessage);
		}


		[TestMethod]
		public async Task UnauthorizedRequest()
		{
			var client = new CITSService.CITSServiceClient();
			client.ClientCredentials.UserName.UserName = "carrier@bluesun.ca";
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
						TransSubType = new TRANS_SUBTYPE_CODES() { tc = "22817", Value = "Changed Producer Information Only" },
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
		}
	}
}
