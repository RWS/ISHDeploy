using System;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions
{
    public abstract class BaseActionWithResult<TResult> : BaseAction
    {
        protected readonly Action<TResult> ReturnResult; 

        protected BaseActionWithResult(ILogger logger, Action<TResult> returnResult) : base(logger)
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
