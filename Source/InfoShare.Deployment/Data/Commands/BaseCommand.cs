using InfoShare.Deployment.Interfaces;
using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Data.Commands
{
    public abstract class BaseCommand : ICommand
    {
        protected readonly ILogger Logger;

        protected BaseCommand(ILogger logger)
        {
            Logger = logger;
        }

        public abstract void Execute();
    }
}
