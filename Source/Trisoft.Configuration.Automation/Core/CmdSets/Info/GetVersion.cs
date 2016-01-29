using Trisoft.Configuration.Automation.Core.Invokers;
using Trisoft.Configuration.Automation.Core.Models;

namespace Trisoft.Configuration.Automation.Core.CmdSets.Info
{
    public class GetVersion
    {
        private readonly ICommandInvoker<ICommand> _invoker; 

        public GetVersion(ILogger logger, ISHProject ishProject, bool enableBackup)
        {
            _invoker = new CommandInvoker<ICommand>(logger, ishProject, enableBackup, "InfoShare ContentEditor activation");

            //TODO: fix _invoker.AddCommand(new GetVersionCommand());
        }

        public void Run()
        {
            _invoker.Invoke();
        }
    }
}
