# Starting and stopping

This tutorial explains how to start or stop a Content Manager deployment.

A deployment offer components and the module can manage and track them. When a deployment is stopped, all processes related to the deployment are also stopped. This means that the deployment's impact on the server is on the file system, the registry and gac. Enabling or disabling components, gives the module the knowledge of which processes to start or not based on the components configuration.

The following workflow is advised when configuring a deployment:

1. Stop the deployment.
1. Execute any cmdlet that configured or customized the deployment.
1. Start the deployment.

Unless, components were enabled or disabled during with the above flow, the deployment will effectively start only the processes that were running before the changes.

## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Stop a deployment

To stop a deployment use `Stop-ISHDeployment`. For example

CopyCodeBlock(_nopublish\Example.Stop-ISHDeployment.ps1)

Then the status of the deployment becomes `Stopped`. 

CopyCodeBlock(_nopublish\Example.Get-ISHDeployment.ps1)

```text
Name        Status
----        ------
InfoShare  Stopped
```

## Start a deployment

To start a deployment use `Start-ISHDeployment`. For example

CopyCodeBlock(_nopublish\Example.Start-ISHDeployment.ps1)

Then the status of the deployment becomes `Started`. 

CopyCodeBlock(_nopublish\Example.Get-ISHDeployment.ps1)

```text
Name        Status
----        ------
InfoShare  Started
```

## Restart a deployment

Sometimes it is necessary to restart all processes of a deployment. This flow would be a stop and then start but the module offers one cmdlet `Restart-ISHDeployment` for both steps.

 To restart a deployment execute 

CopyCodeBlock(_nopublish\Example.Restart-ISHDeployment.ps1)
