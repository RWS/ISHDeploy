using System;
using ISHDeploy.Common.Interfaces;

namespace ISHDeploy.Data.Actions.Asserts
{
    /// <summary>
    /// The action that verify condition and output Verbos in case if condition returns <see langword="true"/>
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class WriteVerboseAction : BaseAction
    {
        /// <summary>
        /// The condition
        /// </summary>
        private readonly Func<bool> _condition;

        /// <summary>
        /// The exception message
        /// </summary>
        private readonly string _message;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteVerboseAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The message in case of condition returns true.</param>
        public WriteVerboseAction(ILogger logger, Func<bool> condition, string message)
            : base(logger)
        {
            _condition = condition;
            _message = message;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        public override void Execute()
        {
            if (_condition.Invoke())
            {
                Logger.WriteVerbose(_message);
            }
        }
    }
}
