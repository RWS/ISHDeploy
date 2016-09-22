$name="Quality Assistant"
$realm="https://bl.example.com/"
# Add relying party
Set-ISHSTSRelyingParty -ISHDeployment $deploymentName -Name $name -Realm $realm -BL