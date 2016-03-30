using System.Management.Automation;
using ISHDeploy.Data.Managers.Interfaces;
using ISHDeploy.Business;

namespace ISHDeploy.Cmdlets.ISHPackage
{
    /// <summary>
    /// <para type="synopsis">Gets the path of a folder where the commandlets output files or packages or use them as input.</para>
    /// <para type="description">The Get-ISHPackageFolderPath cmdlet gets the path to packages folder used by module. This folder is specific per deployment.
    /// The folder contains archives or files that were created as output of other cmdlets of the module.
    /// The folder contains also archives or files that are used as input to other cmdlets of the module.</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHPackageFolderPath -ISHDeployment $deployment -UNC</code>
    /// <para>This command gets the UNC path to Packages folder for Content Manager deployment.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHPackageFolderPath")]
    public class GetISHPackageFolderPathCmdlet : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// <para type="description">Return path in UNC format.</para>
        /// </summary>
        [Parameter(Mandatory = false, HelpMessage = "Result path format")]
        public SwitchParameter UNC { get; set; }

        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();
            var ishPaths = new ISHPaths(ISHDeployment);

            // Create "Packages" folder if folder not exists
            fileManager.EnsureDirectoryExists(ishPaths.PackagesFolderPath);

            var result = UNC ? ishPaths.PackagesFolderUNCPath : ishPaths.PackagesFolderPath;

            WriteObject(result);
        }
    }
}
