using System.IO;
using System.Management.Automation;

namespace InfoShare.Deployment.Cmdlets.ISHDeployment
{
    [Cmdlet(VerbsCommon.Get, CmdletNames.ISHDeploymentHistory)]
    public class GetISHDeploymentHistoryCmdlet : BaseCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, HelpMessage = "Suffix of the already deployed Content Manager instance", ParameterSetName = "SuffixParams")]
        [Alias("Suffix")]
        public string Deployment { get; set; }

        [Parameter(Mandatory = true, Position = 0, ParameterSetName = "DeploymentParams")]
        [Alias("proj")]
        [ValidateNotNull]
        public Models.ISHDeployment ISHDeployment { get; set; }

        public override void ExecuteCmdlet()
        {
            var suffix = (ISHDeployment != null) ? ISHDeployment.Suffix : Deployment;
            var filePath = BaseHistoryEntryCmdlet.GetHistoryFilePath(suffix);

            if (!File.Exists(filePath))
            {
                WriteWarning("There is no customization history for such deployment found!");
                return;
            }

            BaseHistoryEntryCmdlet.HistoryFileMutex.WaitOne();
            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    WriteObject(reader.ReadToEnd());
                }
            }
            finally
            {
                BaseHistoryEntryCmdlet.HistoryFileMutex.ReleaseMutex();
            }
        }
    }
}
