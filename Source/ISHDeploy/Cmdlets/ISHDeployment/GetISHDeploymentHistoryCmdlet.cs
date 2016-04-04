﻿using System.Management.Automation;
using ISHDeploy.Business;
using ISHDeploy.Data.Managers.Interfaces;

namespace ISHDeploy.Cmdlets.ISHDeployment
{
    /// <summary>
    /// <para type="synopsis">Gets customization history for Content Manager deployment.</para>
    /// <para type="description">The Get-ISHDeploymentHistory cmdlet gets the customization history information for Content Manager deployment that was generated by other cmdlets.</para>
    /// <para type="link">Clear-ISHDeploymentHistory</para>
    /// <para type="link">Get-ISHDeployment</para>
    /// <para type="link">Undo-ISHDeployment</para>
    /// </summary>
    /// <example>
    /// <code>PS C:\>Get-ISHDeploymentHistory -ISHDeployment $deployment</code>
    /// <para>This command gets the history information for Content Manager deployment.
    /// Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// </example>
    [Cmdlet(VerbsCommon.Get, "ISHDeploymentHistory")]
    [OutputType(typeof(string))]
    public class GetISHDeploymentHistoryCmdlet : BaseCmdlet
    {
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
        public Models.ISHDeployment ISHDeployment { get; set; }
        
        /// <summary>
        /// Executes cmdlet
        /// </summary>
        public override void ExecuteCmdlet()
        {
            var fileManager = ObjectFactory.GetInstance<IFileManager>();

            var historyFilePath = new ISHPaths(ISHDeployment).HistoryFilePath;

            if (!fileManager.FileExists(historyFilePath))
            {
                return;
            }

            var historyContent = fileManager.ReadAllText(historyFilePath);

            WriteObject(historyContent);
        }
    }
}
