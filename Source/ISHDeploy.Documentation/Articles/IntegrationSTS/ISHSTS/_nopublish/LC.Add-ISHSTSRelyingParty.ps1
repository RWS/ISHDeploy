$name="Content Delivery"
$realm="review.lc.example.com"
# Add relying party
Add-ISHSTSRelyingParty -ISHDeployment $deployment -Name $name -Realm $realm -LC