# Levy Transfer Matching Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-levy-transfer-matching-web?repoName=SkillsFundingAgency%2Fdas-levy-transfer-matching-web&branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2418&repoName=SkillsFundingAgency%2Fdas-levy-transfer-matching-web&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-levy-transfer-matching-web&metric=alert_status)](https://sonarcloud.io/dashboard?id=SkillsFundingAgency_das-levy-transfer-matching-web)
[![Jira Project](https://img.shields.io/badge/Jira-Project-blue)](https://skillsfundingagency.atlassian.net/secure/RapidBoard.jspa?rapidView=674&projectKey=TM)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/2706801162/Levy+Transfers+Matching)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

Levy transfer matching allows levy to be pledged by an employer and made available to other employers to use.

## How It Works

This web application operates as a sub-site of [Manage Apprenticeships](https://github.com/SkillsFundingAgency/das-employerapprenticeshipsservice). 
It consists of this web application, an Api and an Azure Function component, links for which can be found below.

* [Levy Transfer Matching Api](https://github.com/SkillsFundingAgency/das-levy-transfer-matching-api)
* [Levy Transfer Matching Functions](https://github.com/SkillsFundingAgency/das-levy-transfer-matching-functions)
* [Apim endpoints](https://github.com/SkillsFundingAgency/das-apim-endpoints)

## 🚀 Installation

### Pre-Requisites

The Azure Functions component is not necessary for the website to function but can be found [here](https://github.com/SkillsFundingAgency/das-levy-transfer-matching-functions)

* A clone of this repository
* A clone of [Levy Transfer Matching Api](https://github.com/SkillsFundingAgency/das-levy-transfer-matching-api)
* A clone of [Apim endpoints](https://github.com/SkillsFundingAgency/das-apim-endpoints)
* A code editor that supports .NetCore 3.1
* A CosmosDB instance or emulator
* The [EmployerAccount](https://github.com/SkillsFundingAgency/das-employerapprenticeshipsservice) Api running locally or available in a test environment
* A Redis instance
* The latest [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config) for:
  *  `SFA.DAS.LevyTransferMatching_Web_1.0`
  *  `SFA.DAS.EmployerUrlHelper_1.0`
  *  `SFA.DAS.SFA.DAS.Employer.Shared.U_1.0`
  *  `SFA.DAS.EncodingService_1.0`
  *  `SFA.DAS.EmployerAccountAPI_1.0`
* A valid employer user account

### Config


This utility uses the standard Apprenticeship Service configuration. All configuration can be found in the [das-employer-config repository](https://github.com/SkillsFundingAgency/das-employer-config) which may be more up-to-date than what is described here.

AppSettings.json file
```json
{
    {
      "ConnectionStrings": {
        "RedisLogging": ""
      },
      "Logging": {
        "LogLevel": {
          "Default": "Warning"
        }
      },
      "AllowedHosts": "*",
      "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true",
      "ConfigNames": "SFA.DAS.LevyTransferMatching.Web,SFA.DAS.EmployerUrlHelper:EmployerUrlHelper,SFA.DAS.Employer.Shared.UI,SFA.DAS.Encoding:EncodingService,SFA.DAS.EmployerAccountAPI:EmployerAccountApi",
      "Environment": "LOCAL",
      "Version": "1.0",
      "APPINSIGHTS_INSTRUMENTATIONKEY": "",
      "cdn": {
        "url": "https://das-at-frnt-end.azureedge.net"
      }
    }
  }  
```

Azure Table Storage config

Row Key: SFA.DAS.LevyTransferMatching.Web_1.0

Partition Key: LOCAL

Data:

```json
{
  "LevyTransferMatchingWeb": {
    "RedisConnectionString": "localhost",
    "DataProtectionKeysDatabase": "DefaultDatabase=3",
    "UtcNowOverride": null
  },
 "LevyTransferMatchingApi": {
    "ApiVersion": "1.0",
    "SubscriptionKey": "",
    "ApiBaseUrl": "https://localhost:5221/"
  },
  "Authentication": {
    "AccountActivationUrl": "/account/confirm",
    "AuthorizeEndpoint": "/connect/authorize",
    "BaseAddress": "",
    "ChangeEmailUrl": "/account/changeemail?clientId={0}&returnurl=",
    "ChangePasswordUrl": "/account/changepassword?clientId={0}&returnurl=",
    "ClientId": "",
    "ClientSecret": "",
    "LogoutEndpoint": "/connect/endsession?id_token_hint={0}",
    "Scopes": "openid profile",
    "TokenEndpoint": "/connect/token",
    "UsePkce": false,
    "UserInfoEndpoint": "/connect/userinfo"
  },
  "CosmosDb": {
    "Uri": "https://localhost:8081",
    "AuthKey": ""
   }
}
```

## 🔗 External Dependencies

* This utility uses the [EmployerAccount](https://github.com/SkillsFundingAgency/das-employerapprenticeshipsservice) Api

## Technologies

* .NetCore 3.1
* CosmosDB
* REDIS
* NLog
* Azure Table Storage
* NUnit
* Moq
* FluentAssertions

## 🐛 Known Issues

* This web application must be run under the Kestrel web server