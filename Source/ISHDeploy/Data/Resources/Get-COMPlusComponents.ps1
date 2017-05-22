$VerbosePreference="Continue"
$comAdmin = New-Object -com ("COMAdmin.COMAdminCatalog.1")
$catalog = New-Object -com COMAdmin.COMAdminCatalog 
$applications = $catalog.getcollection("Applications") 
$applications.populate()
return $applications|Where-Object -Property Name -Like "Trisoft*"| ForEach-Object { $_.Name }
