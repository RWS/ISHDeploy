using System;
using System.Management.Automation;
using Trisoft.Configuration.Automation.Core;
using Trisoft.Configuration.Automation.Core.Model;

namespace Trisoft.Configuration.Automation.Cmdlets
{
    public abstract class BaseConfigurationCmdlet : Cmdlet
    {
        protected readonly ILogger Logger;
        protected ICommandInvoker<ICommand> Invoker { get; private set; }

        [Parameter(Mandatory = true, Position = 1, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        [ValidateNotNullOrEmpty]
        public ISHProject IshProject { get; set; }

        [Parameter(Mandatory = false, Position = 2, ValueFromPipelineByPropertyName = false, ParameterSetName = "ParameterGroup")]
        public bool RollbackOnFailure { get; set; }

        protected BaseConfigurationCmdlet()
        {
            Logger = CmdletsLogger.Instance(this);
        }

        protected void SetInvoker(ICommandInvoker<ICommand> invoker)
        {
            Invoker = invoker;
        }

        protected override void ProcessRecord()
        {
            try
            {
                Invoker.Invoke();
            }
            catch (Exception exception)
            {
                if (RollbackOnFailure)
                {
                    Invoker.Rollback();
                }
                ThrowTerminatingError(new ErrorRecord(exception, base.GetType().Name, ErrorCategory.NotSpecified, null));
            }
        }
    }
}
