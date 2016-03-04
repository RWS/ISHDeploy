using System.Linq;
using System.Management.Automation;
using InfoShare.Deployment.Business.Operations.ISHDeployment;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Get, "ISHDeployment")]
    public class GetISHDeploymentCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, HelpMessage = "Name of the already deployed Content Manager instance")]
        [Alias("Suffix")]
        public string Name { get; set; }

        public override void ExecuteCmdlet()
        {
            var operation = new GetISHDeploymentOperation(Logger, Name);

            var result = operation.Run().ToArray();

            if (Name != null && result.Count() == 1)
            {
                WriteObject(result[0]);
                return;
            }

            WriteObject(result);
        }
    }
}
