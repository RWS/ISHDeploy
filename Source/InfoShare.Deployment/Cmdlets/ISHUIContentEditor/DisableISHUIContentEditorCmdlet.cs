using System.Management.Automation;
using InfoShare.Deployment.Core.CmdSets.ISHUIContentEditor;
using InfoShare.Deployment.Core.Models;

namespace InfoShare.Deployment.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(VerbsLifecycle.Disable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class DisableISHUIContentEditorCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        [ValidateNotNullOrEmpty]
        public ISHProject IshProject { get; set; }

        //TODO: Is that really needed?
        [Parameter(Mandatory = false, Position = 2, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        public bool RollbackOnFailure { get; set; }

        public override void ExecuteCmdlet()
        {
            IshProject = new ISHProject {AuthorFolderPath = @"e:\Projects\RnDProjects\Trisoft\Dev\Server.Web\Websites" };
            RollbackOnFailure = true;

            // Calling the set of command with entry parameters
            var cmdSet = new DisableISHUIContentEditorCmdSet(this, IshProject, RollbackOnFailure);

            cmdSet.Run();
        }
    }
}
