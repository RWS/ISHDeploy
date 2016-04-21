### Identifiers and encryption certificates

For each service Content Manager expects the following combination of identifiers and encryption certificate to be configured on a Security Token Service.
<table>
	<tr>
		<th>Content Manager service name</th>
		<th>Identifiers</th>
		<th>Encryption certificate</th>
	</tr>
	<tr>
		<td>ISHCM</td>
		<td><pre>[token:CM_id]</pre></td>
		<td>None</td>
	</tr>
	<tr>
		<td>ISHWS</td>
		<td><pre>[token:WS_ids]</pre></td>
		<td>The public key of the certificate referenced through the [token:ServiceCertificateThumbprint] input parameter.</td>
	</tr>
</table>

### Claims in the token
Content Manager maps an incoming token to a user in the users repository by the external identifier.

The mapping is done through the token's attribute matching the claim type http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name.

For a token to be useful for Content Manager, the token's subject name should match a user in the Content Manager users repository.

### Token claims example
For a user that can be identified as user@company.com, the External ID in the users repository is expected to be user@company.com.

A valid incoming token must have at least the following attributes defined in it:

    <saml:AttributeStatement>
    	<saml:Attribute AttributeName="name" AttributeNamespace="http://schemas.xmlsoap.org/ws/2005/05/identity/claims">
    		<saml:AttributeValue>user@company.com</saml:AttributeValue>
    	</saml:Attribute>
    </saml:AttributeStatement>

    