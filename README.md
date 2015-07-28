# CITS Web Service

This is a Visual Studio 2015 project that tests the basic interactions with APEXA's CITS Web Service.  This project has been published to help you jumpstart your own integration efforts.  Here you'll find:

- XSD of current DRAFT specification.
- Excel sheet to define some fields (helps augment the current CLIEDIS documentation).
- Example code to connect to the endpoint.
- Example code for each type of request.
- Example XML requests.
- Example XML responses.

## Background
APEXA is implementing the [Advisor Screening] (http://www.cliediscair.ca/ViewTag.aspx?tag=TXLife&spec=2.34.00&view=36) specification published by CLIEDIS.  This specification is currently missing a few key items that APEXA needs.  We have worked with CLIEDIS in order to enhance the specification and this project includes a DRAFT version of the specification that represents APEXA and CLIEDIS's proposal to ACORD.  The next official version of the CITS specification should be available in November 2015 and should closely track the early DRAFT that is included in this project.

We expect minor enhancements to be made along the way (for example, the current DRAFT specification is missing supervision details due to several pending items currently sitting with the compliance committee), but this should represent 95% of what will be available in November when the final specification is released.

## Timeline
- July 2015
  - DRAFT specification available
- August (mid) 2015
  - DEV endpoint available with fake data
- September 2015
  - Changes sent to ACORD for approval
- November 2015
  - CLIEDIS publishes final specification
  - UAT endpoint available with real data
- January 2016
  - Go-Live

## API Details
- A single method that follows the standard CITS XML request/response pattern.
- Two types of requests supported:
  1. Request a list of advisors with updates between two dates.
    - We’re working with CLIEDIS to add time as a parameter to allow better intraday polling.
    - Responds with a list of APEXA Advisor IDs.
  2. Request all of the data associated with a list of Advisor IDs
    - Request could be a single ID, or could just echo back the list of IDs from the first call.
    - Request can be filtered by section to reduce volume of data.
    - Responds with the full Advisor record and all associated contracts.
      - Links embedded in the XML can be used to retrieve associated documents.
- API will not be throttled at the start, but may be in the future depending on volumes.
  - May be throttled by # of Advisor records per hour.
  - Throttling may depend on time of day.
