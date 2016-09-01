# Known issues

### 1
The following Pester code is not allowed when doing a token signing certificate rollover for ISHSTS.

```powershell
Describe "" {
        Context "Add TOKEN1 to trusted issuer" {
            It "Set-ISHIntegrationSTSCertificate" {
                Set-ISHIntegrationSTSCertificate -ISHDeployment $DeploymentName -Thumbprint ($token1Certificate.Thumbprint) -Issuer "ISHSTS.$($token1Certificate.FriendlyName)"
            }
        }
        Context "Configure ISHSTS with TOKEN1" {
            It "Set-ISHSTSConfiguration" {
                Set-ISHSTSConfiguration -ISHDeployment $DeploymentName -TokenSigningCertificateThumbprint ($token1Certificate.Thumbprint)  
            }
        }
 
 
}
```
