﻿using System;
using System.Management.Automation;
using InfoShare.Deployment.Data.Services;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Cmdlets
{
    public abstract class BaseCmdlet : Cmdlet, ILogger
    {
        protected BaseCmdlet()
        {
            ObjectFactory.SetInstance<IFileManager, FileManager>(new FileManager(this));
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
                WriteError(ex);
            }
        }

        public new void WriteVerbose(string message)
        {
            base.WriteVerbose(message);
        }

        public void WriteProgress(string message, string statusDescription)
        {
            WriteProgress(new ProgressRecord(0, message, statusDescription));
        }

        public void WriteDetail(string message)
        {
            base.WriteCommandDetail(message);
        }

        public new void WriteDebug(string message)
        {
            base.WriteDebug(message);
        }

        public new void WriteWarning(string message)
        {
            base.WriteWarning(message);
        }

        public void WriteError(Exception ex)
        {
            WriteError(new ErrorRecord(ex, string.Empty, ErrorCategory.CloseError, null));
        }
    }
}
