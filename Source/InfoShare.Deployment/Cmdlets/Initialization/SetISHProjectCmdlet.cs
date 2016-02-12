using System;
using System.Collections.Generic;
using System.Management.Automation;
using InfoShare.Deployment.Providers;

namespace InfoShare.Deployment.Cmdlets.Initialization
{
    [Cmdlet(VerbsCommon.Set, "ISHDeployment", SupportsShouldProcess = false)]
    public sealed class SetISHProjectCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipelineByPropertyName = false, HelpMessage = "Path to installed Content Manager instance")]
        [Alias("Path")]
        [ValidateNotNullOrEmpty]
        public string InstallPath { get; set; }

        [Parameter(Mandatory = false, Position = 1, ValueFromPipelineByPropertyName = false, HelpMessage = "Suffix of the installed Content Manager instance")]
        [Alias("sf")]
        public string Suffix { get; set; }

        public override void ExecuteCmdlet()
        {
            var ishProject = new Models.ISHDeployment(new Dictionary<string, string>(), new Version());

            ISHProjectProvider.Instance.InitializeIshProject(ishProject);
        }
    }
}
