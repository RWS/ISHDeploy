using Trisoft.Configuration.Automation.Core.Commands;

namespace Trisoft.Configuration.Automation.Core.Invokers
{
    public interface ICommandInvoker
    {
        void AddCommand(ICommand command);
    }
}
