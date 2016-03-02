using System;

namespace InfoShare.Deployment.Data.Exceptions
{
    public class WrongXmlStructureException : Exception
    {
        public WrongXmlStructureException(string filePath, string message)
            : this(filePath, message, null)
        { }

        public WrongXmlStructureException(string filePath, string message, Exception innerException)
            : base($"File '{filePath}' has wrong structure. {message}", innerException)
        { }
    }
}
