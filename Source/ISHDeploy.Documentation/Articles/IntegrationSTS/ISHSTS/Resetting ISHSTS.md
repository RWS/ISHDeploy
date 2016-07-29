# Resetting ISHSTS
 
This article explains how to reset the **ISHSTS** configuration.

ISHSTS stores all configuration in a SQL compact database. When Content Manager is installed the database is created and automatically seeded. 

To help reset ISHSTS the module offers the `Reset-ISHSTS` cmdlet.

```powershell
$deploymentName="InfoShare"
Reset-ISHSTS -ISHDeployment $deploymentName
```

`Reset-ISHSTS` is not the same as `Undo-ISHDeployment`. As a small comparison 

- `Reset-ISHSTS` deletes only the SQL compact database. When the database is rebuild, all changes applies from other cmdlets with the exception of `Set-ISHSTSRelyingParty` are respected. 
`Set-ISHSTSRelyingParty` configures ISHSTS for 3rd party integrations and that information will be lost with the reset. To configure the 3rd party systems just execute the commands with `Set-ISHSTSRelyingParty` as descussed in [Integrating 3rd party service providers with ISHSTS](Integrating 3rd party service providers with ISHSTS.md).
- `Undo-ISHDeployment` deletes the SQL compact database and reverts all changes applies from other cmdlets. When the database is rebuild, then all original Vanilla state configuration is used.

`Reset-ISHSTS` offers confidence when working adding relying parties to ISHSTS. With `Reset-ISHSTS` you always remove all extra entries and know that the system will reset itself to the current functioning configuration.