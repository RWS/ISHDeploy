using System;
using System.Management.Automation;
using Trisoft.Configuration.Automation.Core.Commands;

namespace Trisoft.Configuration.Automation.Cmdlets.Info
{
    [Cmdlet(VerbsCommon.Get, "Version")]
    public sealed class GetVersionCmdlet : BaseCmdlet
    {
        public override void ExecuteCmdlet()
        {
            // Calling the command directly
            Version result = null;
            var command = new GetVersionCommand(version => result = version);
            
            command.Execute();

            WriteObject(result);
        }
    }
}
