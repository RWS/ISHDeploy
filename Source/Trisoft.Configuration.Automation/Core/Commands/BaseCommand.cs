namespace Trisoft.Configuration.Automation.Core.Commands
{
    public abstract class BaseCommand
    {
        public readonly ILogger Logger;

        protected BaseCommand(ILogger logger)
        {
            Logger = logger;
        }
    }
}
