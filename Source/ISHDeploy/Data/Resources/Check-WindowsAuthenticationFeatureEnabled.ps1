if((Get-WmiObject -class Win32_OperatingSystem).caption -match "Server") {
    Write-Verbose "Checking windows feature with ServerManager"
    return $(Get-WindowsFeature -Name "Web-Windows-Auth").Installed
}
elseif(Get-Module DISM -ListAvailable) {
    Write-Verbose "Checking windows feature with DISM"
    return $(Get-WindowsOptionalFeature -FeatureName "IIS-WindowsAuthentication" -Online).State -eq "Enabled"
}
else {
    Write-Warning "Cannot determine if windows authentication is installed."
    return $false
}