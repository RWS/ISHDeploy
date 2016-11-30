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
using ISHDeploy.Business.Operations.ISHPackage;

namespace ISHDeploy.Cmdlets.ISHPackage
{
    /// <summary>
    ///		<para type="synopsis">Sets resource group in ~\Author\ASP\UI\Extensions\_config.xml.</para>
    ///		<para type="description">The Set-ISHCMCUILResourceGroup cmdlet sets resource group in "~\Author\ASP\UI\Extensions\_config.xml".</para>
    ///     <para type="link">Expand-ISHCMPackage</para>
    ///     <para type="link">Copy-ISHCMFile</para>
    /// </summary>
    /// <seealso cref="BaseHistoryEntryCmdlet" />
    /// <example>
    ///		<code>PS C:\&gt;Set-ISHCMCUILResourceGroup -ISHDeployment $deployment -Path "Extensions\custom\custom.js"</code>
    ///		<para>This command updates "~\Author\ASP\UI\Extensions\_config.xml" CUIF config file with default Name "Trisoft.Extensions" and adds path to "Extensions\custom\custom.js" file in resource group with name "Trisoft.Extensions". In case resource group with such name does not exist this command will add new resource group.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    /// <example>
    ///		<code>PS C:\&gt;Set-ISHCMCUILResourceGroup -ISHDeployment $deployment -Name "Trisoft.Extensions" -Path @("Extensions\custom\custom1.js", "Extensions\custom\custom2.js")</code>
    ///		<para>This command updates "~\Author\ASP\UI\Extensions\_config.xml" CUIF config file and adds paths to bunch of files in resource group with name "Trisoft.Extensions". In case resource group with such name does not exist this command will add new resource group.
    /// Parameter $deployment is a deployment name or an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///		</para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHCMCUILResourceGroup")]
	public sealed class SetISHCMCUILResourceGroupCmdlet : BaseHistoryEntryCmdlet
	{
        /// <summary>
        /// <para type="description">Name of resource group in "~\Author\ASP\UI\Extensions\_config.xml".</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Name of resource group")]
        [ValidateNotNullOrEmpty]
        public string Name { get; set; } = "Trisoft.Extensions";

        /// <summary>
        /// <para type="description">Path or paths to resource files.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Path or paths to resource files")]
        [ValidateNotNullOrEmpty]
        public string[] Path { get; set; }


        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
		{
            var operation = new SetISHCMCUILResourceGroupOperation(Logger, ISHDeployment, Name, Path);
            operation.Run();
		}
	}
}