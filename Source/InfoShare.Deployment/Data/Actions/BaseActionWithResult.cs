using System;
using InfoShare.Deployment.Interfaces;

namespace InfoShare.Deployment.Data.Actions
{
    /// <summary>
    /// Base class for action that can be executed and return execution result.
    /// </summary>
    /// <typeparam name="TResult">The type of the execution result.</typeparam>
    /// <seealso cref="BaseAction" />
    public abstract class BaseActionWithResult<TResult> : BaseAction
    {
        /// <summary>
        /// The execution result.
        /// </summary>
        protected readonly Action<TResult> ReturnResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseActionWithResult{TResult}"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="returnResult">The delegate that returns execution result.</param>
        protected BaseActionWithResult(ILogger logger, Action<TResult> returnResult) : base(logger)
        {
            ReturnResult = returnResult;
        }

        /// <summary>
        /// Executes current action and returns result.
        /// </summary>
        /// <returns></returns>
        protected abstract TResult ExecuteWithResult();

        /// <summary>
        /// Executes current action.
        /// </summary>
        public sealed override void Execute()
        {
            var result = ExecuteWithResult();

            ReturnResult(result);
        }
    }
}
