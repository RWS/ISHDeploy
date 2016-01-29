namespace Trisoft.Configuration.Automation.Core
{
    public interface ICommandRestorable : ICommand
    {
        void Backup();
        void Rollback();
    }
}
