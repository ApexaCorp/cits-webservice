using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

using Apexa.CITS.WCF.Test.CITSService;

namespace Apexa.CITS.WCF.Test
{
    internal static class Validate
    {
        internal static List<string> GetFeedErrors(CITSService.TXLife_Type response)
        {
            List<string> errors = new List<string>();

            var advisors = response.Items.OfType<Party_Type>().Where(p => p.PartyTypeCode.Value == "Person");
            foreach (var advisor in advisors)
            {
                errors.AddRange(AdvisorData(response, advisor));
                errors.AddRange(AdvisorIdentity(advisor));
                errors.AddRange(Addresses(advisor.Address, "Advisor"));
                errors.AddRange(CLHIA(response, advisor));
                errors.AddRange(CreditRating(advisor));
                errors.AddRange(Criminal(advisor));
                errors.AddRange(Debts(response, response.Items.OfType<Holding_Type>().Where(p => p.id.StartsWith("DEBT")).ToArray(), "Advisor"));
                errors.AddRange(EOCoverage(response, advisor));
                errors.AddRange(Education(advisor));
                errors.AddRange(Licence(response, advisor));
                errors.AddRange(Phones(advisor.Phone, "Advisor"));
                errors.AddRange(Reference(response));
                errors.AddRange(Supervision(advisor));
                if (advisor.Producer.CarrierAppointment != null && advisor.Producer.CarrierAppointment.Count() > 0) { errors.AddRange(Appointments(response, advisor)); }
            }

            var organizations = response.Items.OfType<Party_Type>().Where(p => p.PartyTypeCode.Value == "Organization" && p.id.StartsWith("C"));
            foreach (var org in organizations)
            {
                errors.AddRange(OrganizationData(response, org));
                errors.AddRange(OrganizationIdentity(org));
                errors.AddRange(Addresses(org.Address, "Organization"));
                errors.AddRange(Debts(response, response.Items.OfType<Holding_Type>().Where(p => p.id.StartsWith("DEBT")).ToArray(), "Organization"));
                errors.AddRange(EOCoverage(response, org));
                errors.AddRange(Licence(response, org));
                errors.AddRange(Phones(org.Phone, "Organization"));
                errors.AddRange(Shareholder(response, org));
                errors.AddRange(Supervision(org));
                
                if (org.Producer.CarrierAppointment != null && org.Producer.CarrierAppointment.Count() > 0) { errors.AddRange(Appointments(response, org)); }
            }

            var partners = response.Items.OfType<Party_Type>().Where(p => p.PartyTypeCode.Value == "Organization" && p.id.StartsWith("P"));
            foreach (var partner in partners)
            {
                errors.AddRange(PartnerData(partner));
            }

            var shareholders = response.Items.OfType<Party_Type>().Where(p => p.PartyTypeCode.Value == "Person" && p.id.StartsWith("O"));
            foreach (var shareholder in shareholders)
            {
                errors.AddRange(Shareholder(response, shareholder));
            }

            return errors;
        }

        internal static IEnumerable<string> OrganizationData(TXLife_Type response, Party_Type party)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(party.PartyTypeCode.tc)) { errors.Add("organization partyTypeCode tc"); }
            if (party.Item == null) { errors.Add("organization item"); }
            else
            {
                if (party.Item.GetType() != typeof(Organization_Type)) { errors.Add("organization item type"); return errors; }
                var org = party.Item as Organization_Type;
                if (string.IsNullOrWhiteSpace(party.FullName)) { errors.Add("organization fullname"); }
                if (string.IsNullOrWhiteSpace(party.id)) { errors.Add("organization id"); }
            }
            
