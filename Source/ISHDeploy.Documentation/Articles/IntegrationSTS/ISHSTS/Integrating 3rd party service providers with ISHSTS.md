# Integrating 3rd party service providers with ISHSTS
 
This article explains how to use the module's cmdlets to integrate 3rd party service providers with the **Security Token Service** provided in the Vanilla deployment also referred to as **ISHSTS**.

# Acknowledgements

Content Manager is installed on `ish.example.com` and ISHSTS is installed on `ish.example.com/ISHSTS`.

A 3rd party application is installed on `3rdparty.example.com` and need to integrate with the Content Manager's ISHSTS. 
In the context of this article, the 3rd party service provider is a web application.

When executing integrating a service provider with ISHSTS, each side requires certain information from the other.

The service provider needs the following from the ISHSTS:

- The WS Federation endpoint
- The token signing certificate. This is usually in the format of the thumbprint or the entire public key.

ISHSTS needs the following from the service provider:

- An identifier for the service provider. This is usually the url for the service provider. This value is used to create a relying party entry and has to be unique in the ISHSTS database.
- Claim composition in the token. 

A reference variable to a deployment is created with:
```powershell
$deployment=Get-ISHDeployment -Name InfoShare
```

# Configuring ISHSTS for a service provider

To add a relying part execute 
CopyCodeBlock(_nopublish\Add-ISHSTSRelyingParty.ps1)

By default ISHSTS will compose tokens with only the `NameIdentifier` of the `Subject` filled in. For example

```xml
<saml:Subject>
	<saml:NameIdentifier>user@example.com</saml:NameIdentifier>
	<saml:SubjectConfirmation>
		<saml:ConfirmationMethod>urn:oasis:names:tc:SAML:1.0:cm:bearer</saml:ConfirmationMethod>
	</saml:SubjectConfirmation>
</saml:Subject>
```

For the knowledge center **Content Delivery** and **Quality Assistant** service providers the expected claim transformation rules is already implemented.

For this purpose the cmdlet `Add-ISHSTSRelyingParty` provides a set of parameters specific to each one. The mapping is in the following table

| Service provider  | Parameter |
| ----------------- | --------- |
| Content Delivery  | `-LC` |
| Quality Assistant | `-BL` |

For example: 

- to add a relying party for a review deployment of **Content Delivery** execute:
CopyCodeBlock(_nopublish\LC.Add-ISHSTSRelyingParty.ps1)

- to add a relying party for a deployment of **Quality Assistant** execute:
CopyCodeBlock(_nopublish\BL.Add-ISHSTSRelyingParty.ps1)

# Configuring the service provider

If the service provider can be auto-configured through a WS Federation metadata exchange endpoint then ISHSTS provides this on`https://ish.example.com/ISHSTS/FederationMetadata/2007-06/FederationMetadata.xml`. 
In the alternative case retrieve the information manually:

- The WS Federation endpoint is `https://ish.example.com/ISHSTS/issue/wsfed` and is also provided by the system on the **Application Integration** page `https://ish.example.com/ISHSTS/appintegration`.
- The token signing certificate is availabe in the **Key Configuration** administration page `https://ish.example.com/ISHSTS/Admin/KeyConfiguration`. On this page the subject name and the thumbprint are visible but to retrieve the entire public key, you need to retrieve it from the certificate store of the operating system hosting Content Manager.

To get a list of which relying parties are availabe use the `Get-ISHSTSRelyingParty`. As with the `Add-ISHSTSRelyingParty` optional parameters for known service providers are supported

| Service provider  | Parameter |
| ----------------- | --------- |
| Content Manager   | `-ISH` |
| Content Delivery  | `-LC` |
| Quality Assistant | `-BL` |


Here are a couple of examples

CopyCodeBlock(_nopublish\Get-ISHSTSRelyingParty.ps1)