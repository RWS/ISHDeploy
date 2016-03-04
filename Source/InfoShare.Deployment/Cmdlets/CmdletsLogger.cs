using InfoShare.Deployment.Interfaces;
using System;
using System.Management.Automation;

namespace InfoShare.Deployment.Cmdlets
{
    public sealed class CmdletsLogger : ILogger
    {
        private static readonly CmdletsLogger _instance = new CmdletsLogger();
        private static BaseCmdlet _cmdlet;
        private const int ProgressActivityId = 11;
        private const int ParentProgressActivityId = 22;
        private ProgressRecord _progressRecord;
        private ProgressRecord _parentProgressRecord;

        public static ILogger Instance()
        {
            return _instance;
        }

        public static void Initialize(BaseCmdlet cmdlet)
        {
            _cmdlet = cmdlet;
        }

        public void WriteVerbose(string message)
        {
            _cmdlet.WriteVerbose(message);
        }

        public void WriteProgress(string activity, string statusDescription, int percentComplete = -1)
        {
            if (_progressRecord == null)
            {
                _progressRecord = new ProgressRecord(ProgressActivityId, activity, statusDescription);
            }

            _progressRecord.ParentActivityId = (_parentProgressRecord != null) ? ParentProgressActivityId : -1;
            _progressRecord.Activity = activity;
            _progressRecord.StatusDescription = statusDescription;
            _progressRecord.PercentComplete = percentComplete;

            _cmdlet.WriteProgress(_progressRecord);
        }

        public void WriteParentProgress(string activity, string statusDescription, int percentComplete)
        {
            if (_parentProgressRecord == null)
            {
                _parentProgressRecord = new ProgressRecord(ParentProgressActivityId, activity, statusDescription);
            }

            _parentProgressRecord.Activity = activity;
            _parentProgressRecord.StatusDescription = statusDescription;
            _parentProgressRecord.PercentComplete = percentComplete;

            _cmdlet.WriteProgress(_parentProgressRecord);
        }

        public void WriteDebug(string message)
        {
            _cmdlet.WriteDebug(message);
        }

        public void WriteWarning(string message)
        {
            _cmdlet.WriteWarning(message);
        }

        public void WriteError(Exception ex, object errorObject = null)
        {
            _cmdlet.WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.NotSpecified, errorObject));
        }
    }
}
