using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Data.Actions
{
    public abstract class BaseAction : IAction
    {
        protected readonly ILogger Logger;

        protected BaseAction(ILogger logger)
        {
            Logger = logger;
        }

        public abstract void Execute();
    }
}
