using System.Linq;
using System.Management.Automation;
using InfoShare.Deployment.Business.CmdSets.ISHDeployment;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Get, "ISHDeployment")]
    public class GetISHDeploymentCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, HelpMessage = "Suffix of the already deployed Content Manager instance")]
        [Alias("Suffix")]
        public string Deployment { get; set; }

        public override void ExecuteCmdlet()
        {
            var cmdset = new GetISHDeploymentCmdSet(Logger, Deployment);

            var result = cmdset.Run().ToArray();

            if (Deployment != null && result.Count() == 1)
            {
                WriteObject(result[0]);
                return;
            }

            WriteObject(result);
        }
    }
}
