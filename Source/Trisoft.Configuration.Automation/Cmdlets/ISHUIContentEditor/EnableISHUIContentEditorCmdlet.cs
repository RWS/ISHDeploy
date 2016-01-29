using System.Management.Automation;
using Trisoft.Configuration.Automation.Core;
using Trisoft.Configuration.Automation.Core.CmdSets.ISHUIContentEditor;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(TrisoftVerbsCommon.Enable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class EnableISHUIContentEditorCmdlet : BaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            var vm = new ISHUIContentEditorCmdSet(this, new ISHProject(), false);

            vm.Run();
        }
    }
}