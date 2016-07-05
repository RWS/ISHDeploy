using System;
using ISHDeploy.Data.Actions.File;
using ISHDeploy.Interfaces;

namespace ISHDeploy.Data.Actions.Assert
{
    /// <summary>
    /// Implements file read all text action.
    /// </summary>
    /// <seealso cref="BaseActionWithResult{TResult}" />
    public class AssertAction : BaseAction
    {
        /// <summary>
        /// The action
        /// </summary>
        private readonly Func<bool> _condition;

        /// <summary>
        /// The Exception message
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
        /// <exception cref="System.NotImplementedException"></exception>
        public override void Execute()
        {
            if (_condition.Invoke())
            {
                throw new Exception(_message);
            }
        }
    }
}
