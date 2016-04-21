# Explaining **ISHDeploy** Output Messages
This article explains the output messages layers used in **ISHDeploy** module.
***

## Verbose Message 
Verbose messages contain general user information.

Helps monitor the actions of commands at a finer level than the default. You can use the Write-Verbose cmdlet to produce this type of output in a script or the WriteVerbose() method to produce this type of output in a cmdlet. PowerShell displays this output in yellow, unless you customize it through the $host.PrivateData.Verbose* color configuration variables.

**ISHDeploy** Implements Verbose messages in ILogger interface.
```csharp
public interface ILogger
{
    /// <summary>
    /// Writes verbose message.
    /// </summary>
    /// <param name="message">Verbose message.</param>
    void WriteVerbose(string message);
}
```

To enable verbose mode for a cmdlet that checks for the -Verbose parameter:
```powershell
Copy-Item c:\temp\*.txt c:\temp\backup\ -Verbose
```


## Debug Message 
Debug messages contain troubleshooting information.

Helps diagnose problems that may arise and can provide a view into the inner workings of a command. You can use the Write-Debug cmdlet to produce this type of output in a script or the WriteDebug() method to produce this type of output in a cmdlet. PowerShell displays this output in yellow, unless you customize it through the $host.PrivateData.Debug* color configuration variables.

**ISHDeploy** Implements Debug messages in ILogger interface.
```csharp
public interface ILogger
{
    /// <summary>
    /// Writes debug-useful information.
    /// </summary>
    /// <param name="message">Debug message.</param>        
    void WriteDebug(string message);
}
```


To enable debug output for cmdlets use native powershell mechanism
```powershell
$debugPreference = "Continue"
Start-DebugCommand
```


## Progress Message 
Progress report messages contain information about how much work the cmdlet has completed when performing an operation that takes a long time.

Helps you monitor the status of long-running commands. You can use the Write-Progress cmdlet to produce this type of output in a script or the WriteProgress() method to produce this type of output in a cmdlet. PowerShell displays this output in yellow, unless you customize it through the $host.PrivateData.Progress* color configuration variables.

**ISHDeploy** Implements Progress messages in ILogger interface.
```csharp
public interface ILogger
{
    /// <summary>
    /// Reports progress.
    /// </summary>
    /// <param name="activity">Activity that takes place.</param>
    /// <param name="statusDescription">Activity description.</param>
    /// <param name="percentComplete">Complete progress in percent equivalent.</param>        
    void WriteProgress(string activity, string statusDescription, int percentComplete = -1);
}
```


To disable progress output from a script or cmdlet that generates it:
```powershell
$progressPreference = "SilentlyContinue"
Get-Progress.ps1
```


## Warning Message 
Warning messages contain a notification that the cmdlet is about to perform an operation that can have unexpected results.

**ISHDeploy** Implements Warnings messages in ILogger interface.
```csharp
public interface ILogger
{
    /// <summary>
    /// Writes warning message.
    /// </summary>
    /// <param name="message">Warning message.</param>        
    void WriteWarning(string message);
}
```



## Error Message 
Error message reports a nonterminating error to the error pipeline when the cmdlet cannot process a record but can continue to process other records. 

**ISHDeploy** Implements Error reporting in ILogger interface.
```csharp
public interface ILogger
{
    /// <summary>
    /// Writes non-terminating error.
    /// </summary>
    /// <param name="ex">Exception as a result of the error.</param>
    /// <param name="errorObject">Object that caused error.</param>        
    void WriteError(Exception ex, object errorObject = null);
}
```
