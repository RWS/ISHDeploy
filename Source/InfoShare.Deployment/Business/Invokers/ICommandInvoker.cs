using InfoShare.Deployment.Interfaces.Commands;

namespace InfoShare.Deployment.Business.Invokers
{
    public interface ICommandInvoker
    {
        void AddCommand(ICommand command);
    }
}
