# Implementing certificate replacement
 
This article explains how to execute a certificate replacement when the referenced certificate expires.

# Acknowledgements

Content Manager is installed on `ish.example.com` and ISHWS is installed on `ish.example.com/ISHWS`. 
This deployment uses the embedded ISHSTS for authentication.

Let’s assume that the deployment was implemented on the 1st January 2016 and the service certificate thumbprint was `20160101.Thumbprint`.

The service certificate expires in one year, on the **1st January 2017**.

On the 1st December 2016 the owner of the certificate receives an expiration reminder and creates a new certificate.

When the service certificate expires there is no true rollover and a downtime period is expected. The reasons are:

- The manner that the service certificate is used to identify the ISHWS soap endpoints. 
- Once a change is made all established client session will become invalid.

# Certificate rollover execution

## New service certificate becomes available

On the 1st December 2016, a new certificate becomes available on the store with thumbprint `20161201.Thumbprint` but since its not being referenced it's not used.

## Day of scheduled replacement

On the 15th December 2016 the following sequence is executed:

1. All users sign out.
1. The current service certificate with thumbprint `20160101.Thumbprint` is replaced by the newer `20161201.Thumbprint`. The cmdlet for this step is `Set-ISHAPIWCFServiceCertificate`. 
1. Users sign in.

This script replaces the existing certificate with the new one.

CopyCodeBlock(_nopublish\20161215.Set-ISHAPIWCFServiceCertificate.ps1)

## Remove the old certificate

The certificate with thumbprint `20161201.Thumbprint` is removed from the store. Since it's not referenced this step has not impact.