using System;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Commands
{
    public abstract class BaseCommandWithResult<TResult> : BaseCommand
    {
        protected readonly Action<TResult> GetResult; 

        protected BaseCommandWithResult(ILogger logger, Action<TResult> getResult) : base(logger)
        {
            GetResult = getResult;
        }

        protected abstract TResult ExecuteWithResult();

        public sealed override void Execute()
        {
            var result = ExecuteWithResult();

            GetResult(result);
        }
    }
}
