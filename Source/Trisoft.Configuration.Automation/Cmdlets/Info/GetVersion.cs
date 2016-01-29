using System.Management.Automation;
using Trisoft.Configuration.Automation.Core.Commands;

namespace Trisoft.Configuration.Automation.Cmdlets.Info
{
    [Cmdlet(VerbsCommon.Get, "Version", SupportsShouldProcess = false)]
    public sealed class GetVersionCmdlet : BaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            // Calling of the command directly
            var command = new GetVersionCommand();

            WriteObject(command.Execute());
        }
    }
}
