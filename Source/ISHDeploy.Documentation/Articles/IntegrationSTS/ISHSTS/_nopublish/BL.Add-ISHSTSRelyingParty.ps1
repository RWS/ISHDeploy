$name="Quality Assistant"
$realm="bl.example.com"
# Add relying party
Add-ISHSTSRelyingParty -ISHDeployment $deployment -Name $name -Realm $realm -BL