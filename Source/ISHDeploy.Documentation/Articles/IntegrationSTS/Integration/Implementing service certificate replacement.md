# Implementing service certificate replacement
 
This article explains how to execute a service certificate replacement for Content Manager's web service application **ISHWS**.

# Acknowledgements

Content Manager is installed on `ish.example.com` and ISHWS is installed on `ish.example.com/ISHWS`. 
This deployment is integrated with a 3rd party security token service installed on `sts.example.com`.

Let’s assume that the deployment was implemented on the 1st January 2016 and the service certificate thumbprint was `20160101.Thumbprint`.

The service certificate expires in one year, on the **1st January 2017**.

On the 1st December 2016 the owner of the certificate receives an expiration reminder and creates a new certificate.

When the service certificate expires there is no true rollover and a downtime period is expected. The reasons are:

- The manner that the service certificate is used to identify the ISHWS soap endpoints. 
- On the security token service, the encryption certificate on the ISHWS relying parties must be also replaced.
- Once a change is made all established client session will become invalid.

# Certificate rollover execution

## New service certificate becomes available

On the 1st December 2016, a new certificate becomes available on the store with thumbprint `20161201.Thumbprint` but since it’s not being referenced it's not used.

## Day of scheduled replacement

On the 15th December 2016 the following sequence is executed:

1. All users sign out.
1. The current service certificate with thumbprint `20160101.Thumbprint` is replaced by the newer `20161201.Thumbprint`. The cmdlet for this step is `Set-ISHAPIWCFServiceCertificate`. 
1. Users sign in.

This script replaces the existing certificate with the new one.

CopyCodeBlock(_nopublish\20161215.Set-ISHAPIWCFServiceCertificate.ps1)

The integration with any security token service is discussed in [Integrating with Security Token Service](Integrating with Security Token Service.md). 
Use `Save-ISHIntegrationSTSConfigurationPackage` to extract all necessary information from the deployment.

The integration with ADFS is discussed in [Integrating with ADFS](Integrating with ADFS.md). 
Use `Save-ISHIntegrationSTSConfigurationPackage` to extract all necessary information and a PowerShell script from the deployment. 
The script will update all relying party entries when executed against the integrated ADFS.

## Remove the old certificate

The certificate with thumbprint `20161201.Thumbprint` is removed from the store. Since it's not referenced this step has not impact.