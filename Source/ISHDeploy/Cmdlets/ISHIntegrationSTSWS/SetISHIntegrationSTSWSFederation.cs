using System;
using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHIntegrationSTSWS;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTSWS
{
    /// <summary>
    ///		<para type="synopsis">Sets WSFederation configuration.</para>
    ///		<para type="description">The Set-ISHIntegrationSTSWSFederation cmdlet sets WSFederation configuration to Content Manager deployment.</para>
    /// </summary>
    /// <example>
    ///		<code>PS C:\>Set-ISHIntegrationSTSWSFederation -ISHDeployment $deployment -Endpoint "https://test.global.sdl.corp/InfoShareSTS/issue/wsfed"</code>
    ///     <para>This command configure WS Federation to use specified Endpoint of STS server.
    ///         Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSWSFederation")]
    public class SetISHIntegrationSTSWSFederationCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// <para type="description">Specifies the URL to issuer endpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSFederation endpoint")]
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Cashed value for <see cref="IshPaths"/> property
        /// </summary>
        private ISHPaths _ishPaths;

        /// <summary>
        /// Returns instance of the <see cref="ISHPaths"/>
        /// </summary>
        protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            IOperation operation = null;
            OperationPaths.Initialize(ISHDeployment);

            operation = new SetISHIntegrationSTSWSFederationOperation(Logger, Endpoint);

            operation.Run();
        }
    }
}
