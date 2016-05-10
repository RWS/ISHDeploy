# Identifiers and encryption certificates

For each service Content Manager expects the following combination of identifiers and encryption certificate to be configured on a Security Token Service.
## Configuration for `$ishcmwebappname`
The identifier for service provider `$ishcmwebappname` is
```
https://$ishhostname/$ishcmwebappname/
```

## Configuration for `$ishwswebappname`
The identifiers for service provider `$ishwswebappname` are
```
https://$ishhostname/$ishwswebappname/
https://$ishhostname/$ishwswebappname/Wcf/API25/Application.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/Baseline.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/DocumentObj.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/EDT.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/EventMonitor.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/Folder.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/ListOfValues.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/MetadataBinding.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/OutputFormat.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/PublicationOutput.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/Search.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/Settings.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/TranslationJob.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/TranslationTemplate.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/User.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/UserGroup.svc
https://$ishhostname/$ishwswebappname/Wcf/API25/UserRole.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/Application.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/DocumentObj.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/EDT.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/Folder.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/MetaDataAssist.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/OutputFormat.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/Publication.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/PublicationOutput.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/Reports.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/Search.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/Settings.svc
https://$ishhostname/$ishwswebappname/Wcf/API20/Workflow.svc
https://$ishhostname/$ishwswebappname/Wcf/API/Application.svc
https://$ishhostname/$ishwswebappname/Wcf/API/ConditionManagement.svc
```

The issued token for each identifier of `$ishwswebappname` must be encrypted with the [$ishwscertificate]($ishwscertificate) certificate. The content is
```
$ishwscontent
```

# Claims in the token
Content Manager maps an incoming token to a user in the users repository by the external identifier.

The mapping is done through the token's attribute matching the claim type http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name.

For a token to be useful for Content Manager, the token's subject name should match a user in the Content Manager users repository.

## Token claims example
For a user that can be identified as user@example.com, the External ID in the users repository is expected to be user@example.com.

A valid incoming token must have at least the following attributes defined in it:

```xml
<saml:AttributeStatement>
    <saml:Attribute AttributeName="name" AttributeNamespace="http://schemas.xmlsoap.org/ws/2005/05/identity/claims">
    	<saml:AttributeValue>user@example.com</saml:AttributeValue>
    </saml:Attribute>
</saml:AttributeStatement>
```
    