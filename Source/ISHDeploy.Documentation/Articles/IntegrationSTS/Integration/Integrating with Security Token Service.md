# Integrating with Security Token Service (STS)
 
This article explains how to use the module's commandlets to integrate with a **Security Token Service** also referred to as **STS**.

# Acknowledgements

When a service provider integrated with an STS, it requires some information exchange. In the context of this article the service provider is Content Manager

Content Manager requires knowledge of the following information:

- WS Federation endpoint
- WS Trust endpoint
- WS Trust binding type
- WS Trust Metadata Exchange endpoint
- Token signing certificate thumbprint

The following values assume an STS at `sts.example.com`
CopyCodeBlock(_nopublish\IntegrationValues.ps1)
When the authentication type is Windows then the values change like this
CopyCodeBlock(_nopublish\IntegrationValues.Windows.ps1)

STS required knowledge of the following information:

- The endpoints to create relying parties for
  - ISHCM e.g. `https://ish.example.com/ISHCM`
  - ISHWS e.g. `https://ish.example.com/ISHWS`. Although it is not required it is recommended to create relying parties for each .svc endpoint in **ISHWS** e.g. `https://ish.example.com/ISHWS/Wcf/API25/Application.svc`
- For each **ISHWS** based identifier the public key to encrypt the tokens.
- Expected token claim composition. This drives the claims transformation rules.

# Implementing the integration on Content Manager

There are two kind of integrations:
1. Only the clients not in the server zone use the **STS** at `sts.example.com` to authenticate. All clients within the server zone continue using the internal **ISHSTS**.
2. All clients in the user and server zone use the **STS** at `sts.example.com` to authenticate.

The differences are shown in the following images
![](User.Zone.Clients.png)![](All.Zone.Clients.png)

## User zone integration

### Set deploymentName variable
First set deploymentName variable.
```powershell
$deploymentName="InfoShare"
```

Then
CopyCodeBlock(Implementation.User.ps1)

## All zone integration

### Set deploymentName variable
First set deploymentName variable.
```powershell
$deploymentName="InfoShare"
```

The internal clients are all non interactive. That means they required a pre-configured set of credentials to use during authentication.
When these clients are redirected to the **STS** at `sts.example.com`, then there are two options

- When the authentication type is windows then the `osuser` credential set will be used
- When the authentication type is username/password then a the `-ActorUsername` and `-ActorPassword` must be provided.

This is an example for a username/password authentication
CopyCodeBlock(Implementation.All.Username.ps1)

This is an example for a windows authentication
CopyCodeBlock(Implementation.All.Windows.ps1)

# Implement the integration on the Security Token Service

For every STS the configuration is different. Therefore the module's `Save-ISHIntegrationSTSConfigurationPackage` generates a set of information that describe the important information for configuring Content Manager's relying parties.
```powershell
$filename="$(Get-Date -Format "yyyyMMdd").IntegrationISH.zip"
Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deploymentName -FileName $filename
```

Inside the zip file you will find two files

- `CM Security Token Service Requirements.md` that is a markdown file with the specific deployment information.
- `ishws.cer` that is the public key of the **ISHWS** service certificate. This is added for your convienience.

# Enable authentication with Content Manager internal users.

While the deployment is integrated with an external security token service (STS), it could be requested that the system allows access for internal users without modifying the integration. 
An internal user is one that has username and password in the Content Manager database and the credential will be validated by ISHSTS.

```powershell
# Just Content Manager (ISH)
Enable-ISHIntegrationSTSInternalAuthentication -ISHDeployment $deploymentName
```

If the deployment is integrated with Content Delivery then provide the necessary values like this:

```powershell
# When the deployment is integrated with a Content Delivery (LC)
Enable-ISHIntegrationSTSInternalAuthentication -ISHDeployment $deployment -LCHost "lc.example.com" -LCWebAppName "ContentDelivery"
```

`Enable-ISHIntegrationSTSInternalAuthentication` will enable a special url at `https://ish.example.com/ISHWS/Internal/`. 
Provide this url to any user that wishes to login with internal users.

To disable this mode use `Disable-ISHIntegrationSTSInternalAuthentication`.

```powershell
Disable-ISHIntegrationSTSInternalAuthentication -ISHDeployment $deploymentName
```