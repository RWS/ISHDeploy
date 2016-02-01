using InfoShare.Deployment.Core.Commands;

namespace InfoShare.Deployment.Core.Invokers
{
    public interface ICommandInvoker
    {
        void AddCommand(ICommand command);
    }
}
