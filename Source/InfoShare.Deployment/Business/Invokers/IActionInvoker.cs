using InfoShare.Deployment.Interfaces.Actions;

namespace InfoShare.Deployment.Business.Invokers
{
    public interface IActionInvoker
    {
        void AddAction(IAction action);
        void Invoke();
    }
}
