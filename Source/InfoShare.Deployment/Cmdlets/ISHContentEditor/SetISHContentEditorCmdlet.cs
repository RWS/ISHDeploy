using System.Management.Automation;
using InfoShare.Deployment.Data.Actions.File;
using InfoShare.Deployment.Business;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
    /// <summary>
    /// <para type="synopsis">Sets new license for Content Editor.</para>
    /// <para type="description">The Set-ISHContentEditor cmdlet sets new license for Content Editor using domain name and license key parameters.</para>
    /// <para type="link">Test-ISHContentEditor</para>
    /// </summary>
    /// <example>
    /// <para>Set new license for Content Editor using domain name and license key:</para>
    /// <code>Set-ISHContentEditor -Domain "localhost" -LicenseKey "123445345342331313432423" -ISHDeployment $deployment -Force</code>
    /// <para>Parameter $deployment is an instance of the Content Manager deployment retrieved from Get-ISHDeployment cmdlet.</para>
    /// <para>Force parameter allows to skip prompt message and confirms to replace license for specific domain if it is already exists.</para>
    /// </example>
	[Cmdlet(VerbsCommon.Set, "ISHContentEditor")]
	public class SetISHContentEditorCmdlet : BaseHistoryEntryCmdlet
	{
        /// <summary>
        /// Hardcoded value of the license file type
        /// </summary>
		const string LicenseFileExtension = ".txt";
        
		/// <summary>
        /// <para type="description">Specifies the domain name for license.</para>
        /// </summary>
		[Parameter(Mandatory = true, HelpMessage = "Domain name for license")]
		[ValidateNotNullOrEmpty]
		public string Domain { get; set; }

        /// <summary>
        /// <para type="description">Specifies the license key value.</para>
        /// </summary>
		[Parameter(Mandatory = true, HelpMessage = "License key value")]
		[ValidateNotNullOrEmpty]
		public string LicenseKey { get; set; }
        
        /// <summary>
        /// <para type="description">Specifies the instance of the Content Manager deployment.</para>
        /// </summary>
        [Parameter(Mandatory = true, HelpMessage = "Instance of the installed Content Manager deployment.")]
		public Models.ISHDeployment ISHDeployment { get; set; }

        /// <summary>
        /// <para type="description">Allows the cmdlet to replace existing license if there is already registered one for that domain.</para>
        /// </summary>
        [Parameter(Mandatory = false)]
        public SwitchParameter Force { get; set; }

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
			var action = new FileCreateAction(Logger, IshPaths.LicenceFolderPath, string.Concat(Domain, LicenseFileExtension), LicenseKey);

            action.Execute();
		}
	}
}