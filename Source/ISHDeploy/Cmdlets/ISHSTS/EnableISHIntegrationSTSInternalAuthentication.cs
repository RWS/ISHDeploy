/*
 * Copyright (c) 2014 All Rights Reserved by the SDL Group.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System.Management.Automation;
using System;
using ISHDeploy.Business.Operations.ISHSTS;

namespace ISHDeploy.Cmdlets.ISHSTS
{
    /// <summary>
    ///		<para type="synopsis">Create new page with additional login redirect.</para>
    ///		<para type="description">Create new index.html page with additional login redirect. Also create connectionconfiguration.xml file to use STS.</para>
    /// </summary>
    /// <seealso cref="ISHDeploy.Cmdlets.ISHSTS" />
    /// <example>
    ///		<code>PS C:\&gt;Enable-ISHSTSSupportAccess -ISHDeployment $deployment</code>
    ///		<para>This command enable internal STS authentication with new index.html page.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    /// <example>
    ///		<code>PS C:\&gt;Enable-ISHSTSSupportAccess -ISHDeployment $deployment -LCHost "lc.example.com" -LCWebAppName "ContentDelivery" </code>
    ///		<para>This command enable internal STS authentication with new index.html page.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    /// Parameter LCHost define host name
    /// Parameter LCWebAppName define web application name by default it is "ContentDelivery".
    ///		</para>
    /// </example>
    [Cmdlet(VerbsLifecycle.Enable, "ISHIntegrationSTSInternalAuthentication")]
    public sealed class EnableISHIntegrationSTSInternalAuthentication : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Certificate Thumbprint.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Host name")]
        public string LCHost { get; set; }

        /// <summary>
        /// <para type="description">Issuer name.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Web application name")]
        public string LCWebAppName { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            if (LCHost == null && LCWebAppName != null)
            {
                throw new Exception("Parameter '-LCWebAppName' could not be inserted without '-LCHost'");
            }
            if (LCHost != null && LCWebAppName == null)
            {
                LCWebAppName = "ContentDelivery"; // default value
            }

            var operation = new EnableISHAuthenticationOperation(Logger, ISHDeployment, LCHost, LCWebAppName);

            operation.Run();
        }
    }
}