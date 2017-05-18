$VerbosePreference="Continue"
$comAdmin = New-Object -com ("COMAdmin.COMAdminCatalog.1")
$catalog = New-Object -com COMAdmin.COMAdminCatalog 
$applications = $catalog.getcollection("Applications") 
$applications.populate()
$trisoftInfoShareAuthorApplication=$applications|Where-Object -Property Name -EQ "$name"
$trisoftInfoShareAuthorApplication.Value("IsEnabled") = $false
return $applications.SaveChanges();