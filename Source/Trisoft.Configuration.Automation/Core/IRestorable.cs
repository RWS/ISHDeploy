namespace Trisoft.Configuration.Automation.Core
{
    public interface IRestorable : ICommand
    {
        void Backup();
        void Rollback();
    }
}
