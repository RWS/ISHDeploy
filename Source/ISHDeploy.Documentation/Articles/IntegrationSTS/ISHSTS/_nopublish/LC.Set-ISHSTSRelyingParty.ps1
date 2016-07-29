$name="Content Delivery"
$realm="review.lc.example.com"
# Add relying party
Set-ISHSTSRelyingParty -ISHDeployment $deploymentName -Name $name -Realm $realm -LC