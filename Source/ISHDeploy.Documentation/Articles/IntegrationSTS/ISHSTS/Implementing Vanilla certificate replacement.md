# Implementing Vanilla certificate replacement
 
This article explains how to execute a certificate replacement when the referenced certificate expires with a Vanilla deployment.

# Acknowledgements

Content Manager is installed on `ish.example.com` and ISHWS is installed on `ish.example.com/ISHWS`. 
This deployment uses the embedded ISHSTS for authentication. 
With a Vanilla deployment the referenced certificate serves multiple purposes:

- Service certificate in ISHWS
- Issuer certificate in ISHSTS
- The SSL certificate configured on the web site's HTTPS binding. This is not a prerequisite but it is a common used case.

Let’s assume that the deployment was implemented on the 1st January 2016 and the used certificate thumbprint was `20160101.Thumbprint`. 

The certificate expires in one year, on the **1st January 2017**.

On the 1st December 2016 the owner of the certificate receives an expiration reminder and creates a new certificate.

When the certificate expires there is no true rollover and a downtime period is expected. The reasons are:

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

# Replace the web site's HTTPS binding SSL certificate

IIS is a web host engine and can support multiple web sites. 
Content Manager creates application pools under a web site and lets IIS power the secure encryption on the network layer through an HTTPS binding. 
Content Manager is not responsible for the secure encryption and therefore the module doesn't offer any specific cmdlet to manage the HTTPS certificate.
A Content Manager vanilla deployment depends on one certificate which usually is the one that powers HTTPS on IIS.
For this reason, replacement of the certificate referenced by Content Manager will overlap with the replacement of the one in IIS.
This section explain how to extend the above process for IIS.

When installing IIS, the **WebAdministration** module is also installed. This module offers cmdlets and providers that help manage IIS.

There are many ways to create and configure a web site and its bindings in IIS. For this reason there is not one simple script that can update all possible bindings.
The simplest case is the one offered with a default IIS installation and is summarized as:

- There is only the default Web Site configured on IIS.
- The site's binding has no host header or IP addresses specified.

With this configuration the following script updates the certificate on the HTTPS binding to the new one with thumbprint `20161201.Thumbprint`.

```powershell
$thumbprint="20161201.Thumbprint"
Get-ChildItem "Cert:\LocalMachine\My\$thumbprint" | Set-Item -Path "IIS:\SslBindings\0.0.0.0!443"
```

If the web site was configured with a host header then the script becomes

```powershell
$thumbprint="20161201.Thumbprint"
$hostHeader="example.com"
Get-ChildItem "Cert:\LocalMachine\My\$thumbprint" | Set-Item -Path "IIS:\SslBindings\0.0.0.0!443!$hostHeader" -SSLFlags 1
```