namespace Trisoft.Configuration.Automation.Core
{
    public interface ICommandInvoker
    {
        void AddCommand(IExecutable command);
        void Invoke();
        void Rollback();
    }
}
