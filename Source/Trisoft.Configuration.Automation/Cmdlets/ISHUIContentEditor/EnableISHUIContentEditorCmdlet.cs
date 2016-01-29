using System.Management.Automation;
using Trisoft.Configuration.Automation.Core.Invokers;

namespace Trisoft.Configuration.Automation.Cmdlets.ISHUIContentEditor
{
    [Cmdlet(VerbsCommonTrisoftConfiguration.Enable, "ISHUIContentEditor", SupportsShouldProcess = false)]
    public sealed class EnableISHUIContentEditorCmdlet : BaseConfigurationCmdlet
    {
        protected override void BeginProcessing()
        {
            SetInvoker(new ISHUIContentEditorInvoker(Logger, IshProject, RollbackOnFailure));
        }
    }
}