# CITS Web Service

This is a Visual Studio 2015 project that tests the basic interactions with APEXA's CITS Web Service.  This project has been published to help you jumpstart your own integration efforts.  Here you'll find:

- XSD of the current specification
- WSDL for the API
- Excel sheet to define some fields (helps augment the current CLIEDIS documentation)
- Example code to connect to the endpoint
- Example code for each type of request
- Example XML requests
- Example XML responses

## Background
APEXA implements the CITS API specification published by CLIEDIS (http://www.cliediscair.ca/ViewTag.aspx?tag=TXLife&spec=2.34.00&view=36). We have worked with CLIEDIS in order to enhance the underlying ACORD specification and this project includes the version of the ACORD specification that includes all of the changes APEXA and CLIEDIS requested.  With the exception of changes related to the additional user stories that are currently pending sign off and minor fixes (related to bugs and the CLIEDIS certification process), this version now represents the API that will be available when the next version of APEXA is deployed.

## Timeline
- May 2016
  - API updated to reflect latest version of ACORD
- August 2016
  - CLIEDIS Certification approved
- April 2017
  - Endpoint Available
- July 2017
  - Go-Live

## Terminology and Values
Most terminology, values and enumerations are defined in the ACORD and CLIEDIS documentation.  Where possible, all of the values available in the APEXA system were taken directly from the ACORD standard (for example, all of the address types in APEXA map 1:1 with the ACORD types).  Any terminology or values that are unique to APEXA are defined in the package of user stories that have been published to each organization (e.g. APEXA ID, Application ID, agreement, contract, downgrade, etc.).

## CITS API
- A single method that follows the CITS XML request/response pattern.
- Two types of requests are supported:
  1. Request a list of advisors with changes between two dates and times.
    - Responds with a list of APEXA IDs.
  2. Request all of the data associated with one or more APEXA IDs.
    - Request could be a single APEXA ID or it could just echo back the list of APEXA IDs from the first call.
    - Request can be filtered by section to reduce volume of data.
    - Responds with the full advisor record and all associated contracts.
      - Links embedded in the XML can be used to retrieve any associated documents.
- API will not be throttled at Go-Live, but may be in the future depending on the volume of traffic.
  - May be throttled by number of advisor records per hour.
  - May be throttled by time of day.
  - All organizations will be notified before any throttling mechanism is implemented.

## Extended API
- The CITS specification is limited and does not provide all data required in all scenarios.  As a result, we offer an extended non-CITS API to access this data.  Functionality includes:
  - Request all of the Internal IDs linked to a single APEXA ID
  - Request all of the APEXA IDs linked to a single Internal ID
  - Request all of the APEXA IDs linked to a single Selling Code
  - Request the content of all files given a list of URLs

## Inbound API
- Add, update, and delete a Selling Code based on CarrierAppointmentId (AKA Application ID in the APEXA UI)


## Changelog

#### 2016-11-10
### Changed
- The web service binding has been changed from WSHttp to BasicHttp, to simplify connection from non-Microsoft implementations.

#### 2016-07-22
### Added
- The @id of each element is now a unique identifier within the entire TXLife object (including multiple TXLifeResponse objects within a TXLife object), and will only persist within the TXLife object that the @id is returned with.
- Advisor employment history records are now included in the feed.

### Changed
- The permanent APEXA ID for each advisor and organization is now included in the Party/PartyKey element. Previously this APEXA ID was included in the @id element.
- The permanent Contract ID that is associated with contracts in APEXA is now included in the CarrierAppointment/CarrierAppointmentKey element.
- Permanent Coverage ID and Debt ID values are now included in the Holding/HoldingKey element.

#### 2022-06-15
### Changed
- The Documentation/Samples now include the full SOAP tags and attributes for requests and responses.
- ChangedProducerListing should only be called with a maximum of 1 day interval.
- Replaced all references of 'bluesun' with 'apexa'.
- Updated and added test cases.
- Re-organized project folder structure.
- Added SoapUI project xml for import; Added documentation on how to add UAT/Prod credentials to SoapUI