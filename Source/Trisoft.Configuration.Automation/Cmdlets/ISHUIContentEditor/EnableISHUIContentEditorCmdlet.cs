using System.Management.Automation;
using System.Windows.Input;
using Trisoft.Configuration.Automation.Core;
using Trisoft.Configuration.Automation.Core.CmdSets.ISHUIContentEditor;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(TrisoftVerbsCommon.Enable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class EnableISHUIContentEditorCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        [ValidateNotNullOrEmpty]
        public ISHProject IshProject { get; set; }

        [Parameter(Mandatory = false, Position = 2, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        public bool RollbackOnFailure { get; set; }

        public override void ExecuteCmdlet()
        {
            // Calling of the set of command with entry parameters
            var cmdSet = new ISHUIContentEditorCmdSet(this, IshProject, RollbackOnFailure);

            cmdSet.Execute();
        }
    }
}