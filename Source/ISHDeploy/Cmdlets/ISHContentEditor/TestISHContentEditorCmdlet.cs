using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Business.Operations;
using ISHDeploy.Business.Operations.ISHContentEditor;
using ISHDeploy.Validators;

namespace ISHDeploy.Cmdlets.ISHContentEditor
{
    /// <summary>
    /// <para type="synopsis">Tests if Content Editor license exists for specific domain name.</para>
    /// <para type="description">Test-ISHContentEditor cmdlet tests if Content Editor license exists for specific domain name.</para>
    /// <para type="description">If license for 'com' domain was created then all domains that ends with '.com' will be valid.</para>
    /// <para type="description">In that case localhost.com domain will be valid, but localhost.com.net will be invalid.</para>
    /// <para type="link">Set-ISHContentEditor</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Test-ISHContentEditor -Domain "localhost" -ISHDeployment $deployment</code>
    /// <para>This command checks if license for domain name 'localhost' exists.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsDiagnostic.Test, "ISHContentEditor")]
	public class TestISHContentEditorCmdlet : BaseCmdlet
	{
        /// <summary>
        /// <para type="description">Specifies the domain name to be verified.</para>
        /// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Domain name to be verified")]
		[ValidateNotNullOrEmpty]
		public string Domain { get; set; }

        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        [ValidateDeploymentVersion]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
		public override void ExecuteCmdlet()
		{

            OperationPaths.Initialize(ISHDeployment);

            var operation = new TestISHContentEditorOperation(Logger, OperationPaths.FoldersPaths.LicenceFolderPath, Domain);

            var result = operation.Run();

            WriteObject(result);
		}
	}
}