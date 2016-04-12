#Invokes command in session or locally, depending on -session parameter
Function Invoke-CommandRemoteOrLocal {
    param (
        [Parameter(Mandatory=$true)]
        $ScriptBlock,
        [Parameter(Mandatory=$false)]
        $Session=$null,
        [Parameter(Mandatory=$false)]
        $ArgumentList=$null
    ) 

    if($Session)
    {
        Invoke-Command -Session $Session -ScriptBlock $ScriptBlock -ArgumentList $ArgumentList -ErrorAction Stop
    }
    else
    {
        Invoke-Command -ScriptBlock $ScriptBlock -ArgumentList $ArgumentList -ErrorAction Stop 
    }
}

#retries command specified amount of times with 1 second delay between tries. Exits if command has expected response or tried to run specifeied amount of time
function RetryCommand {
    param([int]$numberOfRetries, $command, $expectedResult)

    for($i=0; $i -lt $numberOfRetries; $i++) {
     $actualResult = Invoke-Command -ScriptBlock $command
     if($actualResult -eq $expectedResult) {
        $i = $numberOfRetries
     }
     else {
        Start-Sleep -Milliseconds 1000
     }
    }

    return $actualResult
}

#Logs debug info
Function Log($Message)
{
    $date=Get-Date -Format "HHmmss.fff"
    $threadID=[System.Threading.Thread]::CurrentThread.ManagedThreadId
    $computerName=$env:COMPUTERNAME

    Write-Debug "$computerName $date [$threadID] - $Message"
}


