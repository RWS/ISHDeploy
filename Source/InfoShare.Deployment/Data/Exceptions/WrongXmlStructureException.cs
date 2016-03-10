using System;

namespace InfoShare.Deployment.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when xml file has wrong and unexpected structure.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class WrongXmlStructureException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXmlStructureException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        public WrongXmlStructureException(string filePath, string message)
            : this(filePath, message, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXmlStructureException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public WrongXmlStructureException(string filePath, string message, Exception innerException)
            : base($"File '{filePath}' has wrong structure. {message}", innerException)
        { }
    }
}