            return errors;
        }

        internal static IEnumerable<string> AdvisorData(TXLife_Type response, Party_Type party)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(party.PartyTypeCode.tc)) { errors.Add("advisor partyTypeCode tc"); }
            if (party.Item == null) { errors.Add("advisor item"); }
            else
            {
                if (party.Item.GetType() != typeof(Person_Type)) { errors.Add("advisor item type"); return errors; }
                var person = party.Item as Person_Type;
                if (string.IsNullOrWhiteSpace(person.FirstName)) errors.Add("advisor firstname");
                if (string.IsNullOrWhiteSpace(person.LastName)) errors.Add("advisor lastname");
            }
            errors.AddRange(Email(party.EMailAddress, "Advisor"));

            return errors;
        }
        
        internal static IEnumerable<string> Appointments(TXLife_Type response, Party_Type contractor)
        {
            List<string> errors = new List<string>();
            foreach (var app in contractor.Producer.CarrierAppointment)
            {
                if (string.IsNullOrWhiteSpace(app.id)) { errors.Add("Appointment id"); }
                if (string.IsNullOrWhiteSpace(app.PartyID)) { errors.Add("Appointment PartyID"); }
                if (LUObjNotValid(app.CarrierApptStatus)) { errors.Add("Appointment CarrierApptStatus"); }
                if (string.IsNullOrWhiteSpace(app.CompanyProducerID)) { errors.Add("Appointment CompanyProducerID"); }
                foreach (var govId in app.GovtIDInfo)
                {
                    if (LUObjNotValid(govId)) { errors.Add("Appointment GovtIDInfo"); }
                }
                foreach (var assocInfo in app.AssocCarrierApptInfo)
                {
                    if (string.IsNullOrWhiteSpace(assocInfo.CompanyProducerID)) { errors.Add("Appointment CompanyProducerID"); }
                    if (LUObjNotValid(assocInfo.CarrierApptStatus)) { errors.Add("Appointment CarrierApptStatus"); }
                    if (string.IsNullOrWhiteSpace(assocInfo.CarrierAppointmentID)) { errors.Add("Appointment CarrierAppointmentID"); }
                    if (string.IsNullOrWhiteSpace(assocInfo.PartyID)) { errors.Add("Appointment PartyID"); }
                }

                // Referral
                foreach (var referral in app.ReferralInfo)
                {
                    if (!referral.PartyID.StartsWith("REF")) { errors.Add("Appointment referral PartyID"); }
                    var responseReferral = response.Items.OfType<Party_Type>().Where(p => p.id == referral.PartyID).FirstOrDefault();
                    if (responseReferral == null) { errors.Add("Appointment referral lifeItem"); }
                    else
                    {
                        if (LUObjNotValid(responseReferral.PartyTypeCode)) { errors.Add("Appointment referral PartyTypeCode"); }
                        if (responseReferral.Item == null || !(responseReferral.Item is Person_Type)) { errors.Add("Appointment referral Item"); }
                        if (string.IsNullOrWhiteSpace(responseReferral.FullName)) { errors.Add("Appointment referral FullName"); }
                    }
                }

                // Payment
                foreach (var distrib in app.DistributionAgreementInfo)
                {
                    if (distrib.BankingInfoID != null)
                    {
                        var holding = response.Items.OfType<Holding_Type>().Where(p => p.id == distrib.BankingInfoID).FirstOrDefault();
                        if (holding == null) { errors.Add("Appointment payment holding"); }
                        else
                        {
                            if (holding.Banking == null) { errors.Add("Appointment payment holding banking"); }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(holding.Banking.AccountNumber)) { errors.Add("Appointment payment holding banking AccountNumber"); }
                                if (string.IsNullOrWhiteSpace(holding.Banking.TransitNumber)) { errors.Add("Appointment payment holding banking TransitNumber"); }
                                if (string.IsNullOrWhiteSpace(holding.Banking.InstitutionNumber)) { errors.Add("Appointment payment holding banking InstitutionNumber"); }
                            }
                            if (holding.Attachment == null) { errors.Add("Appointment payment holding Attachment"); }
                            else
                            {
                                foreach (var attachment in holding.Attachment)
                                {
                                    errors.AddRange(Attachment(attachment, "Appointment payment holding banking"));
                                }
                            }
                        }
                    }
                    if (distrib.CheckMailingID != null)
                    {
                        var address = response.Items.OfType<Address_Type>().Where(p => p.id == distrib.CheckMailingID).FirstOrDefault();
                        if (address == null) { errors.Add("Appointment payment address"); }
                        else { errors.AddRange(Address(address, "Appointment payment")); }
                    }
                    if (LUObjNotValid(distrib.PaymentForm)) { errors.Add("Appointment PaymentForm"); }
                }

                // Hierarchy
                foreach (var distribLevel in app.DistributionLevel)
                {
                    if (string.IsNullOrWhiteSpace(distribLevel.PartyID)) { errors.Add("Appointment hierarchy partyId"); }
                    if (string.IsNullOrWhiteSpace(distribLevel.DistributionLevelValue)) { errors.Add("Appointment hierarchy DistributionLevelValue"); }
                }

                // Consolidations
                foreach (var consolidation in app.ConsolidationInfo)
                {
                    if (string.IsNullOrWhiteSpace(consolidation.CarrierAppointmentID)) { errors.Add("Appointment consolidation CarrierAppointmentID"); }
                }

                // Transfers
                foreach (var transfer in app.TransferInfo)
                {
                    if (string.IsNullOrWhiteSpace(transfer.CarrierAppointmentID)) { errors.Add("Appointment transfer CarrierAppointmentID"); }
                }
                                
                // Debts
                foreach (var debt in app.DebtInfo)
                {
                    errors.AddRange(Debt(response, response.Items.OfType<Holding_Type>().Where(p => p.id == debt.HoldingID).FirstOrDefault(), "Appointment"));
                }

                // Requirements
                var initialReq = response.Items.OfType<RequirementInfo_Type>().Where(p => p.ReqCode.tc == "936").FirstOrDefault();
                if (initialReq == null) { errors.Add("Appointment Requirements Initial Business"); }

                foreach (var req in app.RequirementInfo)
                {
                    if (string.IsNullOrWhiteSpace(req.FulfillerPartyID)) { errors.Add("Appointent Requirements FulfillerPartyID"); }
                    if (req.Attachment == null) { errors.Add("Appointment Requirements Attachment"); }
                    foreach (var attachment in req.Attachment)
                    {
                        errors.AddRange(Attachment(attachment, "Appointment Requirements"));

                        foreach (var signature in attachment.SignatureInfo)
                        {
                            if (string.IsNullOrWhiteSpace(signature.DelegatedSignerPartyID)) { errors.Add("Appointent Requirements attachment DelegatedSignerPartyID"); }
                            var signer = response.Items.OfType<Party_Type>().Where(p => p.id == signature.DelegatedSignerPartyID).FirstOrDefault();
                            if (signer == null) { errors.Add("Appointent Requirements attachment signer"); }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(signer.id)) { errors.Add("Appointent Requirements attachment signer id"); }
                                if (string.IsNullOrWhiteSpace(signer.FullName)) { errors.Add("Appointent Requirements attachment signer FullName"); }
                                errors.AddRange(Email(signer.EMailAddress, "Appointment Requirements attachment signer"));
                                if (LUObjNotValid(signer.PartyTypeCode)) { errors.Add("Appointent Requirements attachment signer PartyTypeCode"); }
                                if (signer.Item == null) { errors.Add("Appointent Requirements attachment signer Item"); }
                            }
                        }
                    }
                }

                // CustomQuestions
                var formInstance = response.Items.OfType<FormInstance_Type>().Where(p => p.RelatedObjectID == app.id).FirstOrDefault();
                if (formInstance == null) { errors.Add("Appointent CustomQuestions formInstance"); }
                if (string.IsNullOrWhiteSpace(formInstance.id)) { errors.Add("Appointent CustomQuestions formInstance id"); }
                if (LUObjNotValid(formInstance.RelatedObjectType)) { errors.Add("Appointent CustomQuestions formInstance RelatedObjectType"); }
                if (formInstance.FormResponse == null) { errors.Add("Appointent CustomQuestions formInstance FormResponse"); }
                foreach (var formResponse in formInstance.FormResponse)
                {
                    if (string.IsNullOrWhiteSpace(formResponse.QuestionNumber)) { errors.Add("Appointent CustomQuestions formInstance FormResponse QuestionNumber"); }
                    if (string.IsNullOrWhiteSpace(formResponse.ResponseText)) { errors.Add("Appointent CustomQuestions formInstance FormResponse QuestionNumber"); }
                }
            }

            return errors;
        }

        internal static IEnumerable<string> PartnerData(Party_Type party)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(party.id)) { errors.Add("partner id"); }
            if (string.IsNullOrWhiteSpace(party.FullName)) { errors.Add("partner FullName"); }

            if (party.Item == null || party.Item.GetType() != typeof(Organization_Type)) { errors.Add("partner item type"); }
            if (LUObjNotValid(party.PartyTypeCode)) { errors.Add("partnerPartyTypeCode"); }
           
            return errors;
        }
        
        internal static IEnumerable<string> OrganizationIdentity(Party_Type party)
        {
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(party.GovtID)) { errors.Add("organization GovtID"); }
            if (LUObjNotValid(party.GovtIDTC)) { errors.Add("organization GovtIDTC"); }
            foreach (var id in party.GovtIDInfo)
            {
                if (LUObjNotValid(id)) { errors.Add("organization GovtIDInfo"); }
            }
            foreach (var attachment in party.Attachment)
            {
                errors.AddRange(Attachment(attachment, "organizationIdentity"));
            }

            return errors;
        }

        internal static IEnumerable<string> Attachment(Attachment_Type attachment, string module)
        {
            List<string> errors = new List<string>();
            if (LUObjNotValid(attachment.AttachmentBasicType)) { errors.Add(module + " attachment AttachmentBasicType"); }
            if (LUObjNotValid(attachment.AttachmentType)) { errors.Add(module + " attachment AttachmentType"); }
            if (LUObjNotValid(attachment.AttachmentLocation)) { errors.Add(module + " attachment AttachmentLocation"); }
            if (LUObjNotValid(attachment.AttachmentData)) { errors.Add(module + " attachment AttachmentData"); }
            if (string.IsNullOrWhiteSpace(attachment.FileName)) { errors.Add(module + " attachment FileName"); }
            return errors;
        }

        internal static IEnumerable<string> AdvisorIdentity(Party_Type party)
        {
            List<string> errors = new List<string>();
            
            var person = party.Item as Person_Type;
            if (party.PriorName != null)
            {
                foreach (var prior in party.PriorName)
                {
                    if (string.IsNullOrWhiteSpace(prior.FirstName)) { errors.Add("advisor identity firstName"); }
                    if (string.IsNullOrWhiteSpace(prior.LastName)) { errors.Add("advisor identity lastName"); }
                    if (LUObjNotValid(prior.NameType)) { errors.Add("advisor identity nameType"); }
                }
            }

            if (party.Producer != null)
            {
                if (LUObjNotValid(party.Producer.PrefLanguage)) { errors.Add("identity prefLanguage"); }
            }
            return errors;
        }

        internal static IEnumerable<string> Addresses(Address_Type[] addresses, string module)
        {
            List<string> errors = new List<string>();

            foreach (var address in addresses)
            {
                errors.AddRange(Address(address, module));
            }

            return errors;
        }

        internal static IEnumerable<string> Address(Address_Type address, string module)
        {
            List<string> errors = new List<string>();
            if (LUObjNotValid(address.AddressTypeCode)) { errors.Add(module + " addressTypeCode"); }
            if (string.IsNullOrWhiteSpace(address.Line1)) { errors.Add(module + " address line1"); }
            if (string.IsNullOrWhiteSpace(address.Line2)) { errors.Add(module + " address line2"); }
            if (string.IsNullOrWhiteSpace(address.City)) { errors.Add(module + " address city"); }
            if (LUObjNotValid(address.AddressStateTC)) { errors.Add(module + " addressStateTC"); }
            if (LUObjNotValid(address.AddressCountryTC)) { errors.Add(module + " AddressCountryTC"); }
            if (string.IsNullOrWhiteSpace(address.Zip)) { errors.Add(module + " address zip"); }
            return errors;
        }

        internal static IEnumerable<string> CLHIA(TXLife_Type response, Party_Type contractor)
        {
            List<string> errors = new List<string>();
            
            long contractorEntityId;
            if (!long.TryParse(Regex.Match(contractor.id, "C([0-9]+)").Groups[1].Value, out contractorEntityId)) { errors.Add("CLHIA contractorEntityId"); }
            else
            {
                if (contractor.Producer != null)
                {
                    if (contractor.Producer.NationApproval == null) { errors.Add("CLHIA producer nationalApproval"); }
                    else
                    {
                        foreach (var approval in contractor.Producer.NationApproval)
                        {
                            if (LUObjNotValid(approval.Nation)) { errors.Add("CLHIA producer nationalApproval"); }
                        }
                    }
                }
            }

            var formInstance = response.Items.OfType<FormInstance_Type>().FirstOrDefault();
            if (formInstance != null) 
            {
                if (string.IsNullOrWhiteSpace(formInstance.id)) { errors.Add("CLHIA formInstance id"); }
                if (formInstance.FormResponse == null) { errors.Add("CLHIA formResponse"); }
                else
                {
                    foreach (var formResponse in formInstance.FormResponse)
                    {
                        if (string.IsNullOrWhiteSpace(formResponse.QuestionNumber)) { errors.Add("CLHIA formResponse questionNumber"); }
                        if (string.IsNullOrWhiteSpace(formResponse.ResponseCode)) { errors.Add("CLHIA formResponse ResponseCode"); }
                        if (string.IsNullOrWhiteSpace(formResponse.ResponseText)) { errors.Add("CLHIA formResponse ResponseText"); }
                        if (string.IsNullOrWhiteSpace(formResponse.ResponseData)) { errors.Add("CLHIA formResponse ResponseData"); }
                    }
                }
                var relation = response.Items.OfType<Relation_Type>().FirstOrDefault(p => p.OriginatingObjectID == contractor.id && p.RelatedObjectID == formInstance.id);
                errors.AddRange(Relation(relation, "CLHIA formInstance"));
            }

            return errors;
        }

        internal static IEnumerable<string> CreditRating(Party_Type contractor)
        {
            List<string> errors = new List<string>();
            if (contractor.RatingAgencyInfo != null) 
            {
                foreach(var info in contractor.RatingAgencyInfo)
                {
                    if (LUObjNotValid(info.RatingSource)) { errors.Add("contractor creditRating ratingSource"); }
                    if (string.IsNullOrWhiteSpace(info.RatingValue)) errors.Add("contractor creditRating RatingValue");
                }
            }

            return errors;
        }

        internal static IEnumerable<string> Criminal(Party_Type contractor)
        {
            List<string> errors = new List<string>();
            if (contractor.Risk != null)
            {
                if (contractor.Risk.CriminalConviction == null) { errors.Add("contractor criminal"); }
                foreach (var conviction in contractor.Risk.CriminalConviction)
                {
                    if (string.IsNullOrWhiteSpace(conviction.CrimeDescription)) { errors.Add("contractor criminal CrimeDescription"); }
                }
            }

            return errors;
        }

        internal static IEnumerable<string> Debts(TXLife_Type response, Holding_Type[] debts, string module)
        {
            List<string> errors = new List<string>();
            foreach (var debt in debts)
            {
                errors.AddRange(Debt(response, debt, module));
            }
            return errors;
        }

        internal static IEnumerable<string> Debt(TXLife_Type response, Holding_Type debt, string module)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(debt.id)) { errors.Add(module + " debt id"); }
            foreach (var loan in debt.Loan)
            {
                if (string.IsNullOrWhiteSpace(loan.FinancialInstitutionPartyID)) { errors.Add(module + " debt loan FinancialInstitutionPartyID"); }
                if (loan.LoanAmtSpecified && loan.LoanAmt == 0) { errors.Add(module + " debt loan LoanAmt"); }
                if (LUObjNotValid(loan.LoanReason)) { errors.Add(module + " debt loan LoanReason"); }
                if (string.IsNullOrWhiteSpace(loan.LoanReasonDesc)) { errors.Add(module + " debt loan LoanReasonDesc"); }
            }

            long debtId;
            if (!long.TryParse(Regex.Match(debt.id, "DEBT([0-9]+)").Groups[1].Value, out debtId)) { errors.Add(module + " debt id"); }
            string debtHolderId = string.Format("DEBTHOLDER{0}", debtId);

            var debtholder = response.Items.OfType<Party_Type>().FirstOrDefault(p => p.id == debtHolderId);
            if (debtholder == null) { errors.Add(module + " debt debtholder"); }
            else
            {
                if (LUObjNotValid(debtholder.PartyTypeCode)) { errors.Add(module + " debt debtholder partyTypeCode"); }
                if (debtholder.Item == null || !(debtholder.Item is Organization_Type)) { errors.Add(module + " debt debtholder Item"); }
                if (string.IsNullOrWhiteSpace(debtholder.FullName)) { errors.Add(module + " debt debtholder fullName"); }

                var relation = response.Items.OfType<Relation_Type>().FirstOrDefault(p => p.OriginatingObjectID == debt.id);
                errors.AddRange(Relation(relation, module + " Debt Debtholder"));
            }
            return errors;
        }

        internal static IEnumerable<string> EOCoverage(TXLife_Type response, Party_Type contractor)
        {
            List<string> errors = new List<string>();
            var coverages = response.Items.OfType<Holding_Type>().Where(p => p.HoldingTypeCode.tc == "40");
            foreach (var coverage in coverages)
            {
                if (string.IsNullOrWhiteSpace(coverage.id)) { errors.Add("contractor coverage id"); }
                if (coverage.Policy == null) { errors.Add("contractor coverage policy"); }
                else
                {
                    if (string.IsNullOrWhiteSpace(coverage.Policy.CarrierPartyID)) { errors.Add("contractor coverage CarrierPartyID"); }
                    if (string.IsNullOrWhiteSpace(coverage.Policy.PolNumber)) { errors.Add("contractor coverage PolNumber"); }
                    if (LUObjNotValid(coverage.Policy.ProductType)) { errors.Add("contractor coverage productType"); }
                    if (coverage.Policy.PolicyValueSpecified && coverage.Policy.PolicyValue == 0) { errors.Add("contractor coverage PolicyValue"); }
                    if (LUObjNotValid(coverage.Policy.PolicyStatus)) { errors.Add("contractor coverage PolicyStatus"); }

                    var coverageHolder = response.Items.OfType<Party_Type>().FirstOrDefault(p => p.id == coverage.Policy.CarrierPartyID);
                    if (coverageHolder == null) { errors.Add("contractor coverage coverageHolder"); }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(coverageHolder.id)) { errors.Add("contractor coverage coverageHolder id"); }
                        if (LUObjNotValid(coverageHolder.PartyTypeCode)) { errors.Add("contractor coverage coverageHolder partyTypeCode"); }
                        if (coverageHolder.Item == null || !(coverageHolder.Item is Organization_Type)) { errors.Add("contractor coverage coverageHolder Item"); }
                        if (string.IsNullOrWhiteSpace(coverageHolder.FullName)) { errors.Add("contractor coverage coverageHolder fullName"); }
                    }
                }

                if (coverage.Attachment == null) { errors.Add("contractor coverage attachment"); }
                else
                {
                    foreach (var attachment in coverage.Attachment)
                    {
                        errors.AddRange(Attachment(attachment, "Contractor coverage"));
                    }
                }

                var coverageInfo = contractor.Producer.EOCoverageInfo.Where(p => p.HoldingID == coverage.id).FirstOrDefault();
                if (coverageInfo == null) { errors.Add("contractor coverage producer coverageInfo"); }
            }

            return errors;
        }

        internal static IEnumerable<string> Education(Party_Type contractor)
        {
            List<string> errors = new List<string>();
            if (contractor.Producer.DesignationInfo != null)
            {
                foreach (var designation in contractor.Producer.DesignationInfo)
                {
                    if (LUObjNotValid(designation.DesignationType)) { errors.Add("contractor education designationType"); }
                    if (string.IsNullOrWhiteSpace(designation.DesignationYear)) { errors.Add("contractor education DesignationYear"); }
                    if (designation.DesignationType != null && designation.DesignationType.tc == "2147483647" && string.IsNullOrWhiteSpace(designation.DesignationDesc)) errors.Add("contractor education DesignationDesc");
                }
            }

            if (contractor.Item != null  && contractor.Item is Person_Type)
            {
                var person = contractor.Item as Person_Type;
                if (LUObjNotValid(person.HighestEducationLevel)) { errors.Add("contractor education HighestEducationLevel"); }
                if (person.Education != null)
                {
                    foreach (var education in person.Education)
                    {
                        if (string.IsNullOrWhiteSpace(education.Major)) { errors.Add("contractor education Major"); }
                        if (string.IsNullOrWhiteSpace(education.ProviderOrSchool)) { errors.Add("contractor education ProviderOrSchool"); }
                    }
                } 
            }

            return errors;
        }

        internal static IEnumerable<string> Email(EMailAddress_Type[] emails, string module)
        {
            List<string> errors = new List<string>();
            if (emails == null) { errors.Add(module + " emailAddress"); }
            else
            {
                foreach (var email in emails)
                {
                    if (string.IsNullOrWhiteSpace(email.AddrLine)) errors.Add(module + " emailAddress addrLine");
                }
            }
            return errors;
        }

        internal static IEnumerable<string> Licence(TXLife_Type response, Party_Type contractor)
        {
            List<string> errors = new List<string>();

            if (contractor.Producer.License != null)
            {
                foreach (var licence in contractor.Producer.License)
                {
                    if (string.IsNullOrWhiteSpace(licence.AgencyAffiliationID)) { errors.Add("contractor licence AgencyAffiliationID"); }
                    else
                    {
                        var sponsor = response.Items.OfType<Party_Type>().FirstOrDefault(p => p.id == licence.AgencyAffiliationID);
                        if (sponsor == null) { errors.Add("contractor licence sponsor"); }
                        else
                        {
                            if (LUObjNotValid(sponsor.PartyTypeCode)) { errors.Add("contractor licence sponsor PartyTypeCode"); }
                            if (!(sponsor.Item is Organization_Type)) { errors.Add("contractor licence sponsor Item"); }
                            if (string.IsNullOrWhiteSpace(sponsor.FullName)) { errors.Add("contractor licence sponsor Item"); }
                        }
                    }
                    if (string.IsNullOrWhiteSpace(licence.LicenseNum)) { errors.Add("contractor licence LicenceNum"); }
                    if (LUObjNotValid(licence.LicenseType)) { errors.Add("contractor licence LicenceType"); }
                    if (LUObjNotValid(licence.LicenseState)) { errors.Add("contractor licence LicenceState"); }
                    if (string.IsNullOrWhiteSpace(licence.LevelDesc)) { errors.Add("contractor licence LevelDesc"); }
                }
            }
            
            return errors;
        }

        internal static IEnumerable<string> Phones(Phone_Type[] phones, string module)
        {
            List<string> errors = new List<string>();
            foreach (var phone in phones)
            {
                errors.AddRange(Phone(phone, module));
            }

            return errors;
        }

        internal static IEnumerable<string> Phone(Phone_Type phone, string module)
        {
            List<string> errors = new List<string>();
            if (string.IsNullOrWhiteSpace(phone.AreaCode)) { errors.Add(module + " phone AreaCode"); }
            if (string.IsNullOrWhiteSpace(phone.DialNumber)) { errors.Add(module + " phone DialNumber"); }
            if (LUObjNotValid(phone.PhoneTypeCode)) { errors.Add(module + " phone PhoneTypeCode"); }
            return errors;
        }

        internal static IEnumerable<string> Reference(TXLife_Type response)
        {
            List<string> errors = new List<string>();
            var references = response.Items.OfType<Relation_Type>().Where(p => p.RelationRoleCode.tc == "290");
            foreach (var reference in references)
            {
                var refParty = response.Items.OfType<Party_Type>().Where(p => p.id == reference.RelatedObjectID).FirstOrDefault();
                if (refParty == null) { errors.Add("Contractor reference relatedParty"); }
                else
                {
                    if (string.IsNullOrWhiteSpace(refParty.id)) { errors.Add("Contractor reference party id"); }
                    if (LUObjNotValid(refParty.PartyTypeCode)) { errors.Add("Contractor reference party PartyTypeCode"); }
                    if (refParty.Item == null || !(refParty.Item is Person_Type)) { errors.Add("contractor reference party Item"); }

                    errors.AddRange(Phones(refParty.Phone, "Contractor reference party"));
                    errors.AddRange(Email(refParty.EMailAddress, "Contractor reference party"));
                }

                errors.AddRange(Relation(reference, "Contractor reference"));
            }

            return errors;
        }

        internal static IEnumerable<string> Relation(Relation_Type relation, string module)
        {
            List<string> errors = new List<string>();
            if (relation == null) { errors.Add(module + " relation"); }
            else
            {
                if (string.IsNullOrWhiteSpace(relation.id)) { errors.Add (module + " relation id"); }
                if (string.IsNullOrWhiteSpace(relation.OriginatingObjectID)) { errors.Add(module + " relation OriginatingObjectID"); }
                if (LUObjNotValid(relation.OriginatingObjectType)) { errors.Add(module + " relation OriginatingObjectType"); }
                if (string.IsNullOrWhiteSpace(relation.RelatedObjectID)) { errors.Add(module + " relation RelatedObjectID"); }
                if (LUObjNotValid(relation.RelatedObjectType)) { errors.Add(module + " relation RelatedObjectType"); }
                if (LUObjNotValid(relation.RelationRoleCode)) { errors.Add(module + " relation RelationRoleCode"); }
            }
            return errors;
        }

        internal static IEnumerable<string> Supervision(Party_Type contractor)
        {
            List<string> errors = new List<string>();
            foreach (var supervision in contractor.Producer.SupervisionLevel)
            {
                if (LUObjNotValid(supervision.SupervisionLevelTC)) { errors.Add("contractor supervision SupervisionLevelTC"); }
                if (string.IsNullOrWhiteSpace(supervision.IssuingPartyID)) { errors.Add("contractor supervision IssuingPartyID"); }
            }
            return errors;
        }

        internal static IEnumerable<string> Shareholder(TXLife_Type response, Party_Type party)
        {
            List<string> errors = new List<string>();

            if (LUObjNotValid(party.PartyTypeCode)) { errors.Add("Shareholder partyTypeCode"); }
            if (party.Item == null || !(party.Item is Person_Type)) { errors.Add("Shareholder item"); }
            if (string.IsNullOrWhiteSpace(party.FullName)) { errors.Add("Shareholder FullName"); }

            errors.AddRange(Email(party.EMailAddress, "Shareholder"));
            errors.AddRange(Relation(response.Items.OfType<Relation_Type>().Where(p => p.RelatedObjectID == party.id).FirstOrDefault(), "Shareholder"));

            return errors;
        }

        private static bool LUObjNotValid(dynamic lu)
        {
            return lu == null || string.IsNullOrWhiteSpace(lu.tc) || string.IsNullOrWhiteSpace(lu.Value);
        }

        internal static string DumpErrors(List<string> errors)
        {
            string output = string.Empty;
            foreach (var error in errors)
            {
                output += (error + "\r\n"); 
            }
            return output;
        }
    }
}
