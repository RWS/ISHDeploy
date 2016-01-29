namespace Trisoft.Configuration.Automation.Core
{
    public interface ICommandInvoker<in T> where T : ICommand
    {
        void AddCommand(T command);
        void Invoke();
        void Rollback();
    }
}
