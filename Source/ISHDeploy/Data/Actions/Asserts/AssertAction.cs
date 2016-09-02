using System;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Asserts
{
    /// <summary>
    /// The action that verify condition and generates an error in case if condition returns <see langword="true"/>
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class AssertAction : BaseAction
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
        /// Initializes a new instance of the <see cref="FileReadAllTextAction"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="condition">The condition to check.</param>
        /// <param name="message">The message in case of condition returns true.</param>
        public AssertAction(ILogger logger, Func<bool> condition, string message)
            : base(logger)
        {
            _condition = condition;
            _message = message;
        }

        /// <summary>
        /// Executes current action.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public override void Execute()
        {
            if (_condition.Invoke())
            {
                throw new Exception(_message);
            }
        }
    }
}
