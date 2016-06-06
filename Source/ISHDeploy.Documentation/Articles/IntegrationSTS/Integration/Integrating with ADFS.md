# Integrating with ADFS
 
**ADFS** is an implementation of a **Security Token Service**. The basic integration principals are explained in [Integrating with Security Token Service](Integrating with Security Token Service.md).
This article explains how to use the module's commandlets to integrate with **ADFS**. 

# Acknowledgements

The following values assume an ADFS at `adfs.example.com`
CopyCodeBlock(_nopublish\IntegrationValues.ADFS.ps1)

# Implementing the integration on Content Manager

There are two kind of integrations:
1. Only the clients not in the server zone use the **STS** at `adfs.example.com` to authenticate. All clients within the server zone continue using the internal **ISHSTS**.
2. All clients in the user and server zone use the **STS** at `adfs.example.com` to authenticate. Since the authentication type is **Windows**, we don't need to define the **Actor** crendentials.

## User zone integration

First get a reference to a deployment.
```powershell
$deployment=Get-ISHDeployment -Name InfoShare
```

Then
CopyCodeBlock(Implementation.User.ps1)

## All zone integration

First get a reference to a deployment.
```powershell
$deployment=Get-ISHDeployment -Name InfoShare
```

This is an example for a windows authentication
CopyCodeBlock(Implementation.All.Windows.ps1)

# Implement the integration on ADFS

The `Save-ISHIntegrationSTSConfigurationPackage` cmdlet supports a parameter `-ADFS` that generates an extra script that can configure the ADFS server remotely.
```powershell
$filename="$(Get-Date -Format "yyyyMMdd").ADFSIntegrationISH.zip"
Save-ISHIntegrationSTSConfigurationPackage -ISHDeployment $deployment -FileName $filename -ADFS
```

Inside the zip file you will find two files

- `CM Security Token Service Requirements.md` that is a markdown file with the specific deployment information.
- `Invoke-ADFSIntegrationISH.ps1` that can **set** or **remove** the necessary relying party entries on a target ADFS.
- `ishws.cer` that is the public key of the **ISHWS** service certificate. This is processes by the script.

If **ADFS01** is the computer name for the ADFS server then to set the relying parties execute this
```powershell
& .\Invoke-ADFSIntegrationISH.ps1 -Computer ADFS01 -Action Set
```

To remove the relying parties execute this
```powershell
& .\Invoke-ADFSIntegrationISH.ps1 -Computer ADFS01 -Action Remove
```

# Updating the encryption certificate on ADFS
When invoking `Invoke-ADFSIntegrationISH.ps1` with `-Action Set`, it will first search for existing relying parties. When found it will update them with the encryption certificate.

To keep the relying parties updated, then

1. Use `Save-ISHIntegrationSTSConfigurationPackage` to get the latest integration script
1. Execute `Invoke-ADFSIntegrationISH.ps1` with `-Action Set`