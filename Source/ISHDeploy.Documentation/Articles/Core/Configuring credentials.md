# Configuring credentials
 
This tutorial explains how to configure credentials for a deployment. The credentials are of the following kind:

| Target | Scope | Remarks |
| ------ | ----- | ------- |
| OSUser | Operating System | This is the operating system user that executes all deployment's processes |
| ServiceUser | Content Manager (ISH) | This is a Content Manager user that is used from the deployment for certain batch operations | 
| Actor | Security Token Service (STS) | This is a user that is used to drive authentication with identity delegation |

**Notice** that the module will assume that the given credential are valid for their respected scope and only configure the deployment to use them. The module will not validate the credential validity.

**Tip** to update the password per target type, use the following cmdlet while keeping the username the same.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Setting the OSUser

`Set-ISHOSUser` sets the credential for all deployment's processes.

**Notice** that the credential's username should follow one of the following formats:

- `DOMAIN\domainusername` for domain users.
- `COMPUTERNAME\localusername` for local users.

In this example we use the `localuser` that is already present in the operating system's registry.

CopyCodeBlock(_nopublish\Example.Set-ISHOSUser.Local.ps1)

In this example we use the `domainuser` already present in the `DOMAIN` active directory.

CopyCodeBlock(_nopublish\Example.Set-ISHOSUser.Domain.ps1)

## Setting the ServiceUser

`Set-ISHServiceUser` sets the credential of the deployment's *ServiceUser*. The referenced user must be valid in Content Manager's user repository.

CopyCodeBlock(_nopublish\Example.Set-ISHServiceUser.ps1)

## Setting the Actor

`Set-ISHActor` sets the credential of the deployment's *Actor*. The referenced user must be valid on the configured Security Token Service (STS). In case the STS is ISHSTS, then the referenced user must be valid in Content Manager's user repository.

CopyCodeBlock(_nopublish\Example.Set-ISHActor.ps1)

**Notice** that in the Vanilla deployment the actor's credential are same as with the credential of the *ServiceUser* but it could be different if desired.