$name="3rd party"
$realm="3rdparty.example.com"
# Add relying party
Add-ISHSTSRelyingParty -ISHDeployment $deploymentName -Name $name -Realm $realm