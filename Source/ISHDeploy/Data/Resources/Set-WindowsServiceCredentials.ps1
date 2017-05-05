$VerbosePreference="Continue"
$service = gwmi win32_service -computer $env:COMPUTERNAME -filter "name='$name'"
$service.change($null,$null,$null,$null,$null,$null,"$username","$password")