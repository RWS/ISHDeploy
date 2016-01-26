using System.Management.Automation;
using System.Reflection;

namespace Trisoft.Configuration.Automation.Cmdlets.Module
{
    [Cmdlet(VerbsCommon.Get, "Version", SupportsShouldProcess = false)]
    public sealed class GetVersion : Cmdlet
    {
        protected override void ProcessRecord()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            WriteObject(version);
        }
    }
}
