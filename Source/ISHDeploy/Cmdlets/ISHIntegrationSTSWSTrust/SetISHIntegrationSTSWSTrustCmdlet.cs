using System;
using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHIntegrationSTSWSTrust;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTSWSTrust
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
    [Cmdlet(VerbsCommon.Set, "ISHIntegrationSTSWSTrust")]
    [CmdletBinding(DefaultParameterSetName = MANDATORY_PARAMETER_SET)]
    public class SetISHIntegrationSTSWSTrustCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// The name of Mandatory parameter sets
        /// </summary>
        private const string MANDATORY_PARAMETER_SET = "Mandatory";

        /// <summary>
        /// The name of Internal parameter sets
        /// </summary>
        private const string INTERNAL_PARAMETER_SET = "Internal";

        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.", ParameterSetName = MANDATORY_PARAMETER_SET)]
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.", ParameterSetName = INTERNAL_PARAMETER_SET)]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// <para type="description">Specifies the URL to issuer WSTrust endpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust endpoint", ParameterSetName = MANDATORY_PARAMETER_SET)]
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust endpoint", ParameterSetName = INTERNAL_PARAMETER_SET)]
        public Uri Endpoint { get; set; }

        /// <summary>
        /// <para type="description">Specifies the URL to issuer WSTrust mexEndpoint.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust mexEndpoint", ParameterSetName = MANDATORY_PARAMETER_SET)]
        [Parameter(Mandatory = true, HelpMessage = "The URL to issuer WSTrust mexEndpoint", ParameterSetName = INTERNAL_PARAMETER_SET)]
        public Uri MexEndpoint { get; set; }

        /// <summary>
        /// <para type="description">Specifies the STS issuer authentication type.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Type of STS issuer authentication", ParameterSetName = MANDATORY_PARAMETER_SET)]
        [Parameter(Mandatory = true, HelpMessage = "Type of STS issuer authentication", ParameterSetName = INTERNAL_PARAMETER_SET)]
        public BindingTypes BindingType { get; set; }

        /// <summary>
        /// <para type="description">xxxxxx xxxx.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "xxx", ParameterSetName = INTERNAL_PARAMETER_SET)]
        public SwitchParameter IncludeInternalClients { get; set; }

        /// <summary>
        /// <para type="description">xxxxxx xxxx.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "xxx", ParameterSetName = INTERNAL_PARAMETER_SET)]
        public string ActorUsername { get; set; }

        /// <summary>
        /// <para type="description">xxxxxx xxxx.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "xxx", ParameterSetName = INTERNAL_PARAMETER_SET)]
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
            IOperation operation = null;

            switch (ParameterSetName)
            {
                case MANDATORY_PARAMETER_SET:
                    operation = new SetISHIntegrationSTSWSTrustOperation(Logger, IshPaths, Endpoint, MexEndpoint, BindingType);
                    break;
                case INTERNAL_PARAMETER_SET:
                    // TODO: Implement SetISHIntegrationSTSWSTrustIncludeInternalClientsOperation
                    break;
            }

            operation.Run();
        }
    }
}
