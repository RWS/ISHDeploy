#Issuer name
$issuerName="Example ADFS (20160101)"
#WS Federation endpoint
$wsFederationUri="https://adfs.example.com/adfs/ls/"
#WS Trust endpoint
$wsTrustUri="https://sts.example.com/adfs/services/trust/13/windowsmixed"
#WS Trust metadata exchange endpoint
$wsTrustMexUri="https://sts.example.com/adfs/services/trust/mex"
#The authentication type
$bindingType="WindowsMixed"
#Token signing thumbprint
$tokenSigningCertificateThumbprint="2509fb22f7671aea2d0a28ae80516f390de0ca21"