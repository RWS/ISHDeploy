# Configuring and maintaining the search capability
 
This tutorial explains how to work with cmdlets that configure the search capability of a deployment. 

The search capability is powered by the **Crawler** and **FullTextIndex** components. The FullTextIndex is responsible for maintaining a slave database that is optimized for searching and Crawler will provide FullTextIndex with the data.

When scaling Content Manager, the architecture requires that within the stack, there is only one deployment with the FullTextIndex component enabled and all other deployments point to this unique instance.
 
## Set deploymentName variable
First set deploymentName variable.

```powershell
$deploymentName="InfoShare"
```

## Configuring the FullTextIndex component

`Set-ISHServiceFullTextIndex` cmdlet will configure the ports required to operate by the underlying process. Changing these port values is not typical and it is required only when the deployment's default ports are already reserved.

CopyCodeBlock(_nopublish\Search\Example.Set-ISHServiceFullTextIndex.ps1)

The script will set the querying and stop ports for the process to `8080` and `8079` respectively. It will also make sure that a proper firewall rule is configured in the operating system.

## Configuring the Crawler component

All Crawler components in a stack must refer to the same index. The identity of the index is derived during installation and is unique to every server. When scaling the stack to multiple servers, the identity needs to be configured to a single value using the `Set-ISHServiceCrawler` cmdlet.

CopyCodeBlock(_nopublish\Search\Example.Set-ISHServiceCrawler.ps1)

## Configuring the deployment

`Set-ISHIntegrationFullTextIndex` cmdlet will configure on the deployment the url of the location of the FullTextIndex component.

CopyCodeBlock(_nopublish\Search\Example.Set-ISHIntegrationFullTextIndex.ps1)

When setting this value, Crawler will not depend on the default local FullTextIndex component to upload the crawled data. For this reason, the invocation of `Set-ISHIntegrationFullTextIndex` removes from the Crawler component the dependency to the FullTextIndex one. `Undo-ISHDeployment` will revert those changes.

## Maintenance of Crawler component

`Invoke-ISHMaintenance` cmdlet provides the following maintenance actions for the Crawler component.

- **Registration**. From the Content Manager architecture, every Crawler's configuration must be first registered in the database.
    - **UnregisterAll**. Use this action to remove all Crawler registrations from the stack. Typically before invoking `Set-ISHServiceCrawler` or in context of debugging.
    - **Register**. Use this action once in the stack, when a new common Crawler identity has been introduced with `Set-ISHServiceCrawler`.
- **ReIndex**. Use this action when the FullTextIndex component's data store is lost or it must be regenerated.

CopyCodeBlock(_nopublish\Search\Example.Invoke-ISHMaintenance.Crawler.ps1)

## Maintenance of Crawler component

`Invoke-ISHMaintenance` cmdlet provides the following maintenance actions for the FullTextIndex component.

- **CleanUp**. Use this action to remove all data of the FullTextIndex component from the filesystem. Invoking the re-index action from the Crawler component's maintenance is expected.

CopyCodeBlock(_nopublish\Search\Example.Invoke-ISHMaintenance.FullTextIndex.ps1)