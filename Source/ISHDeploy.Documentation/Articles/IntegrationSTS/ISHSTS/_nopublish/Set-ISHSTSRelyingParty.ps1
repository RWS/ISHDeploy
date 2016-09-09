$name="3rd party"
$realm="https://3rdparty.example.com/"
# Add relying party
Set-ISHSTSRelyingParty -ISHDeployment $deploymentName -Name $name -Realm $realm