using System;

namespace ISHDeploy.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when user wants to retrieve deployment by name that does not exist on the system.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class DeploymentNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentNotFoundException"/> class.
        /// </summary>
        public DeploymentNotFoundException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DeploymentNotFoundException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DeploymentNotFoundException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public DeploymentNotFoundException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
