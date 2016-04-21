using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using ISHDeploy.Business.Operations.ISHDeployment;
using ISHDeploy.Validators;

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Gets all installed Content Management deployments.</para>
    /// <para type="description">The Get-ISHDeployment cmdlet gets all installed Content Management deployments.</para>
    /// <para type="description">You can get specific instance of the installed Content Manager deployment by specifying the deployment name.</para>
    /// <para type="description">All Content Manager deployment instances' names start with 'InfoShare' prefix.</para>
    /// <para type="link">Clear-ISHDeploymentHistory</para>
    /// <para type="link">Get-ISHDeploymentHistory</para>
    /// <para type="link">Undo-ISHDeployment</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHDeployment</code>
    /// <para>This command retrieves a list of all installed Content Manager deployments.</para>
    /// </example>
    /// <example>
    /// <code>PS C:\>Get-ISHDeployment -Name 'InfoShare'</code>
    /// <para>This command retrieves specific instance of the Content Manager deployment by name 'InfoShare'.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHDeployment")]
    public class GetISHDeploymentCmdlet : BaseCmdlet
    {
        /// <summary>
        /// The information share prefix
        /// </summary>
        private const string InfoSharePrefix = "InfoShare";

        /// <summary>
        /// The validation pattern
        /// </summary>
        private const string ValidationPattern = "^(InfoShare)";

        /// <summary>
        /// <para type="description">Specifies the name of the installed Content Manager deployment.</para>
        /// <para type="description">All names start with InfoShare</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Name of the already installed Content Manager deployment.")]
        [ValidatePattern(ValidationPattern, Options = RegexOptions.IgnoreCase)]
        public string Name { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var suffix = Name?.Substring(InfoSharePrefix.Length);

            var operation = new GetISHDeploymentOperation(Logger, suffix);

            var result = operation.Run().ToArray();
            
            if (Name != null && result.Count() == 1)
            {
                WriteObject(result[0]);
            }
            else
            {
                WriteObject(result);
            }

            if (result.Any())
            {
                string warningMessage;

                if (!ValidateDeploymentVersion.CheckDeploymentVersion(result.First().SoftwareVersion, out warningMessage))
                {
                    WriteWarning(warningMessage);
                }
            }
        }
    }
}
