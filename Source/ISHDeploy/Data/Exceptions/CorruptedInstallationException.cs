using System;
namespace ISHDeploy.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when deployment state is changes and is those changes prevent cmdlets from successful operation.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class CorruptedInstallationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CorruptedInstallationException"/> class.
        /// </summary>
        public CorruptedInstallationException()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CorruptedInstallationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public CorruptedInstallationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CorruptedInstallationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="inner">The inner exception.</param>
        public CorruptedInstallationException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
