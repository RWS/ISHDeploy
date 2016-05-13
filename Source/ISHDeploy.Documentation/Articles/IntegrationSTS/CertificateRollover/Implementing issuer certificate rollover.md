# Implementing issuer certificate rollover
 
This article explains how to execute a issuer certificate roll over when the token signing certificate changes on the **Security Token Service (STS)**.

# What is a certificate roll over.

Certificates have by design an expiration date. When certificates expire they become invalid and need to be replaced. The goal of certificate roll over is to achieve as less downtime as possible for this transition.

Typically, the owner of the certificate will get an updated or reniewed certificate. For the purpose of this article lets see what happens with

- Security Token Service signing certificate
- Service providers issuer validation 

The sequence is as follows:

1. STS imports the new certificate in the certificate store. 
2. STS shares the token signing certificate information with all service providers.
3. A service providers extends the validation with the new issuer certificate information.
4. All service providers become ready to accept the new certificate.
5. STS executes the certificate change.
6. All service providers remove the old issuer certificate information.
7. STS removes the old certificate.


# Acknowledgements

Lets assume that the integration was implemented on the 1st January 2016. A matching example script to configure the issuer certificate is this:
CopyCodeBlock(_nopublish\20160101.Set-ISHIntegrationSTSCertificate.ps1)
The issuer certificate expires in one year, on the **1st January 2017**.

On the 1st December 2016 the owner of the certificate receives a expiration reminder and creates a new certificate.

# Certificate rollover execution

## STS shared new issuer certificate information

STS has imported a new certificate on the store but hasn't made it primary. 

In the middle of **December 2016**, STS shares the information so that it can be added to the deployments.

The following script injects the new issuer certificate.
CopyCodeBlock(_nopublish\20161215.Set-ISHIntegrationSTSCertificate.ps1)
Notice that the Issuer name is changed from `Example STS (20160101)` to `Example STS (20161201)`.

This allows token signed with either certificates to be validated.

## STS switches token signing certificate

All deployments now validate token signing certificates with the value of issuer `Example STS (20161201)`.

## STS has removed old token signing certificate

On the **1st January 2017**, STS shares that the old certificate issued on the **1st January 2016** is completly gone. 

To remove old entries on deployments execute

CopyCodeBlock(_nopublish\20170101.Remove-ISHIntegrationSTSCertificate.ps1)
This completes the certificate rollover.