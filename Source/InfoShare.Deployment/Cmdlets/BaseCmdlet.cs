using System;
using System.Management.Automation;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Cmdlets
{
    public abstract class BaseCmdlet : Cmdlet, ILogger
    {
        private const int ProgressActivityId = 11;
        private const int ParentProgressActivityId = 22;
        private ProgressRecord _progressRecord;
        private ProgressRecord _parentProgressRecord;

        protected BaseCmdlet()
        {
            ObjectFactory.SetInstance<IFileManager>(new FileManager(this));
        }

        public abstract void ExecuteCmdlet();

        protected override void ProcessRecord()
        {
            try
            {
                base.ProcessRecord();

                ExecuteCmdlet();
            }
            catch (Exception ex)
            {
                ThrowTerminatingError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
            }
        }

        public new void WriteVerbose(string message)
        {
            base.WriteVerbose(message);
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
            
            WriteProgress(_progressRecord);
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

            WriteProgress(_parentProgressRecord);
        }

        public new void WriteDebug(string message)
        {
            base.WriteDebug(message);
        }

        public new void WriteWarning(string message)
        {
            base.WriteWarning(message);
        }
        
        public void WriteError(Exception ex, object errorObject = null)
        {
            WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.NotSpecified, errorObject));
        }
    }
}
