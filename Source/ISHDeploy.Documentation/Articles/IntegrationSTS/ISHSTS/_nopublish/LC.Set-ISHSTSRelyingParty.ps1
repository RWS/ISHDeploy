$name="Content Delivery"
$realm="https://review.lc.example.com/"
# Add relying party
Set-ISHSTSRelyingParty -ISHDeployment $deploymentName -Name $name -Realm $realm -LC