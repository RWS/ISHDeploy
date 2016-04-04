using System;

namespace ISHDeploy.Data.Exceptions
{
    /// <summary>
    /// The exception that is thrown when XPath should return a result but does not return anything.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class WrongXPathException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXPathException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="xpath">The xpath to the element.</param>
        public WrongXPathException(string filePath, string xpath)
            : this(filePath, xpath, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrongXPathException"/> class.
        /// </summary>
        /// <param name="filePath">The xml file path that caused the exception.</param>
        /// <param name="xpath">The xpath to the element.</param>
        /// <param name="innerException">The inner exception.</param>
        public WrongXPathException(string filePath, string xpath, Exception innerException)
            : base($"File '{filePath}' does not contain nodes within the xpath: {xpath}.", innerException)
        { }
    }
}
