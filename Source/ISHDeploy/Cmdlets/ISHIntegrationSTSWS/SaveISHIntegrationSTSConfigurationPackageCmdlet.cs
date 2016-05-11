using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHIntegrationSTSWS;
using ISHDeploy.Validators;

namespace ISHDeploy.Cmdlets.ISHIntegrationSTSWS
{
    /// <summary>
    ///		<para type="synopsis">Saves STS integration configuration to zip archive in package folder.</para>
    ///		<para type="description">The Save-ISHIntegrationSTSConfigurationPackageCmdlet saves STS integration configuration to zip archive in package folder.</para>
    ///     <para type="description">Cmdlet will overwrite zip archive if there is already one with same name.</para>
    ///     <para type="link">Set-ISHIntegrationSTSWSTrustCmdlet</para>
    /// </summary>
    /// <example>
    ///		<code>PS C:\>Save-ISHIntegrationSTSConfigurationPackageCmdlet -ISHDeployment $deployment -FileName packageFile.zip</code>
    ///     <para>Creates a zip archive in package folder with current STS integration configurations.
    ///         Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.
    ///     </para>
    /// </example>
    [Cmdlet(VerbsData.Save, "ISHIntegrationSTSConfigurationPackage")]
    public class SaveISHIntegrationSTSConfigurationPackageCmdlet : BaseHistoryEntryCmdlet
    {
        /// <summary>
        /// <para type="description">Name of the output package file.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Name of the output package file.")]
        public string FileName { get; set; }

        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        [ValidateDeploymentVersion]
        public Models.ISHDeployment ISHDeployment { get; set; }
        
        /// <summary>
        /// Cashed value for <see cref="IshPaths"/> property.
        /// </summary>
        private ISHPaths _ishPaths;

        /// <summary>
        /// Returns instance of the <see cref="ISHPaths"/>.
        /// </summary>
        protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment));

        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            OperationPaths.Initialize(ISHDeployment);

            var operation = new SaveISHIntegrationSTSConfigurationPackageOperation(Logger, ISHDeployment, FileName);

            operation.Run();
        }
    }
}
