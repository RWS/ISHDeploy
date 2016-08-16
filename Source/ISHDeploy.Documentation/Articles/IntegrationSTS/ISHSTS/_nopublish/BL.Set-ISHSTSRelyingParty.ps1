$name="Quality Assistant"
$realm="bl.example.com"
# Add relying party
Set-ISHSTSRelyingParty -ISHDeployment $deploymentName -Name $name -Realm $realm -BL