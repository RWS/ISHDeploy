$name="Quality Assistant"
$realm="bl.example.com"
# Add relying party
Add-ISHSTSRelyingParty -ISHDeployment $deploymentName -Name $name -Realm $realm -BL