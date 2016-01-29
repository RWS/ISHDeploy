using System.Management.Automation;
using Trisoft.Configuration.Automation.Core.Commands;

namespace Trisoft.Configuration.Automation.Cmdlets.Info
{
    [Cmdlet(VerbsCommon.Get, "Version", SupportsShouldProcess = false)]
    public sealed class GetVersion : Cmdlet
    {
        protected override void ProcessRecord()
        {
            var command = new GetVersionCommand();
            WriteObject(command.Execute());
        }
    }
}
