# Set WS Federation integration
Set-ISHIntegrationSTSWSFederation -ISHDeployment $deploymentName -Endpoint $wsFederationUri
# Set WS Trust integration
Set-ISHIntegrationSTSWSTrust -ISHDeployment $deploymentName -Endpoint $wsTrustUri -MexEndpoint $wsTrustMexUri -BindingType $bindingType -IncludeInternalClients
# Set Token signing certificate
Set-ISHIntegrationSTSCertificate -ISHDeployment $deploymentName -Issuer $issuerName -Thumbprint $tokenSigningCertificateThumbprint