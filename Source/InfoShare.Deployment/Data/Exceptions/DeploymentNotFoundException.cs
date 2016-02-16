using System;

namespace InfoShare.Deployment.Data.Exceptions
{
    public class DeploymentNotFoundException : Exception
    {
        public DeploymentNotFoundException()
        { }

        public DeploymentNotFoundException(string message)
            : base(message)
        { }

        public DeploymentNotFoundException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
