# Configuring credentials
 
This tutorial explains how to configure credentials for a deployment. The credentials are of the following kind:

| Target | Scope | Remarks |
| ------ | ----- | ------- |
| OSUser | Operating System | This is the operating system user that executes all deployment's processes |
| ServiceUser | Content Manager (ISH) | This is a Content Manager user that is used from the deployment for certain batch operations | 
| Actor | Security Token Service (STS) | This is a user that is used to drive authentication with identity delegation |

**Notice** that the module will assume the given credentials are valid for their respected scope and only configure the deployment to use them. 

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Setting the OSUser

`Set-ISHOSUser` sets the credentials for all deployment's processes. **Notice** that the credential's username could be of the following formats:

| Username | Local or Domain | Allowed |
| -------- | --------------- | ------- |
| `DOMAIN\username` | Domain | Allowed |
| `COMPUTERNAME\username` | Local | Allowed |
| `username` | Local | Not allowed |
| `.\username` | Local | Not allowed |

In this example we use the `localuser` that is already present in the operating system's registry.

CopyCodeBlock(_nopublish\Example.Set-ISHOSUser.Local.ps1)

In this example we use the `domainuser` already present in the `DOMAIN` active directory.

CopyCodeBlock(_nopublish\Example.Set-ISHOSUser.Domain.ps1)