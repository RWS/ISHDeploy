using System.Management.Automation;
using ISHDeploy.Business.Operations.ISHIntegrationSTSWS;

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
    /// <example>
    ///		<code>PS C:\>Save-ISHIntegrationSTSConfigurationPackageCmdlet -ISHDeployment $deployment -ADFS -FileName packageFile.zip</code>
    ///     <para>Creates a zip archive in package folder with current STS integration configurations for ADFS.
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
        /// <para type="description">Specifies that ADFS configuration should be packed.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Used to export ADFS configuration.")]
        public SwitchParameter ADFS { get; set; }

        /// <summary>
        /// Executes cmdlet.
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var operation = new SaveISHIntegrationSTSConfigurationPackageOperation(Logger, ISHDeployment, FileName, ADFS.IsPresent);

            operation.Run();
        }
    }
}
