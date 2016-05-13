# Set WS Federation integration
Set-ISHIntegrationSTSWSFederation -ISHDeployment $deployment -Endpoint $wsFederationUri
# Set WS Trust integration
Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment -Endpoint $wsTrustUri -MexEndpoint $wsTrustMexUri -BindingType $bindingType -IncludeInternalClients
# Set Token signing certificate
Set-ISHIntegrationSTSCertificate -ISHDeployment $deployment -Issuer $issuerName -Thumbprint $tokenSigningCertificateThumbprint