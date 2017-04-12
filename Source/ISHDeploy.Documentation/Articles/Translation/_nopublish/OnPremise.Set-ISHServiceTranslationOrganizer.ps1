# Set the Translation Organizer configuration

#Acquire credential that authorize with target
$credential=Get-Credential

$hash=@{
	ISHWS="https://ish.example.com/ISHWS/"
	IssuerEndpoint="https://ish.example.com/ISHSTS/issue/wstrust/mixed/username"
	IssuerBindingType="UsernameMixed"
    Credential=$credential
}

Set-ISHServiceTranslationOrganizer -ISHDeployment $deploymentName @hash