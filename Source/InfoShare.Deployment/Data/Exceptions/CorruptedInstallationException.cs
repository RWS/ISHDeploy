using System;
namespace InfoShare.Deployment.Data.Exceptions
{
    public class CorruptedInstallationException : Exception
    {
        public CorruptedInstallationException()
        { }

        public CorruptedInstallationException(string message)
            : base(message)
        { }

        public CorruptedInstallationException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
