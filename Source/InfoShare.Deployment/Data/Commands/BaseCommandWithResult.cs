using System;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Commands
{
    public abstract class BaseCommandWithResult<TResult> : BaseCommand
    {
        protected readonly Action<TResult> ReturnResult; 

        protected BaseCommandWithResult(ILogger logger, Action<TResult> returnResult) : base(logger)
        {
            ReturnResult = returnResult;
        }

        protected abstract TResult ExecuteWithResult();

        public sealed override void Execute()
        {
            var result = ExecuteWithResult();

            ReturnResult(result);
        }
    }
}
