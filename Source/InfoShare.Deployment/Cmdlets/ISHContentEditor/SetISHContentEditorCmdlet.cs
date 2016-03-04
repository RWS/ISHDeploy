using System;
using System.Management.Automation;
using InfoShare.Deployment.Data.Actions.File;
using InfoShare.Deployment.Providers;
using InfoShare.Deployment.Business;
using InfoShare.Deployment.Data.Actions;

namespace InfoShare.Deployment.Cmdlets.ISHContentEditor
{
	[Cmdlet(VerbsCommon.Set, "ISHContentEditor", SupportsShouldProcess = false)]
	public sealed class SetISHContentEditorCmdlet : BaseHistoryEntryCmdlet
	{
		const string LICENSE_FILE_EXTENSION = ".txt";

		#region license-path parameters

		[Parameter(Mandatory = true, Position = 0, HelpMessage = "Path to the license file", 
			ParameterSetName = "license-path")]
		[Alias("path")]
		[ValidateNotNullOrEmpty]
		public string LicensePath { get; set; }

		[Parameter(Mandatory = false, Position = 1,
			ParameterSetName = "license-path")]
		public SwitchParameter Force { get; set; }

		#endregion

		#region license-key parameters

		[Parameter(Mandatory = true, Position = 0, HelpMessage = "Domain path",
			ParameterSetName = "license-key")]
		[ValidateNotNullOrEmpty]
		public string Domain { get; set; }

		[Parameter(Mandatory = true, Position = 1, HelpMessage = "License key string",
			ParameterSetName = "license-key")]
		[ValidateNotNullOrEmpty]
		public string LicenseKey { get; set; }

		#endregion

		#region shared parameters

		[Parameter(Mandatory = false, Position = 2)]
        [Alias("proj")]
		[ValidateNotNull]
		public Models.ISHDeployment ISHDeployment { get; set; }

		#endregion

		private ISHPaths _ishPaths;
        protected override ISHPaths IshPaths => _ishPaths ?? (_ishPaths = new ISHPaths(ISHDeployment ?? ISHProjectProvider.Instance.ISHDeployment));

        public override void ExecuteCmdlet()
        {
			BaseAction action;
			switch (ParameterSetName)
			{
				case "license-key":
					action = new FileCreateAction(Logger,
									IshPaths.LicenceFolderPath,
									String.Concat(Domain, LICENSE_FILE_EXTENSION),
									LicenseKey);
					break;
				case "license-path":
					action = new FileCopyAction(Logger,
									LicensePath,
									IshPaths.LicenceFolderPath,
									Force);
					break;
				default:
					throw new ArgumentException($"Either {nameof(LicensePath)} or {nameof(LicenseKey)} shoule be provided.");
			}

            action.Execute();
		}
	}
}