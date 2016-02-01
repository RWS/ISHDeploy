using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHUIContentEditor;
using InfoShare.Deployment.Models;

namespace InfoShare.Deployment.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(VerbsLifecycle.Enable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class EnableISHUIContentEditorCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        [ValidateNotNullOrEmpty]
        public ISHProject IshProject { get; set; }

        //TODO: Is that really needed?
        [Parameter(Mandatory = false, Position = 2, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        public bool RollbackOnFailure { get; set; }

        public override void ExecuteCmdlet()
        {
            // Calling the set of command with entry parameters
            var cmdSet = new EnableISHUIContentEditorCmdSet(this, IshProject, RollbackOnFailure);

            cmdSet.Run();
        }
    }
}