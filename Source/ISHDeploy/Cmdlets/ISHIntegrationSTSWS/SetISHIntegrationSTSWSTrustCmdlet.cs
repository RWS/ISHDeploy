using System;
using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHIntegrationSTSWS;
using ISHDeploy.Validators;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTSWS
{
    /// <summary>
    ///		<para type="synopsis">Sets WSTrust configuration.</para>
    ///		<para type="description">The Set-ISHIntegrationSTSWSTrust cmdlet sets WSTrust configuration to Content Manager deployment.</para>
    ///		<para type="description">When -IncludeInternalClients is switched on then the -ActorUsername and -ActorPassword must be specified.</para>
    /// </summary>
    /// <example>
    ///		<code>PS C:\>Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment -Endpoint "https://test.global.sdl.corp/InfoShareSTS/issue/wstrust/mixed/username" -MexEndpoint "https://test.global.sdl.corp/InfoShareSTS/issue/wstrust/mex" -BindingType "WindowsMixed" -Verbose</code>
    ///     <para>This command configure WS to use specified Endpoint and MexEndpoint of STS server and sets type of authentication as WindowsMixed.
    ///         Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    /// <example>
    ///		<code>PS C:\>Set-ISHIntegrationSTSWSTrust -ISHDeployment $deployment -Endpoint "https://test.global.sdl.corp/InfoShareSTS/issue/wstrust/mixed/username" -MexEndpoint "https://test.global.sdl.corp/InfoShareSTS/issue/wstrust/mex" -BindingType "WindowsMixed" -IncludeInternalClients -ActorUsername "STSUser" -ActorPassword "somepassword" -Verbose</code>
    ///     <para>This command configure WS to use specified Endpoint and MexEndpoint of STS server, sets type of authentication as WindowsMixed and sets internal clients credentials.
    ///         Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSWSTrust")]
    public class SetISHIntegrationSTSWSTrustCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        [ValidateDeploymentVersion]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// <para type="description">Specifies the URL to issuer WSTrust endpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust endpoint")]
        public Uri Endpoint { get; set; }

        /// <summary>
        /// <para type="description">Specifies the URL to issuer WSTrust mexEndpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust mexEndpoint")]
        public Uri MexEndpoint { get; set; }

        /// <summary>
        /// <para type="description">Specifies the STS issuer authentication type.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Type of STS issuer authentication")]
        public BindingTypes BindingType { get; set; }

        /// <summary>
        /// <para type="description">Specifies that STS -ActorUsername and -ActorPassword need to be updated.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "When -IncludeInternalClients is switched on then -ActorUsername and -ActorPassword have meaning")]
        public SwitchParameter IncludeInternalClients { get; set; }

        /// <summary>
        /// <para type="description">Specifies the STS actor Username.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Username of STS actor")]
        public string ActorUsername { get; set; }

        /// <summary>
        /// <para type="description">Specifies the STS actor password.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Password of STS actor")]
        public string ActorPassword { get; set; }

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
            OperationPaths.Initialize(ISHDeployment);

            if (MyInvocation.BoundParameters.ContainsKey("IncludeInternalClients"))
            {
                var operation = new SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation(Logger, Endpoint, MexEndpoint, BindingType);

                if (MyInvocation.BoundParameters.ContainsKey("ActorUsername"))
                {
                    operation.SetActorUsername(Logger, ActorUsername);
                }

                if (MyInvocation.BoundParameters.ContainsKey("ActorPassword"))
                {
                    operation.SetActorPassword(Logger, ActorPassword);
                }

                operation.Run();
            }
            else
            {
                var operation = new SetISHIntegrationSTSWSTrustOperation(Logger, Endpoint, MexEndpoint, BindingType);
                operation.Run();
            }
        }
    }
}
