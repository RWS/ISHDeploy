# Configure deployment with ADFS
This article shows how to setup and provision an integration with ADFS (Active Directory Federated Services).

- Configure the integration for the first time
- 

##  Setup the first integration for the first time

### ADFS integration information
The owner of the ADFS has provided the following values

Name  | Value
------------- | -------------
WS Federation Endpoint  | https://mecdevstsnlb01.global.sdl.corp/adfs/ls/
WS Trust Endpoint  | https://mecdevstsnlb01.global.sdl.corp/adfs/services/trust/13/windowsmixed
WS Trust Endpoint  | https://mecdevstsnlb01.global.sdl.corp/adfs/services/trust/13/windowsmixed
Token Signing Certificate (Thumbprint)  | 43A61356B35A61196D1F9BAD391F897046B539FE
Token Signing Certificate (Validation Mode)  | None

This information is reflected into the following powershell fragment
CopyCodeBlock(_nopublish\ADFS Initial Values.ps1)
 
###  Setup the ish deployment for the first time
To set for the integration for the first we need to use the following commandlets driven by the integration parameters

- [Set-ISHIntegrationSTSWSFederation](/Using/Set-ISHIntegrationSTSWSFederation.md)
- [Set-ISHIntegrationSTSWSTrust](/Using/Set-ISHIntegrationSTSWSTrust.md)
- [Set-ISHIntegrationSTSCertificate](/Using/Set-ISHIntegrationSTSCertificate.md)

To help with the future certificate rollover, we will use an issuer name that reflects the ADFS's token signing certificate expiration date. e.g. ADFS20170215

### Configure ADFS
For the STS integration to be successful we need to configure the ADFS relying parties.
The [Get-ISHIntegrationSTSConfigurationPackage](/Using/Get-ISHIntegrationSTSConfigurationPackage.md) provides a package that contains
- The public key of the service certificate thumbprint
- Powershell scripts that can add or remove the necessary ADFS relying parties for this specific deployment


### Example script
CopyCodeBlockAndLink(Initial.ps1)

##  What to do when the token signing certificate expires

### ADFS integration information
The owner of the ADFS has provided the following values

Name  | Value
------------- | -------------
Token Signing Certificate (Thumbprint)  | 43A61356B35A61196D1F9BAD391F897046B539FE
Token Signing Certificate (Validation Mode)  | None

This information is reflected into the following powershell fragment
CopyCodeBlock(_nopublish\ADFS New Values.ps1)

### Update the deployment with the new token signing certificate
To update the deployment with the new token signing certificate we use [Set-ISHIntegrationSTSCertificate](/Using/Set-ISHIntegrationSTSCertificate.md). 

Once this operation is completed, the ISH deployment will accept tokens signed with the old and new certificate. This is good because it allows the system to operate withing a transitional period that the ADFS owner controls.

### Remove expired thumbprint
Once the owner of ADFS has informed us that we the certificate rollover has been completed, then we need to remove the old thumbprint from the deployment using [Remove-ISHIntegrationSTSCertificate](/Using/Remove-ISHIntegrationSTSCertificate.md)

### Example script
Add the new token signing certificate script
CopyCodeBlockAndLink(Add New Token Signing Certificate.ps1)

Remove the old token signing certificate script
CopyCodeBlockAndLink(Remove Old Token Signing Certificate.ps1)


 